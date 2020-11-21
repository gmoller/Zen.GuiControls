using System;
using Microsoft.Xna.Framework;

namespace Zen.GuiControls
{
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
    }
}