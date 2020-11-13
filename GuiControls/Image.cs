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
            base(name, textureName)
        {
        }

        public Image Clone()
        {
            var clone = new Image(Name, TextureName);
            clone.Status = Status;
            clone.Enabled = Enabled;
            clone.PositionAlignment = PositionAlignment;
            clone.SetPosition(GetPosition());
            clone.Size = Size;
            clone.Owner = Owner;
            clone.Parent = Parent;
            clone.LayerDepth = LayerDepth;

            return clone;
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