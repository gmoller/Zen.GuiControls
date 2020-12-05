using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zen.GuiControls.TheControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Button : Control
    {
        public string TextureNormal
        {
            get => Textures.ContainsKey("TextureNormal") ? Textures["TextureNormal"].TextureString : string.Empty;
            set => AddTexture("TextureNormal", new Texture("TextureNormal", value, TextureNormalIsValid, control => Bounds));
        }

        public string TextureActive
        {
            get => Textures.ContainsKey("TextureActive") ? Textures["TextureActive"].TextureString : string.Empty;
            set => AddTexture("TextureActive", new Texture("TextureActive", value, ControlHelper.TextureActiveIsValid, control => Bounds));
        }

        public string TextureHover
        {
            get => Textures.ContainsKey("TextureHover") ? Textures["TextureHover"].TextureString : string.Empty;
            set => AddTexture("TextureHover", new Texture("TextureHover", value, ControlHelper.TextureHoverIsValid, control => Bounds));
        }

        public string TextureDisabled
        {
            get => Textures.ContainsKey("TextureDisabled") ? Textures["TextureDisabled"].TextureString : string.Empty;
            set => AddTexture("TextureDisabled", new Texture("TextureDisabled", value, ControlHelper.TextureDisabledIsValid, control => Bounds));
        }

        private static bool TextureNormalIsValid(IControl control)
        {
            var isValid = control.Status == ControlStatus.None;

            return isValid;
        }

        /// <summary>
        /// A lovely little button.
        /// </summary>
        /// <param name="name">Name of control.</param>
        public Button(string name) : base(name)
        {
        }

        private Button(Button other) : base(other)
        {
        }

        internal static Button Create(string name, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            try
            {
                var control = new Button(name);
                control = Update(control, state, callingTypeFullName, callingAssemblyFullName);

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create button [{name}]", e);
            }
        }

        internal static Button Update(Button control, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            control.TextureActive = state.GetAsString("TextureActive", control.TextureActive);
            control.TextureHover = state.GetAsString("TextureHover", control.TextureHover);
            control.TextureNormal = state.GetAsString("TextureNormal", control.TextureNormal);
            control.TextureDisabled = state.GetAsString("TextureDisabled", control.TextureDisabled);
            control.AddTextures(state.GetAsListOfTextures("Textures", callingTypeFullName, callingAssemblyFullName, new List<Texture>()));

            return control;
        }

        public override IControl Clone()
        {
            return new Button(this);
        }
    }
}