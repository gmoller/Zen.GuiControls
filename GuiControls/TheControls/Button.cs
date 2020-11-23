using System.Diagnostics;

namespace Zen.GuiControls.TheControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Button : Control
    {
        public string TextureNormal
        {
            get => Textures.ContainsKey("TextureNormal") ? Textures["TextureNormal"].TextureString : string.Empty;
            set
            {
                AddTexture("TextureNormal", new Texture(value, () => Status == ControlStatus.None && Enabled, () => Bounds));
            }
        }

        public string TextureActive
        {
            get => Textures.ContainsKey("TextureActive") ? Textures["TextureActive"].TextureString : string.Empty;
            set
            {
                AddTexture("TextureActive", new Texture(value, () => Status == ControlStatus.Active && Enabled, () => Bounds));
            }
        }

        public string TextureHover
        {
            get => Textures.ContainsKey("TextureHover") ? Textures["TextureHover"].TextureString : string.Empty;
            set
            {
                AddTexture("TextureHover", new Texture(value, () => Status == ControlStatus.MouseOver && Enabled, () => Bounds));
            }
        }

        public string TextureDisabled
        {
            get => Textures.ContainsKey("TextureDisabled") ? Textures["TextureDisabled"].TextureString : string.Empty;
            set
            {
                AddTexture("TextureDisabled", new Texture(value, () => !Enabled, () => Bounds));
            }
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

        public override IControl Clone()
        {
            return new Button(this);
        }
    }
}