using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Input;
using OpenTK;

namespace GameEngine.Managers
{
    //Ben Mullenger
    public enum ButtonPosition
    {
        PressedOnce,
        Held,
        Released,
        UnPressed
    }

    public static class InputManager
    {
        public delegate void OnInputAction();
        private static Dictionary<MouseButton, Dictionary<ButtonPosition, OnInputAction>> _mouseActions = new Dictionary<MouseButton, Dictionary<ButtonPosition, OnInputAction>>();
        private static Dictionary<Key, Dictionary<ButtonPosition, OnInputAction>> _keyboardActions = new Dictionary<Key, Dictionary<ButtonPosition, OnInputAction>>();
        private static OnInputAction _onMouseMove;
        private static OnInputAction _onMouseStill;

        private static KeyboardState _oldKeys = new KeyboardState(), _curKeys;
        private static MouseState _oldMouse = new MouseState(), _curMouse;

        public static bool IsMouseVisible
        {
            get
            {
                return MyGame.CursorVisible;
            }
            set
            {
                MyGame.CursorVisible = value;
            }
        }
        public static bool LockMouseToScreen
        {
            get;
            set;
        }

        private static Vector2 _mouseCoords;
        public static Vector2 MouseCoords
        {
            get;
            private set;
        }
        public static Vector2 MouseChange
        {
            get;
            private set;
        }
        public static GameWindow MyGame { get; private set; }

        private static Vector2 _oldMouseCoords = Vector2.Zero;
        

        public static void Initialise(GameWindow myGame)
        {
            MyGame = myGame;
            _oldMouseCoords = new Vector2(MyGame.Width / 2 + MyGame.X, MyGame.Height / 2 + MyGame.Y);
        }

        public static void Update()
        {
            _mouseCoords = new Vector2(Mouse.GetCursorState().X, Mouse.GetCursorState().Y);
            MouseCoords = new Vector2(MyGame.Mouse.X, MyGame.Mouse.Y);
            _curKeys = Keyboard.GetState();
            _curMouse = Mouse.GetState();

            

            for(int i = 0; i < _keyboardActions.Count; i++)
            {
                var k = _keyboardActions.ElementAt(i);
                if (_curKeys.IsKeyDown(k.Key))
                {
                    if (k.Value.ContainsKey(ButtonPosition.PressedOnce) && _oldKeys.IsKeyUp(k.Key))
                    {
                        k.Value[ButtonPosition.PressedOnce]?.Invoke();
                    }
                    else if (k.Value.ContainsKey(ButtonPosition.Held) && _oldKeys.IsKeyDown(k.Key))
                    {
                        k.Value[ButtonPosition.Held]?.Invoke();
                    }
                }
                else
                {
                    if (k.Value.ContainsKey(ButtonPosition.Released) && _oldKeys.IsKeyDown(k.Key))
                    {
                        k.Value[ButtonPosition.Released]?.Invoke();
                    }
                    else if (k.Value.ContainsKey(ButtonPosition.UnPressed) && _oldKeys.IsKeyUp(k.Key))
                    {
                        k.Value[ButtonPosition.UnPressed]?.Invoke();
                    }
                }
                
            }

            foreach (var m in _mouseActions)
            {
                if (_curMouse.IsButtonDown(m.Key))
                {
                    if (m.Value.ContainsKey(ButtonPosition.PressedOnce) && _oldMouse.IsButtonUp(m.Key))
                    {
                        m.Value[ButtonPosition.PressedOnce]?.Invoke();
                    }
                    else if (m.Value.ContainsKey(ButtonPosition.Held) && _oldMouse.IsButtonDown(m.Key))
                    {
                        m.Value[ButtonPosition.Held]?.Invoke();
                    }
                }
                else
                {
                    if (m.Value.ContainsKey(ButtonPosition.Released) && _oldMouse.IsButtonDown(m.Key))
                    {
                        m.Value[ButtonPosition.Released]?.Invoke();
                    }
                    else if (m.Value.ContainsKey(ButtonPosition.UnPressed) && _oldMouse.IsButtonUp(m.Key))
                    {
                        m.Value[ButtonPosition.UnPressed]?.Invoke();
                    }
                }
            }

            if (_oldMouseCoords != _mouseCoords)
            {
                _onMouseMove?.Invoke();
            }
            else
            {
                _onMouseStill?.Invoke();
            }
            if (LockMouseToScreen)
            {
                _oldMouseCoords = new Vector2(MyGame.Width / 2 + MyGame.X, MyGame.Height / 2 + MyGame.Y);
                Mouse.SetPosition(_oldMouseCoords.X, _oldMouseCoords.Y);
            }
            MouseChange = _mouseCoords - _oldMouseCoords;
            _oldKeys = _curKeys;
            _oldMouse = _curMouse;
        }

