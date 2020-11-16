﻿using System;
using Zen.Input;

namespace Zen.GuiControls.PackagesClasses
{
    public class ControlDrag : IPackage
    {
        private readonly Action<object, EventArgs> _action;

        public ControlDrag()
        {
            _action = null;
        }

        public ControlDrag(Action<object, EventArgs> action)
        {
            _action = action;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public ControlStatus Update(IControl control, InputHandler input, float deltaTime)
        {
            ControlStatus returnStatus = control.Status;

            if (control.Status == ControlStatus.MouseOver && input.IsLeftMouseButtonDown && input.MouseHasMoved)
            {
                OnDrag(control, new MouseEventArgs(input.Mouse, null, deltaTime));
            }

            return returnStatus;
        }

        private void OnDrag(IControl control, EventArgs e)
        {
            _action.Invoke(control, e);
        }
    }
}