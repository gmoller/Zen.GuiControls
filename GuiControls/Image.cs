using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.MonoGameUtilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Image : ControlWithMultipleTextures
    {
        private string _textureName;

        public string TextureName
        {
            get => _textureName;
            set
            {
                _textureName = value;
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

        protected override void InDraw(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, Rectangle destinationRectangle)
        {
            if (texture.HasValue())
            {
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
            }
        }
    }
}