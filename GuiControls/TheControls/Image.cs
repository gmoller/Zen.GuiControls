using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zen.GuiControls.TheControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Image : Control
    {
        public string TextureNormal
        {
            get => Textures.ContainsKey("TextureNormal") ? Textures["TextureNormal"].TextureString : string.Empty;
            set => AddTexture("TextureNormal", new Texture("TextureNormal", value, control => true, control => Bounds));
        }

        /// <summary>
        /// An amazing little image.
        /// </summary>
        /// <param name="name">Name of control.</param>
        public Image(string name) : base(name)
        {
        }

        private Image(Image other) : base(other)
        {
        }

        internal static IControl Create(string name, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            try
            {
                var control = new Image(name);
                control = Update(control, state, callingTypeFullName, callingAssemblyFullName);

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create image [{name}]", e);
            }
        }

        internal static Image Update(Image control, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            control.TextureNormal = state.GetAsString("TextureNormal", control.TextureNormal);
            control.AddTextures(state.GetAsListOfTextures("Textures", callingTypeFullName, callingAssemblyFullName, new List<Texture>()));

            return control;
        }

        public override IControl Clone()
        {
            return new Image(this);
        }
    }
}