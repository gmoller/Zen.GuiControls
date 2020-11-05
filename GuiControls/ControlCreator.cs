using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Zen.Utilities;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    public static class ControlCreator
    {
        public static Controls CreateFromFile(string path, List<KeyValuePair<string, string>> pairs)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var spec = File.ReadAllText(path);
            var controls = CreateFromSpecification(spec, pairs, 2);

            return controls;
        }

        public static Controls CreateFromResource(string resourceName, List<KeyValuePair<string, string>> pairs)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var spec = ReadResource(resourceName, callingAssembly);
            var controls = CreateFromSpecification(spec, pairs, 2);

            return controls;
        }

        private static string ReadResource(string resourceName, Assembly callingAssembly)
        {
            using var stream = callingAssembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException($"Failed to get stream for [{resourceName}] from Assembly [{callingAssembly.FullName}]"));
            var result = reader.ReadToEnd();

            return result;
        }

        public static Controls CreateFromSpecification(string spec, List<KeyValuePair<string, string>> pairs, int level = 1)
        {
            // determine calling type
            var callerType = ObjectCreator.GetCallerType(level);

            var allTheLines = spec.SplitToLines().ToArray();
            var pairsDictionary = pairs.ToDictionary(pair => pair.Key, pair => pair.Value);

            var potentialControls = GetPotentialControls(allTheLines, pairsDictionary);

            var staging = new Dictionary<string, (IControl control, List<string> contains)>();
            foreach (var potentialControl in potentialControls)
            {
                var name = potentialControl.Key;
                var type = potentialControl.Value.controlType;
                var state = potentialControl.Value.state;

                var control = InstantiateControl(name, type, state, callerType.callingTypeFullName, callerType.callingAssemblyFullName);

                var packagesList = GetPackages(state);
                var onClickEventHandler = GetOnClickEventHandler(state);
                if (onClickEventHandler.HasValue())
                {
                    packagesList.Add(onClickEventHandler);
                }

                control.AddPackages(packagesList, callerType.callingTypeFullName, callerType.callingAssemblyFullName);

                var containsList = GetContains(state);
                staging.Add(name, (control, containsList));
            }

            // handle contains
            foreach (var control in staging.Values)
            {
                foreach (var item in control.contains)
                {
                    var childControl = (Control)staging[item].control;
                    control.control.AddControl(childControl);
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

        private static IControl InstantiateControl(string name, string type, Dictionary<string, string> state, string callingTypeFullName, string callingAssemblyFullName)
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
                    throw new ArgumentOutOfRangeException(nameof(type), type, $"ControlType {type} is not supported.");
            }

            return control;
        }

        private static Label InstantiateLabel(string name, Dictionary<string, string> state, string callingTypeFullName, string callingAssemblyFullName)
        {
            try
            {
                var control = new Label(name, state["FontName"]);
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
            catch (Exception e)
            {
                throw new Exception($"Failed to create label [{name}]", e);
            }
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
                if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"], "PositionAlignment");
                if (state.ContainsKey("Position")) control.SetPosition(TranslatePointI(state["Position"], "Position"));
                if (state.ContainsKey("Size")) control.Size = TranslatePointI(state["Size"], "Size");
                if (state.ContainsKey("LayerDepth")) control.LayerDepth = Convert.ToSingle(state["LayerDepth"]);
                if (state.ContainsKey("Enabled")) control.Enabled = Convert.ToBoolean(state["Enabled"]);

                if (state.ContainsKey("Color")) control.Color = TranslateColor(state["Color"], "Color");

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create button [{name}]", e);
            }
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
                if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"], "PositionAlignment");
                if (state.ContainsKey("Position")) control.SetPosition(TranslatePointI(state["Position"], "Position"));
                if (state.ContainsKey("Size")) control.Size = TranslatePointI(state["Size"], "Size");
                if (state.ContainsKey("LayerDepth")) control.LayerDepth = Convert.ToSingle(state["LayerDepth"]);
                if (state.ContainsKey("Enabled")) control.Enabled = Convert.ToBoolean(state["Enabled"]);

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create frame [{name}]", e);
            }
        }

        private static IControl InstantiateImage(string name, Dictionary<string, string> state)
        {
            try
            {
                var textureName = state.ContainsKey("TextureName") ? state["TextureName"] : string.Empty;

                var control = new Image(name, textureName);
                if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"], "PositionAlignment");
                if (state.ContainsKey("Position")) control.SetPosition(TranslatePointI(state["Position"], "Position"));
                if (state.ContainsKey("Size")) control.Size = TranslatePointI(state["Size"], "Size");

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create image [{name}]", e);
            }
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
    }
}