using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.Input;

namespace Zen.GuiControls.TheControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Label : Control
    {
        #region State
        public Alignment ContentAlignment { get; set; }
        public string Text { get; set; }
        public Func<object, string> GetTextFunc { get; set; }
        public Color TextShadowColor { get; set; }
        public float Scale { get; set; }
        public string FontName { get; set; }
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
        /// An awesome little label.
        /// </summary>
        /// <param name="name">Name of control.</param>
        public Label(string name) : base(name)
        {
            ContentAlignment = Alignment.TopLeft;
            Text = string.Empty;
            TextShadowColor = Color.Transparent;
            Scale = 1.0f;
            FontName = string.Empty;
        }

        private Label(Label other) : base(other)
        {
            ContentAlignment = other.ContentAlignment;
            Text = other.Text;
            GetTextFunc = other.GetTextFunc;
            TextShadowColor = other.TextShadowColor;
            Scale = other.Scale;
            FontName = other.FontName;
        }

        public override IControl Clone()
        {
            return new Label(this);
        }

        public override void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            if (loadChildrenContent)
            {
                ChildControls.LoadContent(content, true);
            }
        }

        public override void Update(InputHandler input, float deltaTime, Viewport? viewport = null)
        {
            if (GetTextFunc != null)
            {
                Text = GetTextFunc(this);
            }

            base.Update(input, deltaTime, viewport);
        }

        protected override void DrawExtra(SpriteBatch spriteBatch)
        {
            var font = AssetsManager.Instance.GetSpriteFont(FontName);

            var offset = DetermineOffset(font, new Vector2(Size.X, Size.Y), Text, Scale);

            spriteBatch.DrawString(
                font, 
                Text,
                new Vector2(TopLeft.X, TopLeft.Y) + offset + Vector2.One, 
                TextShadowColor, 
                0.0f, 
                Vector2.Zero,
                Scale, 
                SpriteEffects.None, 
                LayerDepth);

            spriteBatch.DrawString(
                font,
                Text,
                new Vector2(TopLeft.X, TopLeft.Y) + offset,
                Color,
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