using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Zen.Input;

namespace Zen.GuiControls
{
    public class ControlWithMultipleTextures : ControlWithSingleTexture
    {
        #region State
        public Dictionary<string, string> TextureStrings { get; }
        public Dictionary<string, string> Dictionary { get; set; }
        #endregion

        protected ControlWithMultipleTextures(string name) : base(name)
        {
            TextureStrings = new Dictionary<string, string>();
        }

        protected ControlWithMultipleTextures(ControlWithMultipleTextures other) : base(other)
        {
            TextureStrings = other.TextureStrings;
            Dictionary = other.Dictionary;
        }

        public override IControl Clone()
        {
            return new ControlWithMultipleTextures(this);
        }

        public void AddTexture(string textureName, string textureString)
        {
            // up-sert
            TextureStrings[textureName] = textureString;
        }

        public override void Update(InputHandler input, float deltaTime, Viewport? viewport = null)
        {
            base.Update(input, deltaTime, viewport);

            var key = $"{Status}-{Enabled}";
            var textureLookup = Dictionary[key];
            var textureName = TextureStrings[textureLookup];

            SetTexture(textureName);
        }
    }
}