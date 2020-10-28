using System;
using Zen.Input;

namespace Zen.GuiControls.PackagesClasses
{
    public class ControlClick : IPackage
    {
        private const float CLICK_COOLDOWN_TIME_IN_MILLISECONDS = 100.0f;

        private float _currentCooldownTimeInMilliseconds;

        private readonly Action<object, EventArgs> _action;

        public ControlClick(Action<object, EventArgs> action)
        {
            _action = action;
        }

        public void Reset()
        {
            _currentCooldownTimeInMilliseconds = 0.0f;
        }

        public ControlStatus Process(IControl control, InputHandler input, float deltaTime)
        {
            ControlStatus returnStatus = control.Status;

            if (control.Status == ControlStatus.Active)
            {
                _currentCooldownTimeInMilliseconds -= deltaTime;
                if (_currentCooldownTimeInMilliseconds <= 0.0f)
                {
                    returnStatus = OnClickComplete();
                }
            }

            if (control.Status != ControlStatus.Active)
            {
                if (control.Status == ControlStatus.MouseOver)
                {
                    if (input.IsLeftMouseButtonReleased)
                    {
                        returnStatus = OnClick(control, new MouseEventArgs(input.Mouse, null, deltaTime));
                    }
                }
            }

            return returnStatus;
        }

        private ControlStatus OnClickComplete()
        {
            _currentCooldownTimeInMilliseconds = 0.0f;

            return ControlStatus.None;
        }

        private ControlStatus OnClick(IControl control, EventArgs e)
        {
            _currentCooldownTimeInMilliseconds = CLICK_COOLDOWN_TIME_IN_MILLISECONDS;
            _action.Invoke(control, e);

            return ControlStatus.Active;
        }
    }
}