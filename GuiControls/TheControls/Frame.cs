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

        public string TextureName
        {
            get => Textures.ContainsKey("TextureName") ? Textures["TextureName"].TextureString : string.Empty;
            set
            {
                AddTexture("TextureName", new Texture(value, () => true, () => Bounds));
            }
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