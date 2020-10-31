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
        public static Controls CreateFromResource(string resourceName, params KeyValuePair<string, string>[] pairs)
        {
            var controls = CreateFromResource(resourceName, pairs.ToList());

            return controls;
        }

        public static Controls CreateFromResource(string resourceName, List<KeyValuePair<string, string>> pairs)
        {
            var pairsDictionary = new Dictionary<string, string>();
            foreach (var pair in pairs)
            {
                pairsDictionary.Add(pair.Key, pair.Value);
            }

            var spec = ReadResource(resourceName);
            var controls = CreateFromSpecification(spec, pairs);

            return controls;
        }

        private static string ReadResource(string resourceName)
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly is null) throw new InvalidOperationException($"Failed to get stream for [{resourceName}]");

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException($"Failed to get stream for [{resourceName}]"));
            var result = reader.ReadToEnd();

            return result;
        }

        public static Controls CreateFromSpecification(string spec, List<KeyValuePair<string, string>> pairs)
        {
            var allTheLines = spec.SplitToLines().ToArray();
            var pairsDictionary = pairs.ToDictionary(pair => pair.Key, pair => pair.Value);

            var potentialControls = GetPotentialControls(allTheLines, pairsDictionary);

            var staging = new Dictionary<string, (IControl control, List<string> contains)>();

            foreach (var potentialControl in potentialControls)
            {
                var name = potentialControl.Key;
                var type = potentialControl.Value.controlType;
                var state = potentialControl.Value.state;

                var control = InstantiateControl(name, type, state);

                var packagesList = TranslatePackages(state);
                control.AddPackages(packagesList);

                var containsList = TranslateContains(state);
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

        private static IControl InstantiateControl(string name, string type, Dictionary<string, string> state)
        {
            IControl control;
            switch (type)
            {
                case "Label":
                    control = InstantiateLabel(name, state);
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

        private static Label InstantiateLabel(string name, Dictionary<string, string> state)
        {
            var control = new Label(name, state["FontName"]);
            if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"]);
            if (state.ContainsKey("Position")) control.SetPosition(TranslatePosition(state["Position"]));
            if (state.ContainsKey("Size")) control.Size = TranslateSize(state["Size"]);
            if (state.ContainsKey("LayerDepth")) control.LayerDepth = Convert.ToSingle(state["LayerDepth"]);
            if (state.ContainsKey("Enabled")) control.Enabled = Convert.ToBoolean(state["Enabled"]);

            if (state.ContainsKey("ContentAlignment")) control.ContentAlignment = TranslateAlignment(state["ContentAlignment"]);
            if (state.ContainsKey("Text")) control.Text = state["Text"];
            if (state.ContainsKey("GetTextFunc")) control.GetTextFunc = TranslateGetTextFunc(state["GetTextFunc"]);
            if (state.ContainsKey("TextColor")) control.TextColor = TranslateColor(state["TextColor"]);
            if (state.ContainsKey("TextShadowColor")) control.TextShadowColor = TranslateColor(state["TextShadowColor"]);
            if (state.ContainsKey("BackgroundColor")) control.BackgroundColor = TranslateColor(state["BackgroundColor"]);
            if (state.ContainsKey("BorderColor")) control.BorderColor = TranslateColor(state["BorderColor"]);
            if (state.ContainsKey("Scale")) control.Scale = Convert.ToSingle(state["Scale"]);

            return control;
        }

        private static Button InstantiateButton(string name, Dictionary<string, string> state)
        {
            var textureNormal = state.ContainsKey("TextureNormal") ? state["TextureNormal"] : "";
            var textureActive = state.ContainsKey("TextureActive") ? state["TextureActive"] : "";
            var textureHover = state.ContainsKey("TextureHover") ? state["TextureHover"] : "";
            var textureDisabled = state.ContainsKey("TextureDisabled") ? state["TextureDisabled"] : "";

            var control = new Button(name, textureNormal, textureActive, textureHover, textureDisabled);
            if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"]);
            if (state.ContainsKey("Position")) control.SetPosition(TranslatePosition(state["Position"]));
            if (state.ContainsKey("Size")) control.Size = TranslateSize(state["Size"]);
            if (state.ContainsKey("LayerDepth")) control.LayerDepth = Convert.ToSingle(state["LayerDepth"]);
            if (state.ContainsKey("Enabled")) control.Enabled = Convert.ToBoolean(state["Enabled"]);

            if (state.ContainsKey("Color")) control.Color = TranslateColor(state["Color"]);

            return control;
        }

        private static Frame InstantiateFrame(string name, Dictionary<string, string> state)
        {
            var textureName = state.ContainsKey("TextureName") ? state["TextureName"] : string.Empty;
            var topPadding = state.ContainsKey("TopPadding") ? Convert.ToInt32(state["TopPadding"]) : 0;
            var bottomPadding = state.ContainsKey("BottomPadding") ? Convert.ToInt32(state["BottomPadding"]) : 0;
            var leftPadding = state.ContainsKey("LeftPadding") ? Convert.ToInt32(state["LeftPadding"]) : 0;
            var rightPadding = state.ContainsKey("RightPadding") ? Convert.ToInt32(state["RightPadding"]) : 0;

            var control = new Frame(name, textureName, topPadding, bottomPadding, leftPadding, rightPadding);
            if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"]);
            if (state.ContainsKey("Position")) control.SetPosition(TranslatePosition(state["Position"]));
            if (state.ContainsKey("Size")) control.Size = TranslateSize(state["Size"]);
            if (state.ContainsKey("LayerDepth")) control.LayerDepth = Convert.ToSingle(state["LayerDepth"]);
            if (state.ContainsKey("Enabled")) control.Enabled = Convert.ToBoolean(state["Enabled"]);

            return control;
        }

        private static IControl InstantiateImage(string name, Dictionary<string, string> state)
        {
            var textureName = state.ContainsKey("TextureName") ? state["TextureName"] : string.Empty;

            var control = new Image(name, textureName);
            if (state.ContainsKey("PositionAlignment")) control.PositionAlignment = TranslateAlignment(state["PositionAlignment"]);
            if (state.ContainsKey("Position")) control.SetPosition(TranslatePosition(state["Position"]));
            if (state.ContainsKey("Size")) control.Size = TranslateSize(state["Size"]);

            return control;
        }

        private static Color TranslateColor(string colorAsString)
        {
            var tempColor = System.Drawing.Color.FromName(colorAsString);
            var color = new Color(tempColor.R, tempColor.G, tempColor.B, tempColor.A);

            return color;
        }

        private static Alignment TranslateAlignment(string alignmentAsString)
        {
            var success = Enum.TryParse(alignmentAsString, out Alignment alignment);
            if (!success) throw new Exception($"Alignment {alignmentAsString} could not be translated.");

            return alignment;
        }

        private static PointI TranslateSize(string sizeAsString)
        {
            var x = Convert.ToInt32(sizeAsString.Split(';')[0].Trim());
            var y = Convert.ToInt32(sizeAsString.Split(';')[1].Trim());
            var size = new PointI(x, y);

            return size;
        }

        private static PointI TranslatePosition(string positionAsString)
        {
            var x = Convert.ToInt32(positionAsString.Split(';')[0].Trim());
            var y = Convert.ToInt32(positionAsString.Split(';')[1].Trim());
            var size = new PointI(x, y);

            return size;
        }

        private static Func<string> TranslateGetTextFunc(string getTextFuncAsString) // 'Game1.EventHandlers, Game1 - GetTextFunc' or 'Game1.EventHandlers.GetTextFunc'
        {
            var firstAndLastCharactersRemoved = getTextFuncAsString.RemoveFirstAndLastCharacters();
            var split = firstAndLastCharactersRemoved.Split('-');

            if (split.Length > 1) // if 'Game1.EventHandlers, Game1 - GetTextFunc'
            {
                var assemblyQualifiedName = split[0].Trim(); // Game1.EventHandlers, Game1
                var methodName = split[1].Trim(); // GetTextFunc
                var func = ObjectCreator.CreateFuncDelegate(assemblyQualifiedName, methodName);

                return func;
            }
            // else: 'Game1.EventHandlers.GetTextFunc'
            split = split[0].Split('.');

            if (split.Length > 1)
            {
                var methodName = split[^1].Trim(); // GetTextFunc
                var className = split[^2].Trim(); // EventHandlers
                var nameSpace = firstAndLastCharactersRemoved.Replace($".{methodName}", string.Empty).Replace($".{className}", string.Empty); // Game1
                var assemblyQualifiedName = $"{nameSpace}.{className}, {nameSpace}"; // Game1.EventHandlers, Game1
                var func = ObjectCreator.CreateFuncDelegate(assemblyQualifiedName, methodName);

                return func;
            }
            else
            {
                return null;
            }
        }

        private static List<string> TranslateContains(Dictionary<string, string> state)
        {
            var containsList = new List<string>();
            if (state.ContainsKey("Contains"))
            {
                var cont = state["Contains"].RemoveFirstAndLastCharacters();
                var contains = cont.Split(';');
                containsList.AddRange(contains);
            }

            return containsList;
        }

        private static List<string> TranslatePackages(Dictionary<string, string> state)
        {
            var packagesList = new List<string>();
            if (state.ContainsKey("Packages"))
            {
                var pack = state["Packages"].RemoveFirstAndLastCharacters();
                var packages = pack.Split(';');
                packagesList.AddRange(packages);
            }

            return packagesList;
        }

        private static Dictionary<string, (string controlType, Dictionary<string, string> state)> GetPotentialControls(string[] allTheLines, Dictionary<string, string> pairs)
        {
            var potentialControls = new Dictionary<string, (string controlType, Dictionary<string, string> state)>();

            var currentControlName = string.Empty;
            var betweenSquirlies = false;
            for (var i = 0; i < allTheLines.Length; i++)
            {
                var line = allTheLines[i];
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
                    if (betweenSquirlies && line.Trim().HasValue())
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
    }
}