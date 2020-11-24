using System.Diagnostics;

namespace Zen.GuiControls.TheControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Button : Control
    {
        public string TextureNormal
        {
            get => Textures.ContainsKey("TextureNormal") ? Textures["TextureNormal"].TextureString : string.Empty;
            set => AddTexture("TextureNormal", new Texture("TextureNormal", value, control => Status == ControlStatus.None && Enabled, control => Bounds));
        }

        public string TextureActive
        {
            get => Textures.ContainsKey("TextureActive") ? Textures["TextureActive"].TextureString : string.Empty;
            set => AddTexture("TextureActive", new Texture("TextureActive", value, control => Status == ControlStatus.Active && Enabled, control => Bounds));
        }

        public string TextureHover
        {
            get => Textures.ContainsKey("TextureHover") ? Textures["TextureHover"].TextureString : string.Empty;
            set => AddTexture("TextureHover", new Texture("TextureHover", value, control => Status == ControlStatus.MouseOver && Enabled, control => Bounds));
        }

        public string TextureDisabled
        {
            get => Textures.ContainsKey("TextureDisabled") ? Textures["TextureDisabled"].TextureString : string.Empty;
            set => AddTexture("TextureDisabled", new Texture("TextureDisabled", value, control => !Enabled, control => Bounds));
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