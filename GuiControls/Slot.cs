using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.MonoGameUtilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Slot : ControlWithSingleTexture
    {
        /// <summary>
        /// A satisfying little slot.
        /// </summary>
        /// <param name="name">Name of control.</param>
        /// <param name="textureName">Texture to use for control.</param>
        public Slot(
            string name,
            string textureName) :
            base(name, textureName)
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