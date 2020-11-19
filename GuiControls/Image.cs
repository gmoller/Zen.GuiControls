using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.MonoGameUtilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Image : ControlWithMultipleTextures
    {
        /// <summary>
        /// An amazing little image.
        /// </summary>
        /// <param name="name">Name of control.</param>
        public Image(string name) : base(name)
        {
            TextureStringPicker = new Dictionary<string, string>
            {
                {"Active-True", "TextureName"},
                {"Active-False", "TextureName"},
                {"MouseOver-True", "TextureName"},
                {"MouseOver-False", "TextureName"},
                {"None-True", "TextureName"},
                {"None-False", "TextureName"}
            };
        }

        private Image(Image other) : base(other)
        {
        }

        public override IControl Clone()
        {
            return new Image(this);
        }

        protected override void InDraw(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle)
        {
            if (texture.HasValue())
            {
                spriteBatch.Draw(texture, ActualDestinationRectangle, sourceRectangle, Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
            }
        }
    }
}