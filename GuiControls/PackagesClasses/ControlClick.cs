using System;
using Zen.Input;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls.PackagesClasses
{
    public class ControlClick : IPackage
    {
        private const float CLICK_COOLDOWN_TIME_IN_MILLISECONDS = 100.0f;

        private float _currentCooldownTimeInMilliseconds;

        private readonly Action<object, EventArgs> _action;

        public ControlClick()
        {
            _action = null;
        }

        public ControlClick(Action<object, EventArgs> action)
        {
            _action = action;
        }

        public void Reset()
        {
            _currentCooldownTimeInMilliseconds = 0.0f;
        }

        public ControlStatus Update(IControl control, InputHandler input, float deltaTime)
        {
            if (control.Status.HasFlag(ControlStatus.Active))
            {
                _currentCooldownTimeInMilliseconds -= deltaTime;
                if (_currentCooldownTimeInMilliseconds <= 0.0f)
                {
                    return OnClickComplete(control.Status);
                }

                return control.Status;
            }

            if (!input.IsLeftMouseButtonPressed) return control.Status;

            // left mouse button clicked
            if (control.Status.HasFlag(ControlStatus.MouseOver))
            {
                var returnStatus = OnClick(control, new MouseEventArgs(input.Mouse, null, deltaTime));

                return returnStatus;
            }

            // clicked off the control
            if (!control.Status.HasFlag(ControlStatus.HasFocus)) return control.Status;

            // so remove focus
            var controlStatusAsInt = (int)control.Status;
            controlStatusAsInt = controlStatusAsInt.UnsetBit(ControlStatus.HasFocus.GetIndexOfEnumeration());

            return (ControlStatus)controlStatusAsInt;

        }

        private ControlStatus OnClickComplete(ControlStatus controlStatus)
        {
            _currentCooldownTimeInMilliseconds = 0.0f;

            var controlStatusAsInt = (int)controlStatus;
            controlStatusAsInt = controlStatusAsInt.UnsetBit(ControlStatus.Active.GetIndexOfEnumeration());

            return (ControlStatus)controlStatusAsInt;
        }

        private ControlStatus OnClick(IControl control, EventArgs e)
        {
            _currentCooldownTimeInMilliseconds = CLICK_COOLDOWN_TIME_IN_MILLISECONDS;
            _action?.Invoke(control, e);

            var controlStatusAsInt = (int)control.Status;
            controlStatusAsInt = controlStatusAsInt.SetBit(ControlStatus.Active.GetIndexOfEnumeration());

            return (ControlStatus)controlStatusAsInt;
        }
    }
}