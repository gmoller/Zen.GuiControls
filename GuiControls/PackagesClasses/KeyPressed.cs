using System;
using Zen.Input;

namespace Zen.GuiControls.PackagesClasses
{
    public class KeyPressed : IPackage
    {
        private readonly Action<object, EventArgs> _action;

        public KeyPressed()
        {
            _action = null;
        }

        public KeyPressed(Action<object, EventArgs> action)
        {
            _action = action;
        }

        public void Reset()
        {
        }

        public ControlStatus Update(IControl control, InputHandler input, float deltaTime)
        {
            if (input.KeyPressed && control.Status.HasFlag(ControlStatus.HasFocus))
            {
                OnKeyPressed(control, new KeyboardEventArgs(input.Keyboard, input.Keyboard.Keys[0], null, deltaTime));
            }

            return control.Status;
        }

        private void OnKeyPressed(IControl control, EventArgs e)
        {
            _action?.Invoke(control, e);
        }
    }
}