using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.MonoGameUtilities;
using Zen.MonoGameUtilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Button : ControlWithMultipleTextures
    {
        /// <summary>
        /// A lovely little button.
        /// </summary>
        /// <param name="name">Name of control.</param>
        /// <param name="textureNormal">Texture to use for control.</param>
        /// <param name="textureActive">Texture to use when button is active.</param>
        /// <param name="textureHover">Texture to use when button is hovered over.</param>
        /// <param name="textureDisabled">Texture to use when button is disabled.</param>
        public Button(
            string name,
            string textureNormal = "",
            string textureActive = "",
            string textureHover = "",
            string textureDisabled = "") :
            base(
                name,
                textureNormal,
                textureNormal,
                textureActive,
                textureHover,
                textureDisabled)
        {
        }

        public Button Clone()
        {
            var clone = new Button(Name, TextureNormal, TextureActive, TextureHover, TextureDisabled);
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
            else
            {
                var col = Status switch
                {
                    ControlStatus.MouseOver => Color.White,
                    ControlStatus.Active => Color.Red,
                    _ => Color
                };

                spriteBatch.DrawRectangle(ActualDestinationRectangle, col, 2.0f);
            }
        }
    }
}