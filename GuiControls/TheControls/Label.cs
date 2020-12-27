using System;
using System.Collections.Generic;
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

        public string TextureNormal
        {
            get => Textures.ContainsKey("TextureNormal") ? Textures["TextureNormal"].TextureString : string.Empty;
            set => AddTexture("TextureNormal", new Texture("TextureNormal", value, ControlHelper.TextureNormalIsValid, control => Bounds));
        }

        public string TextureActive
        {
            get => Textures.ContainsKey("TextureActive") ? Textures["TextureActive"].TextureString : string.Empty;
            set => AddTexture("TextureActive", new Texture("TextureActive", value, ControlHelper.TextureActiveIsValid, control => Bounds));
        }

        public string TextureHover
        {
            get => Textures.ContainsKey("TextureHover") ? Textures["TextureHover"].TextureString : string.Empty;
            set => AddTexture("TextureHover", new Texture("TextureHover", value, ControlHelper.TextureHoverIsValid, control => Bounds));
        }

        public string TextureDisabled
        {
            get => Textures.ContainsKey("TextureDisabled") ? Textures["TextureDisabled"].TextureString : string.Empty;
            set => AddTexture("TextureDisabled", new Texture("TextureDisabled", value, ControlHelper.TextureDisabledIsValid, control => Bounds));
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

        internal static Label Create(string name, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            try
            {
                var control = new Label(name);
                control = Update(control, state, callingTypeFullName, callingAssemblyFullName);

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create label [{name}]", e);
            }
        }

        internal static Label Update(Label control, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            control.FontName = state.GetAsString("FontName", control.FontName);
            control.ContentAlignment = state.GetAsAlignment("ContentAlignment", control.ContentAlignment);
            control.Text = state.GetAsString("Text", control.Text);
            control.GetTextFunc = state.GetAsGetTextFunc("GetTextFunc", callingTypeFullName, callingAssemblyFullName, control.GetTextFunc);
            control.TextShadowColor = state.GetAsColor("TextShadowColor", control.TextShadowColor);
            control.Scale = state.GetAsSingle("Scale", control.Scale);
            control.TextureNormal = state.GetAsString("TextureNormal", control.TextureNormal);
            control.TextureHover = state.GetAsString("TextureHover", control.TextureHover);
            control.TextureActive = state.GetAsString("TextureActive", control.TextureActive);
            control.TextureDisabled = state.GetAsString("TextureDisabled", control.TextureDisabled);
            control.AddTextures(state.GetAsListOfTextures("Textures", callingTypeFullName, callingAssemblyFullName, new List<Texture>()));

            return control;
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

        public override void Update(InputHandler input, GameTime gameTime, Viewport? viewport = null)
        {
            if (GetTextFunc != null)
            {
                Text = GetTextFunc(this);
            }

            base.Update(input, gameTime, viewport);
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