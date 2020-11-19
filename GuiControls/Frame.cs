using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Frame : ControlWithMultipleTextures
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

        /// <summary>
        /// A pretty little frame.
        /// </summary>
        /// <param name="name">Name of frame control.</param>
        public Frame(string name) : base(name)
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

        private Frame(Frame other) : base(other)
        {
            BorderSizeTop = other.BorderSizeTop;
            BorderSizeBottom = other.BorderSizeBottom;
            BorderSizeLeft = other.BorderSizeLeft;
            BorderSizeRight = other.BorderSizeRight;
        }

        public override IControl Clone()
        {
            return new Frame(this);
        }

        protected override void InDraw(SpriteBatch spriteBatch)
        {
            var textureNameString = TextureStrings["TextureName"];
            if (!textureNameString.HasValue()) return;

            var texture = GetTexture2D(textureNameString);
            var sourceRectangles = GetSourceRectangles(texture, textureNameString, BorderSizeTop, BorderSizeBottom, BorderSizeLeft, BorderSizeRight);

            var destinationRectangles = Get9SliceScaleRectangles(new Rectangle(TopLeft.X, TopLeft.Y, Size.X, Size.Y), BorderSizeTop, BorderSizeBottom, BorderSizeLeft, BorderSizeRight);
            for (var i = 0; i < sourceRectangles.Length; i++)
            {
                spriteBatch.Draw(texture, destinationRectangles[i], sourceRectangles[i], Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
            }
        }
    }
}