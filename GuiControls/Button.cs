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
        /// 
        /// </summary>
        /// <param name="name">Name of button control.</param>
        /// <param name="textureNormal"></param>
        /// <param name="textureActive"></param>
        /// <param name="textureHover"></param>
        /// <param name="textureDisabled"></param>
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