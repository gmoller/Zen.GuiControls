using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Button : ControlWithMultipleTextures
    {
        private string _textureNormal;
        public string TextureNormal
        {
            get => _textureNormal;
            set
            {
                _textureNormal = value; 
                AddTexture("TextureNormal", new Texture(value, () => Status == ControlStatus.None && Enabled, () => Bounds));
            }
        }

        private string _textureActive;
        public string TextureActive
        {
            get => _textureActive;
            set
            {
                _textureActive = value;
                AddTexture("TextureActive", new Texture(value, () => Status == ControlStatus.Active && Enabled, () => Bounds));
            }
        }

        private string _textureHover;
        public string TextureHover
        {
            get => _textureHover;
            set
            {
                _textureHover = value;
                AddTexture("TextureHover", new Texture(value, () => Status == ControlStatus.MouseOver && Enabled, () => Bounds));
            }
        }

        private string _textureDisabled;
        public string TextureDisabled
        {
            get => _textureDisabled;
            set
            {
                _textureDisabled = value;
                AddTexture("TextureDisabled", new Texture(value, () => !Enabled, () => Bounds));
            }
        }

        /// <summary>
        /// A lovely little button.
        /// </summary>
        /// <param name="name">Name of control.</param>
        public Button(string name) : base(name)
        {
        }

        private Button(Button other) : base(other)
        {
        }

        public override IControl Clone()
        {
            return new Button(this);
        }

        protected override void InDraw(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, Rectangle destinationRectangle)
        {
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color, 0.0f, Vector2.Zero, SpriteEffects.None, LayerDepth);
        }
    }
}