using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public readonly struct Texture
    {
        public string TextureString { get; }
        public Func<bool> IsValid { get; }
        public Func<Rectangle> DestinationRectangle { get; }

        public Texture(string textureString, Func<bool> isValid, Func<Rectangle> destinationRectangle)
        {
            TextureString = textureString;
            IsValid = isValid;
            DestinationRectangle = destinationRectangle;
        }

        public override string ToString()
        {
            return DebuggerDisplay;
        }

        public string DebuggerDisplay => $"{{TextureString={TextureString}}}";
    }
}