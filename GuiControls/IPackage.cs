using Zen.Input;

namespace Zen.GuiControls
{
    public interface IPackage
    {
        void Reset();
        ControlStatus Process(IControl control, InputHandler input, float deltaTime);
    }
}