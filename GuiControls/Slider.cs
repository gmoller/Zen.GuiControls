using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.Utilities;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Slider : ControlWithSingleTexture
    {
        #region State
        public PointI GripSize { get; set; }
        public string TextureGrip { get; set; }
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
        }

        private Slider(Slider other) : base(other)
        {
            GripSize = other.GripSize;
            TextureGrip = other.TextureGrip;
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

            var textureGrip = GetTexture2D(TextureGrip);
            var rectangle = GetSourceRectangle(textureGrip, TextureGrip);
            var destination = GetDestination(GripSize, CurrentValue, MinimumValue, MaximumValue, ActualDestinationRectangle);

            spriteBatch.Draw(textureGrip, destination, rectangle, Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
        }

        private static Rectangle GetSourceRectangle(Texture2D texture, string textureName)
        {
            Rectangle rectangle;
            var textureAtlas = ControlHelper.GetTextureAtlas(textureName);
            if (textureAtlas.HasValue())
            {
                var atlas = AssetsManager.Instance.GetAtlas(textureAtlas);
                var textureName2 = ControlHelper.GetTextureName(textureName);
                var f = atlas.Frames[textureName2];
                rectangle = new Rectangle(f.X, f.Y, f.Width, f.Height);
            }
            else
            {
                rectangle = texture.Bounds;
            }

            return rectangle;
        }

        private static Texture2D GetTexture2D(string textureName)
        {
            var textureAtlas = ControlHelper.GetTextureAtlas(textureName);
            var texture = AssetsManager.Instance.GetTexture(textureAtlas.HasValue() ? textureAtlas : textureName);

            return texture;
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