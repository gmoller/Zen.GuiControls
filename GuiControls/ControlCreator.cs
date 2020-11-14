using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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

        private static Dictionary<string, (string controlType, Dictionary<string, string> state)> GetPotentialControls(string[] allTheLines, Dictionary<string, string> pairs)
        {
            var potentialControls = new Dictionary<string, (string controlType, Dictionary<string, string> state)>();

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
                    (string controlType, Dictionary<string, string> state) state = (controlType, new Dictionary<string, string>());
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
                        if (value.StartsWith('%') && value.EndsWith('%'))
                        {
                            value = pairs[value.RemoveFirstAndLastCharacters()];
                        }

                        potentialControl.state[key] = value;
                    }
                }
            }

            return potentialControls;
        }

        private static Dictionary<string, (IControl control, List<string> contains, (Alignment parent, Alignment child) parentContainerAlignment, PointI offset)> GetStagingControls(Dictionary<string, (string controlType, Dictionary<string, string> state)> potentialControls, CallingType callingType)
        {
            var controls = new Dictionary<string, (IControl control, List<string> contains, (Alignment parent, Alignment child) parentContainerAlignment, PointI offset)>();
            var templates = new Dictionary<string, IControl>();

            foreach (var potentialControl in potentialControls)
            {
                var name = potentialControl.Key;
                var type = potentialControl.Value.controlType;
                var state = potentialControl.Value.state;

                if (type.StartsWith('<') && type.EndsWith('>'))
                {
                    var template = InstantiateControl(name, type.RemoveFirstAndLastCharacters(), state, callingType.TypeFullName, callingType.AssemblyFullName, templates);
                    templates.Add(name, template);
                }
                else
                {
                    var control = InstantiateControl(name, type, state, callingType.TypeFullName, callingType.AssemblyFullName, templates);

                    var packagesList = GetPackages(state);
                    var onClickEventHandler = GetOnClickEventHandler(state);
                    if (onClickEventHandler.HasValue())
                    {
                        packagesList.Add(onClickEventHandler);
                    }

                    control.AddPackages(packagesList, callingType.TypeFullName, callingType.AssemblyFullName);

                    var containsList = GetContains(state);
                    var parentContainerAlignment = GetParentContainerAlignment(state);
                    var offset = GetOffset(state);
                    controls.Add(name, (control, containsList, parentContainerAlignment, offset));
                }
            }

            return controls;
        }

        private static IControl InstantiateControl(string name, string type, Dictionary<string, string> state, string callingTypeFullName, string callingAssemblyFullName, Dictionary<string, IControl> templates)
        {
            IControl control;
            switch (type)
            {
                case "Label":
                    control = InstantiateLabel(name, state, callingTypeFullName, callingAssemblyFullName);
                    break;
                case "Button":
                    control = InstantiateButton(name, state);
                    break;
                case "Frame":
                    control = InstantiateFrame(name, state);
                    break;
                case "Image":
                    control = InstantiateImage(name, state);
                    break;
                default:
                    if (templates.ContainsKey(type))
                    {
                        var template = templates[type];
                        if (template is Label lbl)
                        {
                            control = lbl.Clone();
                            control.Name = name;
                            control = UpdateLabel((Label)control, state, callingTypeFullName, callingAssemblyFullName);
                        }
                        else if (template is Button btn)
                        {
                            control = btn.Clone();
                            control.Name = name;
                            control = UpdateButton((Button)control, state);
                        }
                        else if (template is Frame frm)
                        {
                            control = frm.Clone();
                            control.Name = name;
                            control = UpdateFrame((Frame)control, state);
                        }
                        else if (template is Image img)
                        {
                            control = img.Clone();
                            control.Name = name;
                            control = UpdateImage((Image)control, state);
                        }
                        else
                        {
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

        private static Label InstantiateLabel(string name, Dictionary<string, string> state, string callingTypeFullName, string callingAssemblyFullName)
        {
            try
            {
                var control = new Label(name, state["FontName"]);
                control = UpdateLabel(control, state, callingTypeFullName, callingAssemblyFullName);
                
                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create label [{name}]", e);
            }
        }

        private static Label UpdateLabel(Label control, Dictionary<string, string> state, string callingTypeFullName, string callingAssemblyFullName)
        {
            if (state.ContainsKey("FontName")) control.FontName = state["FontName"];

            if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"], "PositionAlignment");
            if (state.ContainsKey("Position")) control.SetPosition(TranslatePointI(state["Position"], "Position"));
            if (state.ContainsKey("Size")) control.Size = TranslatePointI(state["Size"], "Size");
            if (state.ContainsKey("LayerDepth")) control.LayerDepth = Convert.ToSingle(state["LayerDepth"]);
            if (state.ContainsKey("Enabled")) control.Enabled = Convert.ToBoolean(state["Enabled"]);

            if (state.ContainsKey("ContentAlignment")) control.ContentAlignment = TranslateAlignment(state["ContentAlignment"], "ContentAlignment");
            if (state.ContainsKey("Text")) control.Text = state["Text"];
            if (state.ContainsKey("GetTextFunc")) control.GetTextFunc = TranslateGetTextFunc(state["GetTextFunc"], "GetTextFunc", callingTypeFullName, callingAssemblyFullName);
            if (state.ContainsKey("TextColor")) control.TextColor = TranslateColor(state["TextColor"], "TextColor");
            if (state.ContainsKey("TextShadowColor")) control.TextShadowColor = TranslateColor(state["TextShadowColor"], "TextShadowColor");
            if (state.ContainsKey("BackgroundColor")) control.BackgroundColor = TranslateColor(state["BackgroundColor"], "BackgroundColor");
            if (state.ContainsKey("BorderColor")) control.BorderColor = TranslateColor(state["BorderColor"], "BorderColor");
            if (state.ContainsKey("Scale")) control.Scale = Convert.ToSingle(state["Scale"]);

            return control;
        }

        private static Button InstantiateButton(string name, Dictionary<string, string> state)
        {
            try
            {
                var textureNormal = state.ContainsKey("TextureNormal") ? state["TextureNormal"] : "";
                var textureActive = state.ContainsKey("TextureActive") ? state["TextureActive"] : "";
                var textureHover = state.ContainsKey("TextureHover") ? state["TextureHover"] : "";
                var textureDisabled = state.ContainsKey("TextureDisabled") ? state["TextureDisabled"] : "";

                var control = new Button(name, textureNormal, textureActive, textureHover, textureDisabled);
                control = UpdateButton(control, state);

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create button [{name}]", e);
            }
        }

        private static Button UpdateButton(Button control, Dictionary<string, string> state)
        {
            if (state.ContainsKey("TextureNormal")) control.TextureNormal = state["TextureNormal"];
            if (state.ContainsKey("TextureActive")) control.TextureActive = state["TextureActive"];
            if (state.ContainsKey("TextureHover")) control.TextureHover = state["TextureHover"];
            if (state.ContainsKey("TextureDisabled")) control.TextureDisabled = state["TextureDisabled"];

            if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"], "PositionAlignment");
            if (state.ContainsKey("Position")) control.SetPosition(TranslatePointI(state["Position"], "Position"));
            if (state.ContainsKey("Size")) control.Size = TranslatePointI(state["Size"], "Size");
            if (state.ContainsKey("LayerDepth")) control.LayerDepth = Convert.ToSingle(state["LayerDepth"]);
            if (state.ContainsKey("Enabled")) control.Enabled = Convert.ToBoolean(state["Enabled"]);

            if (state.ContainsKey("Color")) control.Color = TranslateColor(state["Color"], "Color");

            return control;
        }

        private static Frame InstantiateFrame(string name, Dictionary<string, string> state)
        {
            try
            {
                var textureName = state.ContainsKey("TextureName") ? state["TextureName"] : string.Empty;
                var topPadding = state.ContainsKey("TopPadding") ? Convert.ToInt32(state["TopPadding"]) : 0;
                var bottomPadding = state.ContainsKey("BottomPadding") ? Convert.ToInt32(state["BottomPadding"]) : 0;
                var leftPadding = state.ContainsKey("LeftPadding") ? Convert.ToInt32(state["LeftPadding"]) : 0;
                var rightPadding = state.ContainsKey("RightPadding") ? Convert.ToInt32(state["RightPadding"]) : 0;

                var control = new Frame(name, textureName, topPadding, bottomPadding, leftPadding, rightPadding);
                control = UpdateFrame(control, state);

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create frame [{name}]", e);
            }
        }

        private static Frame UpdateFrame(Frame control, Dictionary<string, string> state)
        {
            if (state.ContainsKey("TextureName")) control.TextureName = state["TextureName"];
            if (state.ContainsKey("TopPadding")) control.TopPadding = Convert.ToInt32(state["TopPadding"]);
            if (state.ContainsKey("BottomPadding")) control.BottomPadding = Convert.ToInt32(state["BottomPadding"]);
            if (state.ContainsKey("LeftPadding")) control.LeftPadding = Convert.ToInt32(state["LeftPadding"]);
            if (state.ContainsKey("RightPadding")) control.RightPadding = Convert.ToInt32(state["RightPadding"]);

            if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"], "PositionAlignment");
            if (state.ContainsKey("Position")) control.SetPosition(TranslatePointI(state["Position"], "Position"));
            if (state.ContainsKey("Size")) control.Size = TranslatePointI(state["Size"], "Size");
            if (state.ContainsKey("LayerDepth")) control.LayerDepth = Convert.ToSingle(state["LayerDepth"]);
            if (state.ContainsKey("Enabled")) control.Enabled = Convert.ToBoolean(state["Enabled"]);

            return control;
        }

        private static IControl InstantiateImage(string name, Dictionary<string, string> state)
        {
            try
            {
                var textureName = state.ContainsKey("TextureName") ? state["TextureName"] : string.Empty;

                var control = new Image(name, textureName);
                control = UpdateImage(control, state);

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create image [{name}]", e);
            }
        }

        private static Image UpdateImage(Image control, Dictionary<string, string> state)
        {
            if (state.ContainsKey("TextureName")) control.TextureName = state["TextureName"];

            if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"], "PositionAlignment");
            if (state.ContainsKey("Position")) control.SetPosition(TranslatePointI(state["Position"], "Position"));
            if (state.ContainsKey("Size")) control.Size = TranslatePointI(state["Size"], "Size");

            return control;
        }

        private static Color TranslateColor(string colorAsString, string propertyName)
        {
            try
            {
                var tempColor = System.Drawing.Color.FromName(colorAsString);
                var color = new Color(tempColor.R, tempColor.G, tempColor.B, tempColor.A);

                return color;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed To convert [{colorAsString}] for [{propertyName}] to Color.", e);
            }
        }

        private static Alignment TranslateAlignment(string alignmentAsString, string propertyName)
        {
            try
            {
                var success = Enum.TryParse(alignmentAsString, out Alignment alignment);
                if (!success) throw new Exception($"Alignment {alignmentAsString} could not be translated.");

                return alignment;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed To convert [{alignmentAsString}] for [{propertyName}] to Alignment.", e);
            }
        }

        private static ParentContainerAlignment TranslateParentContainerAlignment(string alignmentAsString, string propertyName)
        {
            try
            {
                var success = Enum.TryParse(alignmentAsString, out ParentContainerAlignment alignment);
                if (!success) throw new Exception($"ParentContainerAlignment {alignmentAsString} could not be translated.");

                return alignment;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed To convert [{alignmentAsString}] for [{propertyName}] to ParentContainerAlignment.", e);
            }
        }

        private static PointI TranslatePointI(string pointIAsString, string propertyName)
        {
            try
            {
                var x = Convert.ToInt32(pointIAsString.Split(';')[0].Trim());
                var y = Convert.ToInt32(pointIAsString.Split(';')[1].Trim());
                var pointI = new PointI(x, y);

                return pointI;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed To convert [{pointIAsString}] for [{propertyName}] to PointI.", e);
            }
        }

        private static Func<object, string> TranslateGetTextFunc(string getTextFuncAsString, string propertyName, string callingTypeFullName, string callingAssemblyFullName) // 'Game1.EventHandlers, Game1 - GetTextFunc' or 'Game1.EventHandlers.GetTextFunc' or 'GetTextFunc'
        {
            try
            {
                var firstAndLastCharactersRemoved = getTextFuncAsString.RemoveFirstAndLastCharacters(); // remove single quotes
                var split = firstAndLastCharactersRemoved.Split('-');

                if (split.Length > 1) // if 'Game1.EventHandlers, Game1 - GetTextFunc'
                {
                    var assemblyQualifiedName = split[0].Trim(); // Game1.EventHandlers, Game1
                    var methodName = split[1].Trim(); // GetTextFunc
                    var func = ObjectCreator.CreateFuncDelegate(assemblyQualifiedName, methodName);

                    return func;
                }

                split = split[0].Split('.');

                if (split.Length == 1)
                {
                    var methodName = split[0].Trim();
                    var assemblyQualifiedName = $"{callingTypeFullName}, {callingAssemblyFullName}";
                    var func = ObjectCreator.CreateFuncDelegate(assemblyQualifiedName, methodName);

                    return func;
                }

                // else: 'Game1.EventHandlers.GetTextFunc'
                if (split.Length > 1)
                {
                    var methodName = split[^1].Trim(); // GetTextFunc
                    var className = split[^2].Trim(); // EventHandlers
                    var nameSpace = firstAndLastCharactersRemoved.Replace($".{methodName}", string.Empty).Replace($".{className}", string.Empty); // Game1
                    var assemblyQualifiedName = $"{nameSpace}.{className}, {callingAssemblyFullName}"; // Game1.EventHandlers, Game1
                    var func = ObjectCreator.CreateFuncDelegate(assemblyQualifiedName, methodName);

                    return func;
                }

                throw new Exception($"Failed to convert [{getTextFuncAsString}] for property [{propertyName}] to Func.");
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to convert [{getTextFuncAsString}] for property [{propertyName}] to Func.", e);
            }
        }

        private static List<string> GetPackages(Dictionary<string, string> state)
        {
            if (!state.ContainsKey("Packages")) return new List<string>();

            var packagesList = new List<string>();
            var packagesAsString = state["Packages"];
            try
            {
                var pack = packagesAsString.RemoveFirstAndLastCharacters(); // remove square braces
                var packages = pack.Split(';');
                packagesList.AddRange(packages);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get packages for [{packagesAsString}].", e);
            }

            return packagesList;
        }

        private static string GetOnClickEventHandler(Dictionary<string, string> state)
        {
            if (!state.ContainsKey("OnClick")) return string.Empty;

            var onClickEventHandlerAsString = state["OnClick"];
            var onClickEventHandlerAsString2 = $"'Zen.GuiControls.PackagesClasses.ControlClick, Zen.GuiControls - {onClickEventHandlerAsString.RemoveFirstAndLastCharacters()}'"; // remove single quotes

            return onClickEventHandlerAsString2;
        }

        private static List<string> GetContains(Dictionary<string, string> state)
        {
            if (!state.ContainsKey("Contains")) return new List<string>();

            var containsList = new List<string>();
            var containsAsString = state["Contains"];
            try
            {
                var cont = containsAsString.RemoveFirstAndLastCharacters(); // remove square braces
                var contains = cont.Split(';');
                containsList.AddRange(contains);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get contains for [{containsAsString}].", e);
            }

            return containsList;
        }

        private static (Alignment parent, Alignment child) GetParentContainerAlignment(Dictionary<string, string> state)
        {
            if (!state.ContainsKey("ParentContainerAlignment")) return (Alignment.TopLeft, Alignment.TopLeft);

            var parentContainerAlignmentAsString = state["ParentContainerAlignment"];
            var parentContainerAlignment = TranslateParentContainerAlignment(parentContainerAlignmentAsString, "ParentContainerAlignment");

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

        private static PointI GetOffset(Dictionary<string, string> state)
        {
            if (!state.ContainsKey("Offset")) return PointI.Zero;

            var offsetAsString = state["Offset"];
            var offset = TranslatePointI(offsetAsString, "Offset");

            return offset;
        }
    }
}