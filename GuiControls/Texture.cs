using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public readonly struct Texture
    {
        public string TextureName { get; }
        public string TextureString { get; }
        public Func<IControl, bool> IsValid { get; }
        public Func<IControl, Rectangle> DestinationRectangle { get; }

        public Texture(string textureName, string textureString, Func<IControl, bool> isValid, Func<IControl, Rectangle> destinationRectangle)
        {
            TextureName = textureName;
            TextureString = textureString;
            IsValid = isValid;
            DestinationRectangle = destinationRectangle;
        }

        public override string ToString()
        {
            return DebuggerDisplay;
        }

        public string DebuggerDisplay => $"{{TextureName={TextureName},TextureString={TextureString}}}";
    }
}