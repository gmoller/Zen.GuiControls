using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Image : ControlWithSingleTexture
    {
        /// <summary>
        /// Use this constructor if Image is to be used as a child of another control.
        /// When a control is a child of another control, it's position will be relative
        /// to the parent control. Therefore there is no need to pass in a position.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <param name="textureName"></param>
        public Image(
            string name,
            Vector2 size,
            string textureName = null) :
            this(
                Vector2.Zero,
                Alignment.TopLeft,
                size,
                textureName,
                name)
        {
        }

        /// <summary>
        /// Use this constructor if Image is expected to be stand alone (have no parent).
        /// </summary>
        /// <param name="position"></param>
        /// <param name="positionAlignment"></param>
        /// <param name="size"></param>
        /// <param name="textureName"></param>
        /// <param name="name"></param>
        public Image(
            Vector2 position,
            Alignment positionAlignment,
            Vector2 size,
            string textureName,
            string name = null) :
            base(textureName, name)
        {
        }

        protected override void InDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, ActualDestinationRectangle, SourceRectangle, Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
        }
    }
}