        public static void AddMouseMoveTrigger(OnInputAction onInputAction)
        {
            _onMouseMove += onInputAction;
        }

        public static void RemoveMouseMoveTrigger(OnInputAction onInputAction)
        {
            _onMouseMove -= onInputAction;
        }

        public static void AddMouseStillTrigger(OnInputAction onInputAction)
        {
            _onMouseStill += onInputAction;
        }

        public static void RemoveMouseStillTrigger(OnInputAction onInputAction)
        {
            _onMouseStill -= onInputAction;
        }

        public static void AddMouseTrigger(MouseButton mouseButton, ButtonPosition buttonPosition, OnInputAction onInputAction)
        {
            if (!_mouseActions.ContainsKey(mouseButton))
            {
                _mouseActions.Add(mouseButton, new Dictionary<ButtonPosition, OnInputAction>());
            }
            if (!_mouseActions[mouseButton].ContainsKey(buttonPosition))
            {
                _mouseActions[mouseButton].Add(buttonPosition, onInputAction);
            }
            else
            {
                _mouseActions[mouseButton][buttonPosition] += onInputAction;
            }
        }

        public static void RemoveMouseTrigger(MouseButton mouseButton, ButtonPosition buttonPosition, OnInputAction onInputAction)
        {
            if (_mouseActions.ContainsKey(mouseButton))
            {
                if (_mouseActions[mouseButton].ContainsKey(buttonPosition))
                {
                    _mouseActions[mouseButton][buttonPosition] -= onInputAction;
                    if (_mouseActions[mouseButton][buttonPosition] == null)
                    {
                        _mouseActions[mouseButton].Remove(buttonPosition);
                        if (_mouseActions[mouseButton].Count == 0)
                        {
                            _mouseActions.Remove(mouseButton);
                        }
                    }
                }
            }
        }

        public static void AddKeyboardTrigger(Key key, ButtonPosition buttonPosition, OnInputAction onInputAction)
        {
            if (!_keyboardActions.ContainsKey(key))
            {
                _keyboardActions.Add(key, new Dictionary<ButtonPosition, OnInputAction>());
            }
            if (!_keyboardActions[key].ContainsKey(buttonPosition))
            {
                _keyboardActions[key].Add(buttonPosition, onInputAction);
            }
            else
            {
                _keyboardActions[key][buttonPosition] += onInputAction;
            }
        }

        public static void RemoveKeyboardTrigger(Key key, ButtonPosition buttonPosition, OnInputAction onInputAction)
        {
            if (_keyboardActions.ContainsKey(key))
            {
                if (_keyboardActions[key].ContainsKey(buttonPosition))
                {
                    _keyboardActions[key][buttonPosition] -= onInputAction;
                    if (_keyboardActions[key][buttonPosition] == null)
                    {
                        _keyboardActions[key].Remove(buttonPosition);
                        if (_keyboardActions[key].Count == 0)
                        {
                            _keyboardActions.Remove(key);
                        }
                    }
                }
            }
        }
    }
}
