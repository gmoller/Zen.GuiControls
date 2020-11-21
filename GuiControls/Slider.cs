using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.GuiControls.PackagesClasses;
using Zen.Input;
using Zen.MonoGameUtilities.ExtensionMethods;
using Zen.Utilities;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Slider : ControlWithMultipleTextures
    {
        #region State
        private string _textureName;
        public string TextureName
        {
            get => _textureName;
            set
            {
                _textureName = value;
                AddTexture("TextureName", new Texture(value, () => true, () => Bounds));
            }
        }

        private string _textureGripNormal;
        public string TextureGripNormal
        {
            get => _textureGripNormal;
            set
            {
                _textureGripNormal = value;
                AddTexture("TextureGripNormal", new Texture(value, TextureGripNormalIsValid, GetDestination));
            }
        }

        private string _textureGripHover;
        public string TextureGripHover
        {
            get => _textureGripHover;
            set
            {
                _textureGripHover = value;
                AddTexture("TextureGripHover", new Texture(value, TextureGripHoverIsValid, GetDestination));
            }
        }

        private bool TextureGripHoverIsValid()
        {
            var isValid = Status == ControlStatus.MouseOver && GetDestination().Contains(Input.Mouse.Location) && Enabled;

            return isValid;
        }

        private bool TextureGripNormalIsValid()
        {
            var isValid = !TextureGripHoverIsValid();

            return isValid;
        }

        public PointI GripSize { get; set; }
        public int MinimumValue { get; set; }
        public int MaximumValue { get; set; }
        
        private int _currentValue;
        public int CurrentValue
        {
            get => _currentValue;
            set => _currentValue = Math.Clamp(value, MinimumValue, MaximumValue);
        }

        public List<PointI> SlidePath { get; set; }
        #endregion

        /// <summary>
        /// An wonderful little slider.
        /// </summary>
        /// <param name="name">Name of control.</param>
        public Slider(string name) : base(name)
        {
            AddPackage(new ControlDrag(UpdateSliderCurrentValue));
        }

        private Slider(Slider other) : base(other)
        {
            TextureName = other.TextureName;
            TextureGripNormal = other.TextureGripNormal;
            TextureGripHover = other.TextureGripHover;
            GripSize = other.GripSize;
            MinimumValue = other.MinimumValue;
            MaximumValue = other.MaximumValue;
            CurrentValue = other.CurrentValue;
            SlidePath = other.SlidePath;
        }

        public override IControl Clone()
        {
            return new Slider(this);
        }

        protected override void InDraw(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, Rectangle destinationRectangle)
        {
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
        }

        private Rectangle GetDestination()
        {
            var ratio = CurrentValue / (float)(MinimumValue + MaximumValue);
            var p = Bounds.Location.ToPointI() + PointI.Lerp(SlidePath[0], SlidePath[1], ratio);
            var rectangle = new Rectangle((int)(p.X - GripSize.X * 0.5f), (int)(p.Y - GripSize.Y * 0.5f), GripSize.X, GripSize.Y);

            return rectangle;
        }

        private static void UpdateSliderCurrentValue(object sender, EventArgs args)
        {
            var slr = (Slider)sender;
            var mouseEventArgs = (MouseEventArgs)args;

            var x = (float)mouseEventArgs.Mouse.Location.X;
            //var y = (float)mouseEventArgs.Mouse.Location.Y;

            var ratioX = (x - slr.SlidePath[0].X) / (slr.SlidePath[1].X - slr.SlidePath[0].X);
            //var ratioY = (y - slr.SlidePath[0].Y) / (slr.SlidePath[1].Y - slr.SlidePath[0].Y); // divide by zero!
            var currentValue = ratioX * (slr.MinimumValue + slr.MaximumValue);

            slr.CurrentValue = (int)currentValue;
        }
    }
}