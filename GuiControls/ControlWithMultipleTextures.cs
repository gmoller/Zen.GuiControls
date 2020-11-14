using Microsoft.Xna.Framework.Graphics;
using Zen.Assets.ExtensionMethods;
using Zen.Input;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    public abstract class ControlWithMultipleTextures : ControlWithSingleTexture
    {
        private string _textureNormal;
        private string _textureActive;
        private string _textureHover;
        private string _textureDisabled;

        #region State

        public string TextureNormal
        {
            get => _textureNormal;
            set => _textureNormal = value.HasValue() ? value.KeepOnlyAfterCharacter('.') : TextureName;
        }

        public string TextureActive
        {
            get => _textureActive;
            set => _textureActive = value.HasValue() ? value.KeepOnlyAfterCharacter('.') : TextureName;
        }

        public string TextureHover
        {
            get => _textureHover;
            set => _textureHover = value.HasValue() ? value.KeepOnlyAfterCharacter('.') : TextureName;
        }

        public string TextureDisabled
        {
            get => _textureDisabled;
            set => _textureDisabled = value.HasValue() ? value.KeepOnlyAfterCharacter('.') : TextureName;
        }

        #endregion

        protected ControlWithMultipleTextures(string name, string textureName, string textureNormal, string textureActive, string textureHover, string textureDisabled)
            : base(name, textureName)
        {
            TextureNormal = textureNormal;
            TextureActive = textureActive;
            TextureHover = textureHover;
            TextureDisabled = textureDisabled;
        }

        public override void Update(InputHandler input, float deltaTime, Viewport? viewport = null)
        {
            base.Update(input, deltaTime, viewport);

            if (Status == ControlStatus.Active)
            {
                SetTexture(TextureActive);
                return;
            }

            string texture;
            if (Enabled)
            {
                if (Status == ControlStatus.MouseOver)
                {
                    //if (Atlas.IsNull() && !TextureHover.HasValue()) throw new Exception("_textureHover is empty!");
                    texture = Atlas.HasValue() ? (TextureHover.HasValue() ? TextureHover : TextureName) : TextureHover;
                }
                else
                {
                    //if (Atlas.IsNull() && !TextureNormal.HasValue()) throw new Exception("_textureNormal is empty!");
                    texture = Atlas.HasValue() ? (TextureNormal.HasValue() ? TextureNormal : TextureName) : TextureNormal;
                }
            }
            else
            {
                //if (Atlas.IsNull() && !TextureDisabled.HasValue()) throw new Exception("_textureDisabled is empty!");
                texture = Atlas.HasValue() ? (TextureDisabled.HasValue() ? TextureDisabled : TextureName) : TextureDisabled;
            }
            SetTexture(texture);
        }
    }
}