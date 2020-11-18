using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    public class ControlWithSingleTexture : Control
    {
        #region State
        public string TextureName { get; set; }
        #endregion

        protected ControlWithSingleTexture(string name) : base(name)
        {
        }

        protected ControlWithSingleTexture(ControlWithSingleTexture other) : base(other)
        {
            TextureName = other.TextureName;
        }

        public override IControl Clone()
        {
            return new ControlWithSingleTexture(this);
        }

        protected virtual void InDraw(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle)
        {
        }

        protected override void InDraw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle;
            Texture2D texture;
            var textureAtlas = ControlHelper.GetTextureAtlas(TextureName);
            if (textureAtlas.HasValue())
            {
                var atlas = AssetsManager.Instance.GetAtlas(textureAtlas);
                texture = AssetsManager.Instance.GetTexture(textureAtlas);
                var textureName = ControlHelper.GetTextureName(TextureName);
                var f = atlas.Frames[textureName];
                sourceRectangle = new Rectangle(f.X, f.Y, f.Width, f.Height);
            }
            else
            {
                texture = AssetsManager.Instance.GetTexture(TextureName);
                sourceRectangle = texture.Bounds;
            }

            InDraw(spriteBatch, texture, sourceRectangle);
        }

        protected void SetTexture(string textureName)
        {
            TextureName = textureName;
        }
    }
}