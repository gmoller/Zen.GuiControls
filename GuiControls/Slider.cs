using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.Utilities;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Slider : ControlWithMultipleTextures
    {
        #region State
        public PointI GripSize { get; set; }
        public int MinimumValue { get; set; }
        public int MaximumValue { get; set; }
        
        private int _currentValue;
        public int CurrentValue
        {
            get => _currentValue;
            set => _currentValue = Math.Clamp(value, MinimumValue, MaximumValue);
        }
        #endregion

        /// <summary>
        /// An wonderful little slider.
        /// </summary>
        /// <param name="name">Name of control.</param>
        public Slider(string name) : base(name)
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

        private Slider(Slider other) : base(other)
        {
            GripSize = other.GripSize;
            MinimumValue = other.MinimumValue;
            MaximumValue = other.MaximumValue;
            CurrentValue = other.CurrentValue;
        }

        public override IControl Clone()
        {
            return new Slider(this);
        }

        protected override void InDraw(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle)
        {
            spriteBatch.Draw(texture, ActualDestinationRectangle, sourceRectangle, Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);

            var textureGripString = TextureStrings["TextureGrip"];
            var textureGrip = GetTexture2D(textureGripString);
            var rectangle = GetSourceRectangle(textureGrip, textureGripString);
            var destination = GetDestination(GripSize, CurrentValue, MinimumValue, MaximumValue, ActualDestinationRectangle);

            spriteBatch.Draw(textureGrip, destination, rectangle, Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
        }

        private static Rectangle GetDestination(PointI size, int current, int minimum, int maximum, Rectangle actualDestinationRectangle)
        {
            var ratio = current / (float)(minimum + maximum);
            var x = actualDestinationRectangle.X + actualDestinationRectangle.Width * ratio - size.X * 0.5f;
            var y = (actualDestinationRectangle.Top + actualDestinationRectangle.Bottom) * 0.5f - size.Y * 0.5f;

            var rectangle = new Rectangle((int)x, (int)y, size.X, size.Y);

            return rectangle;
        }
    }
}