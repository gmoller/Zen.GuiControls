using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.MonoGameUtilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Slider : ControlWithSingleTexture
    {
        public int MinimumValue { get; set; }
        public int MaximumValue { get; set; }
        public int CurrentValue { get; set; }

        /// <summary>
        /// An wonderful little slider.
        /// </summary>
        /// <param name="name">Name of control.</param>
        /// <param name="textureName">Texture to use for control.</param>
        public Slider(
            string name,
            string textureName = "") :
            base(name, textureName)
        {
        }

        public Slider Clone()
        {
            var clone = new Slider(Name, TextureName);

            clone.MinimumValue = MinimumValue;
            clone.MaximumValue = MaximumValue;
            clone.CurrentValue = CurrentValue;

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