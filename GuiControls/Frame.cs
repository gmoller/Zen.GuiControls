using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Frame : ControlWithSingleTexture
    {
        #region State
        /// <summary>
        /// Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)
        /// </summary>
        public int TopPadding { get; set; }
        /// <summary>
        /// Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)
        /// </summary>
        public int BottomPadding { get; set; }
        /// <summary>
        /// Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)
        /// </summary>
        public int LeftPadding { get; set; }
        /// <summary>
        /// Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)
        /// </summary>
        public int RightPadding { get; set; }
        #endregion

        /// <summary>
        /// Use this constructor if Frame is expected to be stand alone (have no parent).
        /// </summary>
        /// <param name="name">Name of frame control.</param>
        public Frame(string name) : base(name)
        {
        }

        private Frame(Frame other) : base(other)
        {
            TopPadding = other.TopPadding;
            BottomPadding = other.BottomPadding;
            LeftPadding = other.LeftPadding;
            RightPadding = other.RightPadding;
        }

        public override IControl Clone()
        {
            return new Frame(this);
        }

        protected override void InDraw(SpriteBatch spriteBatch)
        {
            if (TextureName.HasValue())
            {
                Rectangle[] sourcePatches;
                Texture2D texture;
                var textureAtlas = ControlHelper.GetTextureAtlas(TextureName);
                if (textureAtlas.HasValue())
                {
                    var atlas = AssetsManager.Instance.GetAtlas(textureAtlas);
                    texture = AssetsManager.Instance.GetTexture(textureAtlas);

                    var textureName = ControlHelper.GetTextureName(TextureName);
                    var frame = atlas.Frames[textureName];

                    sourcePatches = CreatePatches(frame.ToRectangle(), TopPadding, BottomPadding, LeftPadding, RightPadding);
                }
                else
                {
                    texture = AssetsManager.Instance.GetTexture(TextureName);
                    var frame = texture.Bounds;
                    sourcePatches = CreatePatches(frame, TopPadding, BottomPadding, LeftPadding, RightPadding);
                }

                var destinationPatches = CreatePatches(new Rectangle(TopLeft.X, TopLeft.Y, Size.X, Size.Y), TopPadding, BottomPadding, LeftPadding, RightPadding);
                for (var i = 0; i < sourcePatches.Length; ++i)
                {
                    spriteBatch.Draw(texture, destinationPatches[i], sourcePatches[i], Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
                }
            }
        }

        private Rectangle[] CreatePatches(Rectangle rectangle, int topPadding, int bottomPadding, int leftPadding, int rightPadding)
        {
            var x = rectangle.X;
            var y = rectangle.Y;
            var w = rectangle.Width;
            var h = rectangle.Height;
            var middleWidth = w - leftPadding - rightPadding;
            var middleHeight = h - topPadding - bottomPadding;
            var bottomY = y + h - bottomPadding;
            var rightX = x + w - rightPadding;
            var leftX = x + leftPadding;
            var topY = y + topPadding;

            var patches = new[]
            {
                new Rectangle(x,      y,        leftPadding,  topPadding),      // top left
                new Rectangle(leftX,  y,        middleWidth,  topPadding),           // top middle
                new Rectangle(rightX, y,        rightPadding, topPadding),      // top right
                new Rectangle(x,      topY,     leftPadding,  middleHeight),          // left middle
                new Rectangle(leftX,  topY,     middleWidth,  middleHeight),              // middle
                new Rectangle(rightX, topY,     rightPadding, middleHeight),         // right middle
                new Rectangle(x,      bottomY,  leftPadding,  bottomPadding),  // bottom left
                new Rectangle(leftX,  bottomY,  middleWidth,  bottomPadding),       // bottom middle
                new Rectangle(rightX, bottomY,  rightPadding, bottomPadding)   // bottom right
            };

            return patches;
        }
    }
}