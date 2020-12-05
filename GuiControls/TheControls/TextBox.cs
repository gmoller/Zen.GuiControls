using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.GuiControls.PackagesClasses;
using Zen.Input;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls.TheControls
{
    public class TextBox : Control
    {
        #region State
        private Alignment ContentAlignment { get; set; }
        private string Text { get; set; }
        private Color TextShadowColor { get; set; }
        private float Scale { get; set; }
        private string FontName { get; set; }
        #endregion

        private string TextureNormal
        {
            get => Textures.ContainsKey("TextureNormal") ? Textures["TextureNormal"].TextureString : string.Empty;
            set => AddTexture("TextureNormal", new Texture("TextureNormal", value, ControlHelper.TextureNormalIsValid, control => Bounds));
        }

        private string TextureActive
        {
            get => Textures.ContainsKey("TextureActive") ? Textures["TextureActive"].TextureString : string.Empty;
            set => AddTexture("TextureActive", new Texture("TextureActive", value, ControlHelper.TextureActiveIsValid, control => Bounds));
        }

        private string TextureHoverOrFocused
        {
            get => Textures.ContainsKey("TextureHoverOrFocused") ? Textures["TextureHoverOrFocused"].TextureString : string.Empty;
            set => AddTexture("TextureHoverOrFocused", new Texture("TextureHoverOrFocused", value, TextureHoverOrFocusedIsValid, control => Bounds));
        }

        private string TextureDisabled
        {
            get => Textures.ContainsKey("TextureDisabled") ? Textures["TextureDisabled"].TextureString : string.Empty;
            set => AddTexture("TextureDisabled", new Texture("TextureDisabled", value, ControlHelper.TextureDisabledIsValid, control => Bounds));
        }

        public static bool TextureHoverOrFocusedIsValid(IControl control)
        {
            var isValid = (control.Status.HasFlag(ControlStatus.MouseOver) || control.Status.HasFlag(ControlStatus.HasFocus)) &&
                          !control.Status.HasFlag(ControlStatus.Disabled);

            return isValid;
        }
        
        /// <summary>
        /// A titillating little textbox.
        /// </summary>
        /// <param name="name">Name of control.</param>
        private TextBox(string name) : base(name)
        {
            ContentAlignment = Alignment.TopLeft;
            Text = string.Empty;
            TextShadowColor = Color.Transparent;
            Scale = 1.0f;
            FontName = string.Empty;
            AddPackage(new ControlClick(HandleControlClick));
            AddPackage(new KeyPressed(HandleKeyPressed));
        }

        private TextBox(TextBox other) : base(other)
        {
            ContentAlignment = other.ContentAlignment;
            Text = other.Text;
            TextShadowColor = other.TextShadowColor;
            Scale = other.Scale;
            FontName = other.FontName;
        }

        internal static TextBox Create(string name, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            try
            {
                var control = new TextBox(name);
                control = Update(control, state, callingTypeFullName, callingAssemblyFullName);

                return control;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create textbox [{name}]", e);
            }
        }

        internal static TextBox Update(TextBox control, StateDictionary state, string callingTypeFullName, string callingAssemblyFullName)
        {
            control.FontName = state.GetAsString("FontName", control.FontName);
            control.ContentAlignment = state.GetAsAlignment("ContentAlignment", control.ContentAlignment);
            control.Text = state.GetAsString("Text", control.Text);
            control.TextShadowColor = state.GetAsColor("TextShadowColor", control.TextShadowColor);
            control.Scale = state.GetAsSingle("Scale", control.Scale);
            control.TextureNormal = state.GetAsString("TextureNormal", control.TextureNormal);
            control.TextureHoverOrFocused = state.GetAsString("TextureHoverOrFocused", control.TextureHoverOrFocused);
            control.TextureActive = state.GetAsString("TextureActive", control.TextureActive);
            control.TextureDisabled = state.GetAsString("TextureDisabled", control.TextureDisabled);
            control.AddTextures(state.GetAsListOfTextures("Textures", callingTypeFullName, callingAssemblyFullName, new List<Texture>()));

            return control;
        }

        public override IControl Clone()
        {
            return new TextBox(this);
        }

        public override void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            if (loadChildrenContent)
            {
                ChildControls.LoadContent(content, true);
            }
        }

        protected override void DrawExtra(SpriteBatch spriteBatch)
        {
            var font = AssetsManager.Instance.GetSpriteFont(FontName);

            var offset = DetermineOffset(font, new Vector2(Size.X, Size.Y), Text, ContentAlignment, Scale);

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

        private static Vector2 DetermineOffset(SpriteFont font, Vector2 size, string text, Alignment contentAlignment, float scale)
        {
            var textSize = font.MeasureString(text) * scale;

            return contentAlignment switch
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
                _ => throw new Exception($"ContentAlignment [{contentAlignment}] is not implemented."),
            };
        }

        private static void HandleControlClick(object sender, EventArgs args)
        {
            var txt = (TextBox)sender;
            //var mouseEventArgs = (MouseEventArgs)args;

            var controlStatusAsInt = (int)txt.Status;
            controlStatusAsInt = controlStatusAsInt.SetBit(ControlStatus.HasFocus.GetIndexOfEnumeration());

            txt.Status = (ControlStatus)controlStatusAsInt;
        }

        private static void HandleKeyPressed(object sender, EventArgs args)
        {
            var txt = (TextBox)sender;
            var keyboardEventArgs = (KeyboardEventArgs)args;

            if (txt.Status.HasFlag(ControlStatus.HasFocus))
            {
                txt.Text += keyboardEventArgs.Key;
            }
        }
    }
}