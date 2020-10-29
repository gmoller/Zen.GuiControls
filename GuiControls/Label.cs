using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.Input;
using Zen.MonoGameUtilities;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Label : Control
    {
        #region State
        public Alignment ContentAlignment { get; set; }
        public string Text { get; set; }
        public Func<string> GetTextFunc { get; set; }
        public Color TextColor { get; set; }
        public Color? TextShadowColor { get; set; }
        public Color? BackgroundColor { get; set; }
        public Color? BorderColor { get; set; }
        public float Scale { get; set; }

        private string FontName { get; }
        private SpriteFont Font { get; set; }
        #endregion

        /// <summary>
        /// An awesome little label.
        /// </summary>
        /// <param name="name">Name of control.</param>
        /// <param name="fontName">Font to use for label text.</param>
        public Label(
            string name,
            string fontName) : 
            base(name)
        {
            ContentAlignment = Alignment.TopLeft;
            Text = string.Empty;
            TextColor = Color.White;
            TextShadowColor = null;
            BackgroundColor = null;
            BorderColor = null;
            Scale = 1.0f;
            FontName = fontName;
        }

        public override void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            Font = AssetsManager.Instance.GetSpriteFont(FontName);

            if (loadChildrenContent)
            {
                ChildControls.LoadChildControls(content, true);
            }
        }

        public override void Update(InputHandler input, float deltaTime, Viewport? viewport = null)
        {
            if (GetTextFunc != null)
            {
                Text = GetTextFunc();
            }

            base.Update(input, deltaTime, viewport);
        }

        protected override void InDraw(SpriteBatch spriteBatch)
        {
            if (BackgroundColor != null)
            {
                spriteBatch.FillRectangle(
                    ActualDestinationRectangle,
                    BackgroundColor.Value, 
                    LayerDepth);
            }

            var offset = DetermineOffset(Font, new Vector2(Size.X, Size.Y), Text, Scale);
            if (TextShadowColor != null)
            {
                spriteBatch.DrawString(
                    Font, 
                    Text,
                    new Vector2(TopLeft.X, TopLeft.Y) + offset + Vector2.One, 
                    TextShadowColor.Value, 
                    0.0f, 
                    Vector2.Zero,
                    Scale, 
                    SpriteEffects.None, 
                    LayerDepth);
            }

            if (BorderColor != null)
            {
                spriteBatch.DrawRectangle(
                    new Rectangle(
                        ActualDestinationRectangle.X,
                        ActualDestinationRectangle.Y,
                        ActualDestinationRectangle.Width - 1,
                        ActualDestinationRectangle.Height - 1),
                    BorderColor.Value, 
                    LayerDepth);
            }

            spriteBatch.DrawString(
                Font, 
                Text,
                new Vector2(TopLeft.X, TopLeft.Y) + offset, 
                TextColor, 
                0.0f, 
                Vector2.Zero,
                Scale, 
                SpriteEffects.None, 
                LayerDepth);
        }

        private Vector2 DetermineOffset(SpriteFont font, Vector2 size, string text, float scale)
        {
            var textSize = font.MeasureString(text) * scale;

            return ContentAlignment switch
            {
                Alignment.TopLeft => Vector2.Zero,
                Alignment.TopCenter => new Vector2((size.X - textSize.X) * 0.5f, 0.0f),
                Alignment.TopRight => new Vector2(size.X - textSize.X, 0.0f),
                Alignment.MiddleLeft => new Vector2(0.0f, (size.Y - textSize.Y) * 0.5f),
                Alignment.MiddleCenter => new Vector2((size.X - textSize.X) * 0.5f, (size.Y - textSize.Y) * 0.5f),
                Alignment.MiddleRight => new Vector2(size.X - textSize.X, (size.Y - textSize.Y) * 0.5f),
                Alignment.BottomLeft => new Vector2(0.0f, size.Y - textSize.Y),
                Alignment.BottomCenter => new Vector2((size.X - textSize.X) * 0.5f, size.Y - textSize.Y),
                Alignment.BottomRight => new Vector2(size.X - textSize.X, size.Y - textSize.Y),
                _ => throw new Exception($"ContentAlignment [{ContentAlignment}] is not implemented."),
            };
        }
    }
}