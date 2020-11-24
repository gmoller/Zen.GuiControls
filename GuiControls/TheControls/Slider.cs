using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Zen.GuiControls.PackagesClasses;
using Zen.Input;
using Zen.MonoGameUtilities.ExtensionMethods;
using Zen.Utilities;

namespace Zen.GuiControls.TheControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Slider : Control
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

        public List<PointI> SlidePath { get; set; }
        #endregion

        public string TextureNormal
        {
            get => Textures.ContainsKey("TextureNormal") ? Textures["TextureNormal"].TextureString : string.Empty;
            set => AddTexture("TextureNormal", new Texture("TextureNormal", value, control => true, control => Bounds));
        }

        public string TextureGripNormal
        {
            get => Textures.ContainsKey("TextureGripNormal") ? Textures["TextureGripNormal"].TextureString : string.Empty;
            set => AddTexture("TextureGripNormal", new Texture("TextureGripNormal", value, TextureGripNormalIsValid, GetDestination));
        }

        public string TextureGripHover
        {
            get => Textures.ContainsKey("TextureGripHover") ? Textures["TextureGripHover"].TextureString : string.Empty;
            set => AddTexture("TextureGripHover", new Texture("TextureGripHover", value, TextureGripHoverIsValid, GetDestination));
        }

        private bool TextureGripHoverIsValid(IControl control)
        {
            var isValid = Status == ControlStatus.MouseOver && GetDestination(control).Contains(Input.Mouse.Location) && Enabled;

            return isValid;
        }

        private bool TextureGripNormalIsValid(IControl control)
        {
            var isValid = !TextureGripHoverIsValid(control);

            return isValid;
        }

        /// <summary>
        /// An wonderful little slider.
        /// </summary>
        /// <param name="name">Name of control.</param>
        public Slider(string name) : base(name)
        {
            GripSize = PointI.Empty;
            MinimumValue = 0;
            MaximumValue = 0;
            CurrentValue = 0;
            SlidePath = new List<PointI>();
            AddPackage(new ControlDrag(UpdateSliderCurrentValue));
        }

        private Slider(Slider other) : base(other)
        {
            //TextureName = other.TextureName;
            //TextureGripNormal = other.TextureGripNormal;
            //TextureGripHover = other.TextureGripHover;
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

        private Rectangle GetDestination(IControl control)
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