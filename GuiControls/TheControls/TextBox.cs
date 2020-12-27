using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Zen.Assets;
using Zen.GuiControls.PackagesClasses;
using Zen.Input;
using Zen.MonoGameUtilities.ExtensionMethods;
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
        private int CursorPosition { get; set; }
        private static int CursorBlinkRateMs = 500; // -1;1200;1100;1000;900;800;700;600;500;400;300;200 (none->fast)
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

        private string TextureCursor
        {
            get => Textures.ContainsKey("TextureCursor") ? Textures["TextureCursor"].TextureString : string.Empty;
            set => AddTexture("TextureCursor", new Texture("TextureCursor", value, TextureFocusedIsValid, GetDestination));
        }

        public static bool TextureFocusedIsValid(IControl control)
        {
            var isValid = control.Status.HasFlag(ControlStatus.HasFocus) &&
                          !control.Status.HasFlag(ControlStatus.Disabled);

            if (isValid)
            {
                var foo = (int)(control.GameTime.TotalGameTime.TotalMilliseconds / CursorBlinkRateMs);
                isValid = foo.IsEven();
            }


            return isValid;
        }

        public static bool TextureHoverOrFocusedIsValid(IControl control)
        {
            var isValid = (control.Status.HasFlag(ControlStatus.MouseOver) || control.Status.HasFlag(ControlStatus.HasFocus)) &&
                          !control.Status.HasFlag(ControlStatus.Disabled);

            return isValid;
        }

        private Rectangle GetDestination(IControl control)
        {
            var font = AssetsManager.Instance.GetSpriteFont(FontName);
            var substring = Text.GetFirstCharacters(CursorPosition);
            var sizeOfText = font.MeasureString(substring);
            var textSize = DetermineTextSize(font, Text, Scale);
            var offset = DetermineOffset(new Vector2(Size.X, Size.Y), textSize, ContentAlignment);
            var size = new Point(10, Bounds.Height);
            var position = TopLeft.ToPoint() + offset.ToPoint() - new Point(size.X / 2, 0) + new Point((int)sizeOfText.X + 2, 0);

            var rectangle = new Rectangle(position, size);

            return rectangle;
        }

        /// <summary>
        /// A titillating little textbox.
        /// </summary>
        /// <param name="name">Name of control.</param>
        public TextBox(string name) : base(name)
        {
            ContentAlignment = Alignment.TopLeft;
            Text = string.Empty;
            TextShadowColor = Color.Transparent;
            Scale = 1.0f;
            FontName = string.Empty;
            CursorPosition = 0;
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
            CursorPosition = other.CursorPosition;
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
            control.TextureCursor = state.GetAsString("TextureCursor", control.TextureCursor);
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

            var textSize = DetermineTextSize(font, Text, Scale);
            var offset = DetermineOffset(new Vector2(Size.X, Size.Y), textSize, ContentAlignment);

            // textshadow
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

            // text
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

        private static Vector2 DetermineTextSize(SpriteFont font, string text, in float scale)
        {
            var textSize = font.MeasureString(text) * scale;

            return textSize;
        }

        private static Vector2 DetermineOffset(Vector2 size, Vector2 textSize, Alignment contentAlignment)
        {
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

        private void HandleKeyPressed(object sender, EventArgs args)
        {
            var txt = (TextBox)sender;
            var keyboardEventArgs = (KeyboardEventArgs)args;

            char keyToAdd = '\0';
            var delete = false;
            var left = false;
            var right = false;
            if (keyboardEventArgs.Key == Keys.Back)
            {
                delete = true;
            }
            else
            {
                var shiftPressed = keyboardEventArgs.Keyboard.IsKeyDown(Keys.LeftShift) || keyboardEventArgs.Keyboard.IsKeyDown(Keys.RightShift);
                switch (keyboardEventArgs.Key)
                {
                    case Keys.None:
                        break;
                    case Keys.Back: // backspace
                        break;
                    case Keys.Tab:
                        keyToAdd = '\t';
                        break;
                    case Keys.Enter:
                        break;
                    case Keys.CapsLock:
                        break;
                    case Keys.Escape:
                        break;
                    case Keys.Space:
                        keyToAdd = ' ';
                        break;
                    case Keys.PageUp:
                        break;
                    case Keys.PageDown:
                        break;
                    case Keys.End:
                        break;
                    case Keys.Home:
                        break;
                    case Keys.Left:
                        left = true;
                        break;
                    case Keys.Up:
                        break;
                    case Keys.Right:
                        right = true;
                        break;
                    case Keys.Down:
                        break;
                    case Keys.Select:
                        break;
                    case Keys.Print:
                        break;
                    case Keys.Execute:
                        break;
                    case Keys.PrintScreen:
                        break;
                    case Keys.Insert:
                        break;
                    case Keys.Delete:
                        break;
                    case Keys.Help:
                        break;

                    case Keys.D0:
                        keyToAdd = shiftPressed ? ')' : '0';
                        break;
                    case Keys.D1:
                        keyToAdd = shiftPressed ? '!' : '1';
                        break;
                    case Keys.D2:
                        keyToAdd = shiftPressed ? '@' : '2';
                        break;
                    case Keys.D3:
                        keyToAdd = shiftPressed ? '#' : '3';
                        break;
                    case Keys.D4:
                        keyToAdd = shiftPressed ? '$' : '4';
                        break;
                    case Keys.D5:
                        keyToAdd = shiftPressed ? '%' : '5';
                        break;
                    case Keys.D6:
                        keyToAdd = shiftPressed ? '^' : '6';
                        break;
                    case Keys.D7:
                        keyToAdd = shiftPressed ? '&' : '7';
                        break;
                    case Keys.D8:
                        keyToAdd = shiftPressed ? '*' : '8';
                        break;
                    case Keys.D9:
                        keyToAdd = shiftPressed ? '(' : '9';
                        break;

                    case Keys.NumPad0:
                        keyToAdd = '0';
                        break;
                    case Keys.NumPad1:  
                        keyToAdd = '1';
                        break;
                    case Keys.NumPad2:
                        keyToAdd = '2';
                        break;
                    case Keys.NumPad3:
                        keyToAdd = '3';
                        break;
                    case Keys.NumPad4:
                        keyToAdd = '4';
                        break;
                    case Keys.NumPad5:
                        keyToAdd = '5';
                        break;
                    case Keys.NumPad6:
                        keyToAdd = '6';
                        break;
                    case Keys.NumPad7:
                        keyToAdd = '7';
                        break;
                    case Keys.NumPad8:
                        keyToAdd = '8';
                        break;
                    case Keys.NumPad9:
                        keyToAdd = '9';
                        break;

                    case Keys.A:
                        keyToAdd = shiftPressed ? 'A' : 'a';
                        break;
                    case Keys.B:
                        keyToAdd = shiftPressed ? 'B' : 'b';
                        break;
                    case Keys.C:
                        keyToAdd = shiftPressed ? 'C' : 'c';
                        break;
                    case Keys.D:
                        keyToAdd = shiftPressed ? 'D' : 'd';
                        break;
                    case Keys.E:
                        keyToAdd = shiftPressed ? 'E' : 'e';
                        break;
                    case Keys.F:
                        keyToAdd = shiftPressed ? 'F' : 'f';
                        break;
                    case Keys.G:
                        keyToAdd = shiftPressed ? 'G' : 'g';
                        break;
                    case Keys.H:
                        keyToAdd = shiftPressed ? 'H' : 'h';
                        break;
                    case Keys.I:
                        keyToAdd = shiftPressed ? 'I' : 'i';
                        break;
                    case Keys.J:
                        keyToAdd = shiftPressed ? 'J' : 'j';
                        break;
                    case Keys.K:
                        keyToAdd = shiftPressed ? 'K' : 'k';
                        break;
                    case Keys.L:
                        keyToAdd = shiftPressed ? 'L' : 'l';
                        break;
                    case Keys.M:
                        keyToAdd = shiftPressed ? 'M' : 'm';
                        break;
                    case Keys.N:
                        keyToAdd = shiftPressed ? 'N' : 'n';
                        break;
                    case Keys.O:
                        keyToAdd = shiftPressed ? 'O' : 'o';
                        break;
                    case Keys.P:
                        keyToAdd = shiftPressed ? 'P' : 'p';
                        break;
                    case Keys.Q:
                        keyToAdd = shiftPressed ? 'Q' : 'q';
                        break;
                    case Keys.R:
                        keyToAdd = shiftPressed ? 'R' : 'r';
                        break;
                    case Keys.S:
                        keyToAdd = shiftPressed ? 'S' : 's';
                        break;
                    case Keys.T:
                        keyToAdd = shiftPressed ? 'T' : 't';
                        break;
                    case Keys.U:
                        keyToAdd = shiftPressed ? 'U' : 'u';
                        break;
                    case Keys.V:
                        keyToAdd = shiftPressed ? 'V' : 'v';
                        break;
                    case Keys.W:
                        keyToAdd = shiftPressed ? 'W' : 'w';
                        break;
                    case Keys.X:
                        keyToAdd = shiftPressed ? 'X' : 'x';
                        break;
                    case Keys.Y:
                        keyToAdd = shiftPressed ? 'Y' : 'y';
                        break;
                    case Keys.Z:
                        keyToAdd = shiftPressed ? 'Z' : 'z';
                        break;

                    case Keys.LeftWindows:
                        break;
                    case Keys.RightWindows:
                        break;
                    case Keys.Apps:
                        break;
                    case Keys.Sleep:
                        break;
                    case Keys.Multiply:
                        keyToAdd = '*';
                        break;
                    case Keys.Add:
                        keyToAdd = '+';
                        break;
                    case Keys.Separator:
                        break;
                    case Keys.Subtract:
                        keyToAdd = '-';
                        break;
                    case Keys.Decimal:
                        break;
                    case Keys.Divide:
                        keyToAdd = '/';
                        break;

                    case Keys.F1:
                        break;
                    case Keys.F2:
                        break;
                    case Keys.F3:
                        break;
                    case Keys.F4:
                        break;
                    case Keys.F5:
                        break;
                    case Keys.F6:
                        break;
                    case Keys.F7:
                        break;
                    case Keys.F8:
                        break;
                    case Keys.F9:
                        break;
                    case Keys.F10:
                        break;
                    case Keys.F11:
                        break;
                    case Keys.F12:
                        break;
                    case Keys.F13:
                        break;
                    case Keys.F14:
                        break;
                    case Keys.F15:
                        break;
                    case Keys.F16:
                        break;
                    case Keys.F17:
                        break;
                    case Keys.F18:
                        break;
                    case Keys.F19:
                        break;
                    case Keys.F20:
                        break;
                    case Keys.F21:
                        break;
                    case Keys.F22:
                        break;
                    case Keys.F23:
                        break;
                    case Keys.F24:
                        break;

                    case Keys.NumLock:
                        break;
                    case Keys.Scroll:
                        break;
                    case Keys.LeftShift:
                        break;
                    case Keys.RightShift:
                        break;
                    case Keys.LeftControl:
                        break;
                    case Keys.RightControl:
                        break;
                    case Keys.LeftAlt:
                        break;
                    case Keys.RightAlt:
                        break;
                    case Keys.BrowserBack:
                        break;
                    case Keys.BrowserForward:
                        break;
                    case Keys.BrowserRefresh:
                        break;
                    case Keys.BrowserStop:
                        break;
                    case Keys.BrowserSearch:
                        break;
                    case Keys.BrowserFavorites:
                        break;
                    case Keys.BrowserHome:
                        break;
                    case Keys.VolumeMute:
                        break;
                    case Keys.VolumeDown:
                        break;
                    case Keys.VolumeUp:
                        break;
                    case Keys.MediaNextTrack:
                        break;
                    case Keys.MediaPreviousTrack:
                        break;
                    case Keys.MediaStop:
                        break;
                    case Keys.MediaPlayPause:
                        break;
                    case Keys.LaunchMail:
                        break;
                    case Keys.SelectMedia:
                        break;
                    case Keys.LaunchApplication1:
                        break;
                    case Keys.LaunchApplication2:
                        break;

                    case Keys.OemSemicolon:
                        keyToAdd = shiftPressed ? ':' : ';';
                        break;
                    case Keys.OemPlus:
                        keyToAdd = shiftPressed ? '+' : '=';
                        break;
                    case Keys.OemComma:
                        keyToAdd = shiftPressed ? '<' : ',';
                        break;
                    case Keys.OemMinus:
                        keyToAdd = shiftPressed ? '_' : '-';
                        break;
                    case Keys.OemPeriod:
                        keyToAdd = shiftPressed ? '>' : '.';
                        break;
                    case Keys.OemQuestion:
                        keyToAdd = shiftPressed ? '?' : '/';
                        break;
                    case Keys.OemTilde:
                        keyToAdd = shiftPressed ? '~' : '`';
                        break;
                    case Keys.OemOpenBrackets:
                        keyToAdd = shiftPressed ? '{' : '[';
                        break;
                    case Keys.OemPipe:
                        keyToAdd = shiftPressed ? '|' : '\\';
                        break;
                    case Keys.OemCloseBrackets:
                        keyToAdd = shiftPressed ? '}' : ']';
                        break;
                    case Keys.OemQuotes:
                        keyToAdd = shiftPressed ? '"' : '\'';
                        break;
                    case Keys.Oem8:
                        break;
                    case Keys.OemBackslash:
                        break;
                    case Keys.ProcessKey:
                        break;
                    case Keys.Attn:
                        break;
                    case Keys.Crsel:
                        break;
                    case Keys.Exsel:
                        break;
                    case Keys.EraseEof:
                        break;
                    case Keys.Play:
                        break;
                    case Keys.Zoom:
                        break;
                    case Keys.Pa1:
                        break;
                    case Keys.OemClear:
                        break;
                    case Keys.ChatPadGreen:
                        break;
                    case Keys.ChatPadOrange:
                        break;
                    case Keys.Pause:
                        break;
                    case Keys.ImeConvert:
                        break;
                    case Keys.ImeNoConvert:
                        break;
                    case Keys.Kana:
                        break;
                    case Keys.Kanji:
                        break;
                    case Keys.OemAuto:
                        break;
                    case Keys.OemCopy:
                        break;
                    case Keys.OemEnlW:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (keyToAdd != '\0')
            {
                var foo = txt.Text.Substring(0, CursorPosition);
                foo += keyToAdd;
                foo += txt.Text.Substring(CursorPosition, txt.Text.Length - CursorPosition);
                txt.Text = foo;
                CursorPosition++;
            }

            if (delete)
            {
                var foo = txt.Text.Substring(0, CursorPosition);
                foo = foo.RemoveLastCharacter();
                foo += txt.Text.Substring(CursorPosition, txt.Text.Length - CursorPosition);
                txt.Text = foo;
                CursorPosition--;
            }

            if (left)
            {
                CursorPosition--;
                CursorPosition = Math.Max(0, CursorPosition);
            }

            if (right)
            {
                CursorPosition++;
                CursorPosition = Math.Min(Text.Length, CursorPosition);
            }
        }
    }
}