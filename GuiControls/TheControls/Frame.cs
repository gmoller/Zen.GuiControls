using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zen.GuiControls.TheControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Frame : Control
    {
        #region State
        /// <summary>
        /// Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)
        /// </summary>
        public int BorderSizeTop { get; set; }
        /// <summary>
        /// Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)
        /// </summary>
        public int BorderSizeBottom { get; set; }
        /// <summary>
        /// Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)
        /// </summary>
        public int BorderSizeLeft { get; set; }
        /// <summary>
        /// Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)
        /// </summary>
        public int BorderSizeRight { get; set; }
        #endregion

        public string TextureNormal
        {
            get => Textures.ContainsKey("TextureNormal") ? Textures["TextureNormal"].TextureString : string.Empty;
            set => AddTexture("TextureNormal", new Texture("TextureNormal", value, ControlHelper.TextureNormalIsValid, control => Bounds));
        }

        /// <summary>
        /// A pretty little frame.
        /// </summary>
        /// <param name="name">Name of frame control.</param>
        public Frame(string name) : base(name)
        {
            BorderSizeTop = 0;
            BorderSizeBottom = 0;
            BorderSizeLeft = 0;
            BorderSizeRight = 0;
        }

        private Frame(Frame other) : base(other)
        {
            BorderSizeTop = other.BorderSizeTop;
            BorderSizeBottom = other.BorderSizeBottom;
            BorderSizeLeft = other.BorderSizeLeft;
            BorderSizeRight = other.BorderSizeRight;
        }

        internal static Frame Create(string name, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            try
            {
                var control = new Frame(name);
                control = Update(control, state, callingTypeFullName, callingAssemblyFullName);

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create frame [{name}]", e);
            }
        }

        internal static Frame Update(Frame control, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            control.TextureNormal = state.GetAsString("TextureNormal", control.TextureNormal);
            control.BorderSizeTop = state.GetAsInt32("BorderSize", control.BorderSizeTop);
            control.BorderSizeBottom = state.GetAsInt32("BorderSize", control.BorderSizeBottom);
            control.BorderSizeLeft = state.GetAsInt32("BorderSize", control.BorderSizeLeft);
            control.BorderSizeRight = state.GetAsInt32("BorderSize", control.BorderSizeRight);
            control.BorderSizeTop = state.GetAsInt32("BorderSizeTop", control.BorderSizeTop);
            control.BorderSizeBottom = state.GetAsInt32("BorderSizeBottom", control.BorderSizeBottom);
            control.BorderSizeLeft = state.GetAsInt32("BorderSizeLeft", control.BorderSizeLeft);
            control.BorderSizeRight = state.GetAsInt32("BorderSizeRight", control.BorderSizeRight);
            control.AddTextures(state.GetAsListOfTextures("Textures", callingTypeFullName, callingAssemblyFullName, new List<Texture>()));

            return control;
        }

        public override IControl Clone()
        {
            return new Frame(this);
        }

        protected override void DrawSingleTexture(SpriteBatch spriteBatch, Texture item, Texture2D texture)
        {
            var sourceRectangles = ControlHelper.GetSourceRectangles(texture, item.TextureString, BorderSizeTop, BorderSizeBottom, BorderSizeLeft, BorderSizeRight);
            var destinationRectangles = ControlHelper.Get9SliceScaleRectangles(new Rectangle(TopLeft.X, TopLeft.Y, Size.X, Size.Y), BorderSizeTop, BorderSizeBottom, BorderSizeLeft, BorderSizeRight);
            for (var i = 0; i < sourceRectangles.Length; i++)
            {
                spriteBatch.Draw(texture, destinationRectangles[i], sourceRectangles[i], Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
            }
        }
    }
}