using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ControlWithMultipleTextures : Control
    {
        #region State

        private Dictionary<string, Texture> Textures { get; }
        #endregion

        protected ControlWithMultipleTextures(string name) : base(name)
        {
            Textures = new Dictionary<string, Texture>();
        }

        protected ControlWithMultipleTextures(ControlWithMultipleTextures other) : base(other)
        {
            Textures = other.Textures;
        }

        public override IControl Clone()
        {
            return new ControlWithMultipleTextures(this);
        }

        protected void AddTexture(string textureName, Texture texture)
        {
            // up-sert
            Textures[textureName] = texture;
        }

        protected virtual void InDraw(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, Rectangle destinationRectangle)
        {
        }

        protected override void InDraw(SpriteBatch spriteBatch)
        {
            foreach (var item in Textures.Values)
            {
                if (!item.IsValid()) continue;

                var texture = GetTexture2D(item.TextureString);
                var sourceRectangle = GetSourceRectangle(texture, item.TextureString);
                var destinationRectangle = item.DestinationRectangle.Invoke();
                InDraw(spriteBatch, texture, sourceRectangle, destinationRectangle);
            }
        }

        protected static Rectangle GetSourceRectangle(Texture2D texture, string textureName)
        {
            Rectangle rectangle;
            var textureAtlas = ControlHelper.GetTextureAtlas(textureName);
            if (textureAtlas.HasValue())
            {
                var atlas = AssetsManager.Instance.GetAtlas(textureAtlas);
                var textureName2 = ControlHelper.GetTextureName(textureName);
                var frame = atlas.Frames[textureName2];
                rectangle = new Rectangle(frame.X, frame.Y, frame.Width, frame.Height);
            }
            else
            {
                rectangle = texture.Bounds;
            }

            return rectangle;
        }

        protected static Rectangle[] GetSourceRectangles(Texture2D texture, string textureName, int borderSizeTop, int borderSizeBottom, int borderSizeLeft, int borderSizeRight)
        {
            Rectangle[] sourceRectangles;
            var textureAtlas = ControlHelper.GetTextureAtlas(textureName);
            if (textureAtlas.HasValue())
            {
                var atlas = AssetsManager.Instance.GetAtlas(textureAtlas);
                var textureName2 = ControlHelper.GetTextureName(textureName);
                var frame = atlas.Frames[textureName2];

                sourceRectangles = Get9SliceScaleRectangles(frame.ToRectangle(), borderSizeTop, borderSizeBottom, borderSizeLeft, borderSizeRight);
            }
            else
            {
                var frame = texture.Bounds;
                sourceRectangles = Get9SliceScaleRectangles(frame, borderSizeTop, borderSizeBottom, borderSizeLeft, borderSizeRight);
            }

            return sourceRectangles;
        }

        protected static Rectangle[] Get9SliceScaleRectangles(Rectangle rectangle, int borderSizeTop, int borderSizeBottom, int borderSizeLeft, int borderSizeRight)
        {
            var x = rectangle.X;
            var y = rectangle.Y;
            var w = rectangle.Width;
            var h = rectangle.Height;
            var middleWidth = w - borderSizeLeft - borderSizeRight;
            var middleHeight = h - borderSizeTop - borderSizeBottom;
            var bottomY = y + h - borderSizeBottom;
            var rightX = x + w - borderSizeRight;
            var leftX = x + borderSizeLeft;
            var topY = y + borderSizeTop;

            var patches = new[]
            {
                new Rectangle(x,      y,        borderSizeLeft,  borderSizeTop),      // top left
                new Rectangle(leftX,  y,        middleWidth,  borderSizeTop),              // top middle
                new Rectangle(rightX, y,        borderSizeRight, borderSizeTop),      // top right
                new Rectangle(x,      topY,     borderSizeLeft,  middleHeight),             // left middle
                new Rectangle(leftX,  topY,     middleWidth,  middleHeight),                     // middle
                new Rectangle(rightX, topY,     borderSizeRight, middleHeight),             // right middle
                new Rectangle(x,      bottomY,  borderSizeLeft,  borderSizeBottom),   // bottom left
                new Rectangle(leftX,  bottomY,  middleWidth,  borderSizeBottom),           // bottom middle
                new Rectangle(rightX, bottomY,  borderSizeRight, borderSizeBottom)    // bottom right
            };

            return patches;
        }

        protected static Texture2D GetTexture2D(string textureName)
        {
            var textureAtlas = ControlHelper.GetTextureAtlas(textureName);
            var texture = AssetsManager.Instance.GetTexture(textureAtlas.HasValue() ? textureAtlas : textureName);

            return texture;
        }
    }
}