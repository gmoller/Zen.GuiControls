﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zen.GuiControls.TheControls;
using Zen.Utilities;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    public static class ControlCreator
    {
        public static Controls CreateFromSpecification(string spec, List<KeyValuePair<string, string>> pairs = null, int level = 1)
        {
            // determine calling type
            var callingType = ObjectCreator.GetCallerType(level);

            var allTheLines = spec.SplitToLines().ToArray();
            var pairsDictionary = pairs?.ToDictionary(pair => pair.Key, pair => pair.Value);

            var potentialControls = GetPotentialControls(allTheLines, pairsDictionary);

            var staging = GetStagingControls(potentialControls, callingType);

            // handle contains
            foreach (var control in staging.Values)
            {
                foreach (var item in control.contains)
                {
                    var stagingItem = staging[item];
                    var childControl = (Control)stagingItem.control;
                    var parentContainerAlignment = stagingItem.parentContainerAlignment;
                    var offset = stagingItem.offset;

                    control.control.AddControl(childControl, parentContainerAlignment.parent, parentContainerAlignment.child, offset);
                }
            }

            // return the one(s) that don't have a parent
            var list = new Controls();
            foreach (var control in staging.Values)
            {
                if (control.control.Parent is null)
                {
                    list.Add(control.control.Name, control.control);
                }
            }

            return list;
        }

        private static Dictionary<string, (string controlType, StateDictionary state)> GetPotentialControls(string[] allTheLines, Dictionary<string, string> pairs)
        {
            var potentialControls = new Dictionary<string, (string controlType, StateDictionary state)>();

            var currentControlName = string.Empty;
            var betweenSquirlies = false;
            for (var i = 0; i < allTheLines.Length; i++)
            {
                var line = allTheLines[i];

                if (line.Trim().StartsWith("//")) continue; // ignore comments
                if (!line.Trim().HasValue()) continue; // ignore black lines

                if (line.Trim().StartsWith("{"))
                {
                    betweenSquirlies = true;
                    var controlNameAndType = allTheLines[i - 1];
                    currentControlName = controlNameAndType.Split(':')[0].Trim();
                    var controlType = controlNameAndType.Split(':')[1].Trim();
                    (string controlType, StateDictionary state) state = (controlType, new StateDictionary());
                    potentialControls.Add(currentControlName, state);
                }
                else if (line.Trim().StartsWith("}"))
                {
                    betweenSquirlies = false;
                    currentControlName = string.Empty;
                }
                else
                {
                    if (betweenSquirlies)
                    {
                        var potentialControl = potentialControls[currentControlName];
                        var key = line.GetTextToLeftOfCharacter(':').Trim();
                        var value = line.GetTextToRightOfCharacter(':').Trim();
                        if (value.ContainsTwoCharactersOf('%'))
                        {
                            // find key
                            var key2 = value.GetTextBetweenCharacters('%');
                            var value2 = pairs[key2];
                            value = value.Replace($"%{key2}%", value2);
                        }

                        potentialControl.state[key] = value;
                    }
                }
            }

            return potentialControls;
        }

        private static Dictionary<string, (IControl control, List<string> contains, (Alignment parent, Alignment child) parentContainerAlignment, PointI offset)> GetStagingControls(Dictionary<string, (string controlType, StateDictionary state)> potentialControls, CallingType callingType)
        {
            var controls = new Dictionary<string, (IControl control, List<string> contains, (Alignment parent, Alignment child) parentContainerAlignment, PointI offset)>();
            var templates = new Dictionary<string, IControl>();

            foreach (var potentialControl in potentialControls)
            {
                var name = potentialControl.Key;
                var type = potentialControl.Value.controlType;
                var state = potentialControl.Value.state;

                var isTemplate = type.StartsAndEndsWith('<', '>');
                if (isTemplate)
                {
                    var template = InstantiateControl(name, type.RemoveFirstAndLastCharacters(), state, callingType.TypeFullName, callingType.AssemblyFullName, templates);
                    templates.Add(name, template);
                }
                else
                {
                    var control = InstantiateControl(name, type, state, callingType.TypeFullName, callingType.AssemblyFullName, templates);

                    var packagesList = state.GetAsListOfStrings("Packages", new List<string>());
                    var onClickEventHandler = GetEventHandler(state, "Click");
                    if (onClickEventHandler.HasValue())
                    {
                        packagesList.Add(onClickEventHandler);
                    }
                    var onDragEventHandler = GetEventHandler(state, "Drag");
                    if (onDragEventHandler.HasValue())
                    {
                        packagesList.Add(onDragEventHandler);
                    }

                    control.AddPackages(packagesList, callingType.TypeFullName, callingType.AssemblyFullName);

                    var containsList = state.GetAsListOfStrings("Contains", new List<string>());
                    var parentContainerAlignment = GetParentContainerAlignment(state);
                    var offset = state.GetAsPointI("Offset", PointI.Empty);
                    controls.Add(name, (control, containsList, parentContainerAlignment, offset));
                }
            }

            return controls;
        }

        private static IControl InstantiateControl(string name, string type, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName, Dictionary<string, IControl> templates)
        {
            IControl control;
            switch (type)
            {
                case "Label":
                    control = Label.Create(name, state, callingTypeFullName, callingAssemblyFullName);
                    control = (Label)UpdateGenericProperties(control, state);
                    break;
                case "Button":
                    control = Button.Create(name, state, callingTypeFullName, callingAssemblyFullName);
                    control = (Button)UpdateGenericProperties(control, state);
                    break;
                case "Frame":
                    control = Frame.Create(name, state, callingTypeFullName, callingAssemblyFullName);
                    control = (Frame)UpdateGenericProperties(control, state);
                    break;
                case "Image":
                    control = Image.Create(name, state, callingTypeFullName, callingAssemblyFullName);
                    control = (Image)UpdateGenericProperties(control, state);
                    break;
                case "Slider":
                    control = Slider.Create(name, state, callingTypeFullName, callingAssemblyFullName);
                    control = (Slider)UpdateGenericProperties(control, state);
                    break;
                case "TextBox":
                    control = TextBox.Create(name, state, callingTypeFullName, callingAssemblyFullName);
                    control = (TextBox)UpdateGenericProperties(control, state);
                    break;
                default:
                    if (templates.ContainsKey(type))
                    {
                        var template = templates[type];
                        switch (template)
                        {
                            case Label lbl:
                                control = lbl.Clone();
                                control.Name = name;
                                control = Label.Update((Label)control, state, callingTypeFullName, callingAssemblyFullName);
                                control = (Label)UpdateGenericProperties(control, state);
                                break;
                            case Button btn:
                                control = btn.Clone();
                                control.Name = name;
                                control = Button.Update((Button)control, state, callingTypeFullName, callingAssemblyFullName);
                                control = (Button)UpdateGenericProperties(control, state);
                                break;
                            case Frame frm:
                                control = frm.Clone();
                                control.Name = name;
                                control = Frame.Update((Frame)control, state, callingTypeFullName, callingAssemblyFullName);
                                control = (Frame)UpdateGenericProperties(control, state);
                                break;
                            case Image img:
                                control = img.Clone();
                                control.Name = name;
                                control = Image.Update((Image)control, state, callingTypeFullName, callingAssemblyFullName);
                                control = (Image)UpdateGenericProperties(control, state);
                                break;
                            case Slider slr:
                                control = slr.Clone();
                                control.Name = name;
                                control = Slider.Update((Slider)control, state, callingTypeFullName, callingAssemblyFullName);
                                control = (Slider)UpdateGenericProperties(control, state);
                                break;
                            case TextBox txt:
                                control = txt.Clone();
                                control.Name = name;
                                control = TextBox.Update((TextBox)control, state, callingTypeFullName, callingAssemblyFullName);
                                control = (TextBox)UpdateGenericProperties(control, state);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(template), template, $"ControlType {template.GetType()} templating is not supported.");
                        }
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(type), type, $"ControlType {type} is not supported.");
                    }
                    break;
            }

            return control;
        }

        private static IControl UpdateGenericProperties(IControl control, StateDictionary state)
        {
            control.Position = state.GetAsPointI("Position", control.Position);
            control.PositionAlignment = state.GetAsAlignment("PositionAlignment", control.PositionAlignment);
            control.Size = state.GetAsPointI("Size", control.Size);
            control.Color = state.GetAsColor("Color", control.Color);
            control.BackgroundColor = state.GetAsColor("BackgroundColor", control.BackgroundColor);
            control.BorderColor = state.GetAsColor("BorderColor", control.BorderColor);
            control.Enabled = state.GetAsBoolean("Enabled", control.Enabled);
            control.Visible = state.GetAsBoolean("Visible", control.Visible);
            control.LayerDepth = state.GetAsSingle("LayerDepth", control.LayerDepth);

            return control;
        }

        private static string GetEventHandler(StateDictionary state, string eventHandlerName)
        {
            if (!state.ContainsKey($"On{eventHandlerName}")) return string.Empty;

            var eventHandlerAsString = state.GetAsString($"On{eventHandlerName}", string.Empty);
            var eventHandlerAsString2 = $"'Zen.GuiControls.PackagesClasses.Control{eventHandlerName}, Zen.GuiControls - {eventHandlerAsString}'"; // remove single quotes

            return eventHandlerAsString2;
        }

        private static (Alignment parent, Alignment child) GetParentContainerAlignment(StateDictionary state)
        {
            if (!state.ContainsKey("ParentContainerAlignment")) return (Alignment.TopLeft, Alignment.TopLeft);

            var parentContainerAlignment = state.GetAsParentContainerAlignment("ParentContainerAlignment",  ParentContainerAlignment.ParentTopLeftAlignsWithChildTopLeft);

            switch (parentContainerAlignment)
            {
                case ParentContainerAlignment.ParentTopLeftAlignsWithChildTopLeft:
                    return (Alignment.TopLeft, Alignment.TopLeft);
                case ParentContainerAlignment.ParentTopLeftAlignsWithChildMiddleLeft:
                    return (Alignment.TopLeft, Alignment.MiddleLeft);
                case ParentContainerAlignment.ParentTopLeftAlignsWithChildBottomLeft:
                    return (Alignment.TopLeft, Alignment.BottomLeft);
                case ParentContainerAlignment.ParentTopLeftAlignsWithChildTopCenter:
                    return (Alignment.TopLeft, Alignment.TopCenter);
                case ParentContainerAlignment.ParentTopLeftAlignsWithChildMiddleCenter:
                    return (Alignment.TopLeft, Alignment.MiddleCenter);
                case ParentContainerAlignment.ParentTopLeftAlignsWithChildBottomCenter:
                    return (Alignment.TopLeft, Alignment.BottomCenter);
                case ParentContainerAlignment.ParentTopLeftAlignsWithChildTopRight:
                    return (Alignment.TopLeft, Alignment.TopRight);
                case ParentContainerAlignment.ParentTopLeftAlignsWithChildMiddleRight:
                    return (Alignment.TopLeft, Alignment.MiddleRight);
                case ParentContainerAlignment.ParentTopLeftAlignsWithChildBottomRight:
                    return (Alignment.TopLeft, Alignment.BottomRight);

                case ParentContainerAlignment.ParentMiddleLeftAlignsWithChildTopLeft:
                    return (Alignment.MiddleLeft, Alignment.TopLeft);
                case ParentContainerAlignment.ParentMiddleLeftAlignsWithChildMiddleLeft:
                    return (Alignment.MiddleLeft, Alignment.MiddleLeft);
                case ParentContainerAlignment.ParentMiddleLeftAlignsWithChildBottomLeft:
                    return (Alignment.MiddleLeft, Alignment.BottomLeft);
                case ParentContainerAlignment.ParentMiddleLeftAlignsWithChildTopCenter:
                    return (Alignment.MiddleLeft, Alignment.TopCenter);
                case ParentContainerAlignment.ParentMiddleLeftAlignsWithChildMiddleCenter:
                    return (Alignment.MiddleLeft, Alignment.MiddleCenter);
                case ParentContainerAlignment.ParentMiddleLeftAlignsWithChildBottomCenter:
                    return (Alignment.MiddleLeft, Alignment.BottomCenter);
                case ParentContainerAlignment.ParentMiddleLeftAlignsWithChildTopRight:
                    return (Alignment.MiddleLeft, Alignment.TopRight);
                case ParentContainerAlignment.ParentMiddleLeftAlignsWithChildMiddleRight:
                    return (Alignment.MiddleLeft, Alignment.MiddleRight);
                case ParentContainerAlignment.ParentMiddleLeftAlignsWithChildBottomRight:
                    return (Alignment.MiddleLeft, Alignment.BottomRight);

                case ParentContainerAlignment.ParentBottomLeftAlignsWithChildTopLeft:
                    return (Alignment.BottomLeft, Alignment.TopLeft);
                case ParentContainerAlignment.ParentBottomLeftAlignsWithChildMiddleLeft:
                    return (Alignment.BottomLeft, Alignment.MiddleLeft);
                case ParentContainerAlignment.ParentBottomLeftAlignsWithChildBottomLeft:
                    return (Alignment.BottomLeft, Alignment.BottomLeft);
                case ParentContainerAlignment.ParentBottomLeftAlignsWithChildTopCenter:
                    return (Alignment.BottomLeft, Alignment.TopCenter);
                case ParentContainerAlignment.ParentBottomLeftAlignsWithChildMiddleCenter:
                    return (Alignment.BottomLeft, Alignment.MiddleCenter);
                case ParentContainerAlignment.ParentBottomLeftAlignsWithChildBottomCenter:
                    return (Alignment.BottomLeft, Alignment.BottomCenter);
                case ParentContainerAlignment.ParentBottomLeftAlignsWithChildTopRight:
                    return (Alignment.BottomLeft, Alignment.TopRight);
                case ParentContainerAlignment.ParentBottomLeftAlignsWithChildMiddleRight:
                    return (Alignment.BottomLeft, Alignment.MiddleRight);
                case ParentContainerAlignment.ParentBottomLeftAlignsWithChildBottomRight:
                    return (Alignment.BottomLeft, Alignment.BottomRight);

                case ParentContainerAlignment.ParentTopCenterAlignsWithChildTopLeft:
                    return (Alignment.TopCenter, Alignment.TopLeft);
                case ParentContainerAlignment.ParentTopCenterAlignsWithChildMiddleLeft:
                    return (Alignment.TopCenter, Alignment.MiddleLeft);
                case ParentContainerAlignment.ParentTopCenterAlignsWithChildBottomLeft:
                    return (Alignment.TopCenter, Alignment.BottomLeft);
                case ParentContainerAlignment.ParentTopCenterAlignsWithChildTopCenter:
                    return (Alignment.TopCenter, Alignment.TopCenter);
                case ParentContainerAlignment.ParentTopCenterAlignsWithChildMiddleCenter:
                    return (Alignment.TopCenter, Alignment.MiddleCenter);
                case ParentContainerAlignment.ParentTopCenterAlignsWithChildBottomCenter:
                    return (Alignment.TopCenter, Alignment.BottomCenter);
                case ParentContainerAlignment.ParentTopCenterAlignsWithChildTopRight:
                    return (Alignment.TopCenter, Alignment.TopRight);
                case ParentContainerAlignment.ParentTopCenterAlignsWithChildMiddleRight:
                    return (Alignment.TopCenter, Alignment.MiddleRight);
                case ParentContainerAlignment.ParentTopCenterAlignsWithChildBottomRight:
                    return (Alignment.TopCenter, Alignment.BottomRight);

                case ParentContainerAlignment.ParentMiddleCenterAlignsWithChildTopLeft:
                    return (Alignment.MiddleCenter, Alignment.TopLeft);
                case ParentContainerAlignment.ParentMiddleCenterAlignsWithChildMiddleLeft:
                    return (Alignment.MiddleCenter, Alignment.MiddleLeft);
                case ParentContainerAlignment.ParentMiddleCenterAlignsWithChildBottomLeft:
                    return (Alignment.MiddleCenter, Alignment.BottomLeft);
                case ParentContainerAlignment.ParentMiddleCenterAlignsWithChildTopCenter:
                    return (Alignment.MiddleCenter, Alignment.TopCenter);
                case ParentContainerAlignment.ParentMiddleCenterAlignsWithChildMiddleCenter:
                    return (Alignment.MiddleCenter, Alignment.MiddleCenter);
                case ParentContainerAlignment.ParentMiddleCenterAlignsWithChildBottomCenter:
                    return (Alignment.MiddleCenter, Alignment.BottomCenter);
                case ParentContainerAlignment.ParentMiddleCenterAlignsWithChildTopRight:
                    return (Alignment.MiddleCenter, Alignment.TopRight);
                case ParentContainerAlignment.ParentMiddleCenterAlignsWithChildMiddleRight:
                    return (Alignment.MiddleCenter, Alignment.MiddleRight);
                case ParentContainerAlignment.ParentMiddleCenterAlignsWithChildBottomRight:
                    return (Alignment.MiddleCenter, Alignment.BottomRight);

                case ParentContainerAlignment.ParentBottomCenterAlignsWithChildTopLeft:
                    return (Alignment.BottomCenter, Alignment.TopLeft);
                case ParentContainerAlignment.ParentBottomCenterAlignsWithChildMiddleLeft:
                    return (Alignment.BottomCenter, Alignment.MiddleLeft);
                case ParentContainerAlignment.ParentBottomCenterAlignsWithChildBottomLeft:
                    return (Alignment.BottomCenter, Alignment.BottomLeft);
                case ParentContainerAlignment.ParentBottomCenterAlignsWithChildTopCenter:
                    return (Alignment.BottomCenter, Alignment.TopCenter);
                case ParentContainerAlignment.ParentBottomCenterAlignsWithChildMiddleCenter:
                    return (Alignment.BottomCenter, Alignment.MiddleCenter);
                case ParentContainerAlignment.ParentBottomCenterAlignsWithChildBottomCenter:
                    return (Alignment.BottomCenter, Alignment.BottomCenter);
                case ParentContainerAlignment.ParentBottomCenterAlignsWithChildTopRight:
                    return (Alignment.BottomCenter, Alignment.TopRight);
                case ParentContainerAlignment.ParentBottomCenterAlignsWithChildMiddleRight:
                    return (Alignment.BottomCenter, Alignment.MiddleRight);
                case ParentContainerAlignment.ParentBottomCenterAlignsWithChildBottomRight:
                    return (Alignment.BottomCenter, Alignment.BottomRight);

                case ParentContainerAlignment.ParentTopRightAlignsWithChildTopLeft:
                    return (Alignment.TopRight, Alignment.TopLeft);
                case ParentContainerAlignment.ParentTopRightAlignsWithChildMiddleLeft:
                    return (Alignment.TopRight, Alignment.MiddleLeft);
                case ParentContainerAlignment.ParentTopRightAlignsWithChildBottomLeft:
                    return (Alignment.TopRight, Alignment.BottomLeft);
                case ParentContainerAlignment.ParentTopRightAlignsWithChildTopCenter:
                    return (Alignment.TopRight, Alignment.TopCenter);
                case ParentContainerAlignment.ParentTopRightAlignsWithChildMiddleCenter:
                    return (Alignment.TopRight, Alignment.MiddleCenter);
                case ParentContainerAlignment.ParentTopRightAlignsWithChildBottomCenter:
                    return (Alignment.TopRight, Alignment.BottomCenter);
                case ParentContainerAlignment.ParentTopRightAlignsWithChildTopRight:
                    return (Alignment.TopRight, Alignment.TopRight);
                case ParentContainerAlignment.ParentTopRightAlignsWithChildMiddleRight:
                    return (Alignment.TopRight, Alignment.MiddleRight);
                case ParentContainerAlignment.ParentTopRightAlignsWithChildBottomRight:
                    return (Alignment.TopRight, Alignment.BottomRight);

                case ParentContainerAlignment.ParentMiddleRightAlignsWithChildTopLeft:
                    return (Alignment.MiddleRight, Alignment.TopLeft);
                case ParentContainerAlignment.ParentMiddleRightAlignsWithChildMiddleLeft:
                    return (Alignment.MiddleRight, Alignment.MiddleLeft);
                case ParentContainerAlignment.ParentMiddleRightAlignsWithChildBottomLeft:
                    return (Alignment.MiddleRight, Alignment.BottomLeft);
                case ParentContainerAlignment.ParentMiddleRightAlignsWithChildTopCenter:
                    return (Alignment.MiddleRight, Alignment.TopCenter);
                case ParentContainerAlignment.ParentMiddleRightAlignsWithChildMiddleCenter:
                    return (Alignment.MiddleRight, Alignment.MiddleCenter);
                case ParentContainerAlignment.ParentMiddleRightAlignsWithChildBottomCenter:
                    return (Alignment.MiddleRight, Alignment.BottomCenter);
                case ParentContainerAlignment.ParentMiddleRightAlignsWithChildTopRight:
                    return (Alignment.MiddleRight, Alignment.TopRight);
                case ParentContainerAlignment.ParentMiddleRightAlignsWithChildMiddleRight:
                    return (Alignment.MiddleRight, Alignment.MiddleRight);
                case ParentContainerAlignment.ParentMiddleRightAlignsWithChildBottomRight:
                    return (Alignment.MiddleRight, Alignment.BottomRight);

                case ParentContainerAlignment.ParentBottomRightAlignsWithChildTopLeft:
                    return (Alignment.BottomRight, Alignment.TopLeft);
                case ParentContainerAlignment.ParentBottomRightAlignsWithChildMiddleLeft:
                    return (Alignment.BottomRight, Alignment.MiddleLeft);
                case ParentContainerAlignment.ParentBottomRightAlignsWithChildBottomLeft:
                    return (Alignment.BottomRight, Alignment.BottomLeft);
                case ParentContainerAlignment.ParentBottomRightAlignsWithChildTopCenter:
                    return (Alignment.BottomRight, Alignment.TopCenter);
                case ParentContainerAlignment.ParentBottomRightAlignsWithChildMiddleCenter:
                    return (Alignment.BottomRight, Alignment.MiddleCenter);
                case ParentContainerAlignment.ParentBottomRightAlignsWithChildBottomCenter:
                    return (Alignment.BottomRight, Alignment.BottomCenter);
                case ParentContainerAlignment.ParentBottomRightAlignsWithChildTopRight:
                    return (Alignment.BottomRight, Alignment.TopRight);
                case ParentContainerAlignment.ParentBottomRightAlignsWithChildMiddleRight:
                    return (Alignment.BottomRight, Alignment.MiddleRight);
                case ParentContainerAlignment.ParentBottomRightAlignsWithChildBottomRight:
                    return (Alignment.BottomRight, Alignment.BottomRight);

                default:
                    throw new ArgumentOutOfRangeException(nameof(parentContainerAlignment), parentContainerAlignment, $"ParentContainerAlignment {parentContainerAlignment} is not supported.");
            }
        }
    }
}