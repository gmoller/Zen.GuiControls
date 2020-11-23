using System.Diagnostics;

namespace Zen.GuiControls.TheControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Image : Control
    {
        public string TextureName
        {
            get => Textures.ContainsKey("TextureName") ? Textures["TextureName"].TextureString : string.Empty;
            set
            {
                AddTexture("TextureName", new Texture(value, () => true, () => Bounds));
            }
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

        public override IControl Clone()
        {
            return new Image(this);
        }
    }
}