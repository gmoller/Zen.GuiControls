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
        public static IControl CreateFromResource(string resourceName)
        {
            var spec = ReadResource(resourceName);
            var control = CreateFromSpecification(spec);

            return control;
        }

        private static string ReadResource(string resourceName)
        {
            //var assembly = Assembly.GetExecutingAssembly();
            var assembly = Assembly.GetEntryAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException($"Failed to get stream for [{resourceName}]"));
            var result = reader.ReadToEnd();

            return result;
        }

        public static IControl CreateFromSpecification(string spec)
        {
            var allTheLines = spec.SplitToLines().ToArray();
            var potentialControls = GetPotentialControls(allTheLines);

            var staging = new Dictionary<string, (IControl control, List<string> contains)>();

            foreach (var potentialControl in potentialControls)
            {
                var name = potentialControl.Key;
                var type = potentialControl.Value.controlType;
                var state = potentialControl.Value.state;

                switch (type)
                {
                    case "Label":
                        var control1 = new Label(name, state["FontName"]);
                        if (state.ContainsKey("BorderColor")) control1.BorderColor = TranslateColor(state["BorderColor"]);
                        if (state.ContainsKey("ContentAlignment")) control1.ContentAlignment = TranslateAlignment(state["ContentAlignment"]);
                        if (state.ContainsKey("Size")) control1.Size = TranslateSize(state["Size"]);
                        if (state.ContainsKey("Text")) control1.Text = state["Text"];
                        if (state.ContainsKey("TextColor")) control1.TextColor = TranslateColor(state["TextColor"]);
                        if (state.ContainsKey("TextShadowColor")) control1.TextShadowColor = TranslateColor(state["TextShadowColor"]);
                        if (state.ContainsKey("Position")) control1.SetPosition(TranslatePosition(state["Position"]));
                        var containsList1 = TranslateContains(state);
                        staging.Add(name, (control1, containsList1));
                        break;
                    case "Button":
                        var control2 = new Button(name);
                        if (state.ContainsKey("Color")) control2.Color = TranslateColor(state["Color"]);
                        if (state.ContainsKey("Size")) control2.Size = TranslateSize(state["Size"]);
                        if (state.ContainsKey("Position")) control2.SetPosition(TranslatePosition(state["Position"]));
                        var containsList2 = TranslateContains(state);
                        staging.Add(name, (control2, containsList2));
                        break;
                    case "Frame":
                        var textureName = state.ContainsKey("TextureName") ? state["TextureName"] : string.Empty;
                        var control3 = new Frame(name, textureName);
                        if (state.ContainsKey("Size")) control3.Size = TranslateSize(state["Size"]);
                        var containsList3 = TranslateContains(state);
                        staging.Add(name, (control3, containsList3));

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, $"ControlType {type} is not supported.");
                }
            }

            foreach (var control in staging.Values)
            {
                foreach (var item in control.contains)
                {
                    var childControl = (Control)staging[item].control;
                    control.control.AddControl(childControl);
                }
            }

            // return the one(s) that don't have a parent
            foreach (var control in staging.Values)
            {
                if (control.control.Parent is null)
                {
                    return control.control;
                }
            }

            return null;
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

        private static List<string> TranslateContains(Dictionary<string, string> state)
        {
            var containsList = new List<string>();
            if (state.ContainsKey("Contains"))
            {
                var cont = state["Contains"].Trim('[').Trim(']');
                var contains = cont.Split(';');
                containsList.AddRange(contains);
            }

            return containsList;
        }

        private static Dictionary<string, (string controlType, Dictionary<string, string> state)> GetPotentialControls(string[] allTheLines)
        {
            var potentialControls = new Dictionary<string, (string controlType, Dictionary<string, string> state)>();

            var controlName = string.Empty;
            var betweenSquirlies = false;
            for (var i = 0; i < allTheLines.Length; i++)
            {
                var line = allTheLines[i];
                if (line.Trim().StartsWith("{"))
                {
                    betweenSquirlies = true;
                    var controlNameAndType = allTheLines[i - 1];
                    controlName = controlNameAndType.Split(':')[0].Trim();
                    var controlType = controlNameAndType.Split(':')[1].Trim();
                    (string controlType, Dictionary<string, string> state) state = (controlType, new Dictionary<string, string>());
                    potentialControls.Add(controlName, state);
                }
                else if (line.Trim().StartsWith("}"))
                {
                    betweenSquirlies = false;
                    controlName = string.Empty;
                }
                else
                {
                    if (betweenSquirlies && line.Trim().HasValue())
                    {
                        var potentialControl = potentialControls[controlName];
                        var key = line.Split(':')[0].Trim();
                        var value = line.Split(':')[1].Trim();
                        potentialControl.state[key] = value;
                    }
                }
            }

            return potentialControls;
        }
    }
}