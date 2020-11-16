using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Zen.Assets;
using Zen.Assets.ExtensionMethods;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    public abstract class ControlWithSingleTexture : Control
    {
        #region State
        private string _textureName;
        public string TextureAtlas { get; private set; }

        public Color Color { get; set; }

        protected AtlasSpec2 Atlas { get; private set; }
        protected Texture2D Texture { get; set; }
        protected Rectangle SourceRectangle { get; private set; }
        #endregion

        protected ControlWithSingleTexture(string name, string textureName)
            : base(name)
        {
            if (textureName.HasValue())
            {
                TextureName = textureName;
            }

            Color = Color.White;
        }

        public string TextureName
        {
            get => _textureName;
            set
            {
                TextureAtlas = ControlHelper.GetTextureAtlas(value);
                _textureName = ControlHelper.GetTextureName(value);
            }
        }

        public override void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            if (TextureAtlas.HasValue())
            {
                Atlas = AssetsManager.Instance.GetAtlas(TextureAtlas);
                Texture = AssetsManager.Instance.GetTexture(TextureAtlas);
                SourceRectangle = Atlas.Frames[TextureName].ToRectangle();
            }
            else // no atlas
            {
                SetTexture(TextureName);
            }

            base.LoadContent(content, loadChildrenContent);
        }

        protected void SetTexture(string textureName)
        {
            if (Atlas.HasValue())
            {
                var f = Atlas.Frames[textureName];
                SourceRectangle = new Rectangle(f.X, f.Y, f.Width, f.Height);
            }
            else
            {
                if (textureName.HasValue())
                {
                    Texture = AssetsManager.Instance.GetTexture(textureName);
                    SourceRectangle = Texture.Bounds;
                }
            }
        }

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;
            SourceRectangle = Texture.Bounds;
        }
    }
}