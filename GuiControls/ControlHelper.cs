using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.Utilities;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    internal static class ControlHelper
    {
        internal static bool IsMouseOverControl(Rectangle actualDestinationRectangle, Point mousePosition, Viewport? viewport)
        {
            var mousePosition2 = GetMousePosition(mousePosition, viewport);
            var mouseIsOverControl = actualDestinationRectangle.Contains(mousePosition2.X, mousePosition2.Y);

            return mouseIsOverControl;
        }

        internal static PointI GetMousePosition(Point mousePosition, Viewport? viewport)
        {
            var retMousePosition = viewport.HasValue ? new PointI(mousePosition.X - viewport.Value.X, mousePosition.Y - viewport.Value.Y) : new PointI(mousePosition.X, mousePosition.Y);

            return retMousePosition;
        }

        internal static PointI DetermineTopLeft(IControl childControl, Alignment parentAlignment, Alignment childAlignment, PointI offset, Vector2 parentPosition, Alignment parentPositionAlignment, PointI size)
        {
            var actualRectangle = DetermineArea(parentPosition, parentPositionAlignment, size);

            PointI topLeft;
            switch (parentAlignment)
            {
                case Alignment.TopLeft when childAlignment == Alignment.TopLeft:
                    topLeft = new PointI(actualRectangle.Left, actualRectangle.Top);
                    break;
                case Alignment.TopCenter when childAlignment == Alignment.TopCenter:
                    topLeft = new PointI(actualRectangle.Left + (int)((actualRectangle.Size.X - childControl.Size.X) * 0.5f), actualRectangle.Top);
                    break;
                case Alignment.TopRight when childAlignment == Alignment.TopRight:
                    topLeft = new PointI(actualRectangle.Right - childControl.Size.X, actualRectangle.Top);
                    break;

                case Alignment.MiddleLeft when childAlignment == Alignment.MiddleLeft:
                    topLeft = new PointI(actualRectangle.Left, actualRectangle.Top + (int)((actualRectangle.Size.Y - childControl.Size.Y) * 0.5f));
                    break;
                case Alignment.MiddleCenter when childAlignment == Alignment.MiddleCenter:
                    topLeft = new PointI(actualRectangle.Left + (int)((actualRectangle.Size.X - childControl.Size.X) * 0.5f), actualRectangle.Top + (int)((actualRectangle.Size.Y - childControl.Size.Y) * 0.5f));
                    break;
                case Alignment.MiddleRight when childAlignment == Alignment.MiddleRight:
                    topLeft = new PointI(actualRectangle.Right, actualRectangle.Top + (int)((actualRectangle.Size.Y - childControl.Size.Y) * 0.5f));
                    break;
                case Alignment.MiddleRight when childAlignment == Alignment.MiddleLeft:
                    topLeft = new PointI(actualRectangle.Right, actualRectangle.Top + (int)((actualRectangle.Size.Y - childControl.Size.Y) * 0.5f));
                    break;

                case Alignment.BottomLeft when childAlignment == Alignment.BottomLeft:
                    topLeft = new PointI(actualRectangle.Left, actualRectangle.Bottom - childControl.Size.Y);
                    break;
                case Alignment.BottomLeft when childAlignment == Alignment.TopLeft:
                    topLeft = new PointI(actualRectangle.Left, actualRectangle.Bottom);
                    break;
                case Alignment.BottomCenter when childAlignment == Alignment.BottomCenter:
                    topLeft = new PointI(actualRectangle.Left + (int)((actualRectangle.Size.X - childControl.Size.X) * 0.5f), actualRectangle.Bottom - childControl.Size.Y);
                    break;
                case Alignment.BottomCenter when childAlignment == Alignment.TopCenter:
                    topLeft = new PointI(actualRectangle.Left + (int)((actualRectangle.Size.X - childControl.Size.X) * 0.5f), actualRectangle.Bottom);
                    break;
                case Alignment.BottomRight when childAlignment == Alignment.BottomRight:
                    topLeft = new PointI(actualRectangle.Right - childControl.Size.X, actualRectangle.Bottom - childControl.Size.Y);
                    break;
                default:
                    throw new Exception($"ParentAlignment [{parentAlignment}] with ChildAlignment [{childAlignment}] not implemented.");
            }
            topLeft += offset;

            return topLeft;
        }

        internal static Rectangle DetermineArea(Vector2 position, Alignment alignment, PointI size)
        {
            var topLeft = DetermineTopLeft(position, alignment, size);
            var actualDestinationRectangle = new Rectangle((int)topLeft.X, (int)topLeft.Y, size.X, size.Y);

            return actualDestinationRectangle;
        }

        internal static Vector2 DetermineTopLeft(Vector2 position, Alignment alignment, PointI size)
        {
            Vector2 topLeft;
            switch (alignment)
            {
                case Alignment.TopLeft:
                    topLeft = position;
                    break;
                case Alignment.TopCenter:
                    topLeft = new Vector2(position.X - size.X * 0.5f, position.Y);
                    break;
                case Alignment.TopRight:
                    topLeft = new Vector2(position.X - size.X, position.Y);
                    break;
                case Alignment.MiddleLeft:
                    topLeft = new Vector2(position.X, position.Y - size.Y * 0.5f);
                    break;
                case Alignment.MiddleCenter:
                    topLeft = new Vector2(position.X - size.X * 0.5f, position.Y - size.Y * 0.5f);
                    break;
                case Alignment.MiddleRight:
                    topLeft = new Vector2(position.X - size.X, position.Y - size.Y * 0.5f);
                    break;
                case Alignment.BottomLeft:
                    topLeft = new Vector2(position.X, position.Y - size.Y);
                    break;
                case Alignment.BottomCenter:
                    topLeft = new Vector2(position.X - size.X * 0.5f, position.Y - size.Y);
                    break;
                case Alignment.BottomRight:
                    topLeft = new Vector2(position.X - size.X, position.Y - size.Y);
                    break;
                default:
                    throw new Exception($"Alignment [{alignment}] not implemented.");
            }

            return topLeft;
        }

        internal static string GetTextureAtlas(string textureName)
        {
            string returnTextureAtlas;

            var split = textureName.Split('.');
            switch (split.Length)
            {
                case 1:
                    returnTextureAtlas = null;
                    break;
                case 2:
                    returnTextureAtlas = split[0];
                    break;
                default:
                    throw new Exception($"TextureName [{textureName}] may not have more than one period.");
            }

            return returnTextureAtlas;
        }

        internal static string GetTextureName(string textureName)
        {
            string returnTextureName;

            var split = textureName.Split('.');
            switch (split.Length)
            {
                case 1:
                    returnTextureName = split[0];
                    break;
                case 2:
                    returnTextureName = split[1];
                    break;
                default:
                    throw new Exception($"TextureName [{textureName}] may not have more than one period.");
            }

            return returnTextureName;
        }

        internal static Texture2D GetTexture2D(string textureName)
        {
            var textureAtlas = GetTextureAtlas(textureName);
            var texture = AssetsManager.Instance.GetTexture(textureAtlas.HasValue() ? textureAtlas : textureName);

            return texture;
        }

        internal static Rectangle GetSourceRectangle(Texture2D texture, string textureName)
        {
            Rectangle rectangle;
            var textureAtlas = ControlHelper.GetTextureAtlas(textureName);
            if (textureAtlas.HasValue())
            {
                var atlas = AssetsManager.Instance.GetAtlas(textureAtlas);
                var textureName2 = GetTextureName(textureName);
                var frame = atlas.Frames[textureName2];
                rectangle = new Rectangle(frame.X, frame.Y, frame.Width, frame.Height);
            }
            else
            {
                rectangle = texture.Bounds;
            }

            return rectangle;
        }

        internal static Rectangle[] GetSourceRectangles(Texture2D texture, string textureName, int borderSizeTop, int borderSizeBottom, int borderSizeLeft, int borderSizeRight)
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

        internal static Rectangle[] Get9SliceScaleRectangles(Rectangle rectangle, int borderSizeTop, int borderSizeBottom, int borderSizeLeft, int borderSizeRight)
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
    }
}