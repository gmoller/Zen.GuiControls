using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Zen.Input;

namespace Zen.GuiControls.PackagesClasses
{
    public class KeyPressed : IPackage
    {
        private readonly Action<object, EventArgs> _action;
        private readonly float _delayTimeBetweenSameKeyPressed1 = 125.0f;
        private readonly float _delayTimeBetweenSameKeyPressed2 = 75.0f;
        private Keys[] _keysPressedInLastFrame;
        private double _lastTimeKeyHandled;
        private int _count;

        public KeyPressed()
        {
            _action = null;
            Reset();
        }

        public KeyPressed(Action<object, EventArgs> action)
        {
            _action = action;
            Reset();
        }

        public void Reset()
        {
            _keysPressedInLastFrame = new Keys[0];
            _lastTimeKeyHandled = 0.0d;
            _count = 0;
        }

        public ControlStatus Update(IControl control, InputHandler input, GameTime gameTime)
        {
            if (input.KeyPressed && control.Status.HasFlag(ControlStatus.HasFocus))
            {
                var keys = input.Keyboard.Keys;
                foreach (var currentKey in keys)
                {
                    var handleKey = false;
                    // if we have pressed the same key twice, wait some time before adding it again
                    if (_keysPressedInLastFrame.Contains(currentKey))
                    {
                        var delayTimeBetweenSameKeyPressed = _count == 0 ? _delayTimeBetweenSameKeyPressed1 : _delayTimeBetweenSameKeyPressed2;

                        if (gameTime.TotalGameTime.TotalMilliseconds - _lastTimeKeyHandled > delayTimeBetweenSameKeyPressed)
                        {
                            _count++;
                            handleKey = true;
                        }
                    }
                    // if we press a new key, add it
                    else
                    {
                        _count = 0;
                        handleKey = true;
                    }

                    if (handleKey)
                    {
                        HandleKey(control, input, currentKey, gameTime);
                    }
                }

                _keysPressedInLastFrame = keys;

                return control.Status;
            }

            // no keys pressed or control does not have focus
            Reset();

            return control.Status;
        }

        private void HandleKey(IControl control, InputHandler input, Keys currentKey, GameTime gameTime)
        {
            var e = new KeyboardEventArgs(input.Keyboard, currentKey, null, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
            _action?.Invoke(control, e);

            _lastTimeKeyHandled = gameTime.TotalGameTime.TotalMilliseconds;
        }
    }
}