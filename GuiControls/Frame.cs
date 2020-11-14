using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.MonoGameUtilities.ExtensionMethods;
using Zen.Utilities;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Frame : ControlWithSingleTexture
    {
        #region State
        public int TopPadding { get; set; }
        public int BottomPadding { get; set; }
        public int LeftPadding { get; set; }
        public int RightPadding { get; set; }

        private Rectangle[] _sourcePatches;
        private Rectangle[] _destinationPatches;
        #endregion

        /// <summary>
        /// Use this constructor if Frame is expected to be stand alone (have no parent).
        /// </summary>
        /// <param name="name">Name of frame control.</param>
        /// <param name="textureName">Texture to use for rendering. If texture is from a texture atlas use 'AtlasName.TextureName'</param>
        /// <param name="topPadding">Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)</param>
        /// <param name="bottomPadding">Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)</param>
        /// <param name="leftPadding">Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)</param>
        /// <param name="rightPadding">Used to 'stretch' the texture to the size of the frame. Using 9-slice scaling (http://en.wikipedia.org/wiki/9-slice_scaling)</param>
        public Frame(
            string name,
            string textureName = "",
            int topPadding = 0,
            int bottomPadding = 0,
            int leftPadding = 0,
            int rightPadding = 0) :
            base(name, textureName)
        {
            TopPadding = topPadding;
            BottomPadding = bottomPadding;
            LeftPadding = leftPadding;
            RightPadding = rightPadding;
        }

        public Frame Clone()
        {
            var clone = new Frame(Name, TextureName, TopPadding, BottomPadding, LeftPadding, RightPadding);
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

        public override void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            if (TextureAtlas.HasValue())
            {
                Texture = AssetsManager.Instance.GetTexture(TextureAtlas);
                var atlas = AssetsManager.Instance.GetAtlas(TextureAtlas);

                var frame = atlas.Frames[TextureName];

                _sourcePatches = CreatePatches(frame.ToRectangle(), TopPadding, BottomPadding, LeftPadding, RightPadding);
            }
            else if (TextureName.HasValue())
            {
                Texture = AssetsManager.Instance.GetTexture(TextureName);
                var frame = Texture.Bounds;
                _sourcePatches = CreatePatches(frame, TopPadding, BottomPadding, LeftPadding, RightPadding);
            }

            _destinationPatches = CreatePatches(new Rectangle(TopLeft.X, TopLeft.Y, Size.X, Size.Y), TopPadding, BottomPadding, LeftPadding, RightPadding);

            base.LoadContent(content, loadChildrenContent);
        }

        protected override void InDraw(SpriteBatch spriteBatch)
        {
            if (Texture.HasValue())
            {
                for (var i = 0; i < _sourcePatches.Length; ++i)
                {
                    spriteBatch.Draw(Texture, _destinationPatches[i], _sourcePatches[i], Color, 0.0f, Vector2.Zero,
                        SpriteEffects.None, LayerDepth);
                }
            }
        }

        public override void SetPosition(PointI point)
        {
            base.SetPosition(point);

            if (_destinationPatches != null)
            {
                _destinationPatches = CreatePatches(new Rectangle(TopLeft.X, TopLeft.Y, Size.X, Size.Y), TopPadding, BottomPadding, LeftPadding, RightPadding);
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