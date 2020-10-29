using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.MonoGameUtilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Image : ControlWithSingleTexture
    {
        /// <summary>
        /// An amazing little image.
        /// </summary>
        /// <param name="name">Name of control.</param>
        /// <param name="textureName">Texture to use for control.</param>
        public Image(
            string name,
            string textureName = "") :
            base(textureName, name)
        {
        }

        protected override void InDraw(SpriteBatch spriteBatch)
        {
            if (Texture.HasValue())
            {
                spriteBatch.Draw(Texture, ActualDestinationRectangle, SourceRectangle, Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
            }
        }
    }
}