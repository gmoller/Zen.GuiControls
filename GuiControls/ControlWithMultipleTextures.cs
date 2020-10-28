using Microsoft.Xna.Framework.Graphics;
using Zen.Assets.ExtensionMethods;
using Zen.Input;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    public abstract class ControlWithMultipleTextures : ControlWithSingleTexture
    {
        #region State
        private string TextureNormal { get; }
        private string TextureActive { get; }
        private string TextureHover { get; }
        private string TextureDisabled { get; }
        #endregion

        protected ControlWithMultipleTextures(string name, string textureName, string textureNormal, string textureActive, string textureHover, string textureDisabled)
            : base(name, textureName)
        {
            TextureNormal = textureNormal.HasValue() ? textureNormal.KeepOnlyAfterCharacter('.') : TextureName;
            TextureActive = textureActive.HasValue() ? textureActive.KeepOnlyAfterCharacter('.') : TextureName;
            TextureHover = textureHover.HasValue() ? textureHover.KeepOnlyAfterCharacter('.') : TextureName;
            TextureDisabled = textureDisabled.HasValue() ? textureDisabled.KeepOnlyAfterCharacter('.') : TextureName;
        }

        public override void Update(InputHandler input, float deltaTime, Viewport? viewport = null)
        {
            base.Update(input, deltaTime, viewport);

            if (Status == ControlStatus.Active)
            {
                SetTexture(TextureActive);
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