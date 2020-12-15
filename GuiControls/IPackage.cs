using Microsoft.Xna.Framework;
using Zen.Input;

namespace Zen.GuiControls
{
    public interface IPackage
    {
        void Reset();
        ControlStatus Update(IControl control, InputHandler input, GameTime gameTime);
    }
}