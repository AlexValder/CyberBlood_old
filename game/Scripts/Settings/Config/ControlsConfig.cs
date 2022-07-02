using System.Collections.Generic;
using CyberBlood.Scripts.Settings.Config.Gamepad;
using CyberBlood.Scripts.Utils;
using Godot;
using Serilog;

namespace CyberBlood.Scripts.Settings.Config {
    public class ControlsConfig : FileConfig {
        private const string GAMEPAD = "joypad";
        private const string STICKS_SWITCHED = "sticks_switched";
        private const string BUTTON_THEME = "button_theme";
        private const string CAMERA_CENTER = "camera_center";
        private const string ROTATE_H = "rotate_horizontal";
        private const string ROTATE_V = "rotate_vertical";
        private const string CAMERA_UP = "camera_up";
        private const string CAMERA_LEFT = "camera_left";
        private const string CAMERA_DOWN = "camera_down";
        private const string CAMERA_RIGHT = "camera_right";
        private const string INVERTED = "inverted";
        private const string JUMP = "jump";
        private const string MOUSE_KEYBOARD = "mouse_keyboard";
        private const string MOVE_LEFT = "move_left";
        private const string MOVE_RIGHT = "move_right";
        private const string MOVE_FORWARD = "move_forward";
        private const string MOVE_BACK = "move_back";

        #region Joybad
        public float CameraJoyDenominator => .02f;

        public float CameraJoyRotateHorizontal {
            get => GetValue<float>(GAMEPAD, ROTATE_H);
            set => SetValue(GAMEPAD, ROTATE_H, value);
        }

        public float CameraJoyRotateVertical {
            get => GetValue<float>(GAMEPAD, ROTATE_V);
            set => SetValue(GAMEPAD, ROTATE_V, value);
        }

        public bool CameraInverted {
            get => GetValue<bool>(GAMEPAD, INVERTED);
            set => SetValue(GAMEPAD, INVERTED, value);
        }

        public bool SticksSwitched {
            get => GetValue<bool>(GAMEPAD, STICKS_SWITCHED);
            set {
                SetValue(GAMEPAD, STICKS_SWITCHED, value);
                SetSticksActions(switched: value);
            }
        }

        public JoystickList CameraJoyCenter {
            get => (JoystickList)GetValue<int>(GAMEPAD, CAMERA_CENTER);
            private set => SetValue(GAMEPAD, CAMERA_CENTER, (int)value);
        }

        public JoystickList JoyJump {
            get => (JoystickList)GetValue<int>(GAMEPAD, JUMP);
            private set => SetValue(GAMEPAD, JUMP, (int)value);
        }

        public ButtonTheme GamepadButtonTheme {
            get => (ButtonTheme)GetValue<int>(GAMEPAD, BUTTON_THEME);
            set {
                SetValue(GAMEPAD, BUTTON_THEME, (int)value);
                GamepadButtonMetaSelector.Theme = value;
            }
        }

        #endregion

        #region Mouse+Keyboard

        public float CameraMouseDenominator => 0.005f;

        public float CameraMouseRotateHorizontal {
            get => GetValue<float>(MOUSE_KEYBOARD, ROTATE_H);
            set => SetValue(MOUSE_KEYBOARD, ROTATE_H, value);
        }

        public float CameraMouseRotateVertical {
            get => GetValue<float>(MOUSE_KEYBOARD, ROTATE_V);
            set => SetValue(MOUSE_KEYBOARD, ROTATE_V, value);
        }

        public MouseKeyboardButton MoveForward {
            get {
                var value = GetValue<int>(MOUSE_KEYBOARD, MOVE_FORWARD);
                return
                    value < 0 ? new MouseKeyboardButton((MouseButtons)value) : new MouseKeyboardButton((KeyList)value);
            }
            private set {
                if (value.KeyButton == 0) {
                    SetValue(MOUSE_KEYBOARD, MOVE_FORWARD, (int)value.MouseButtons);
                } else {
                    SetValue(MOUSE_KEYBOARD, MOVE_FORWARD, (int)value.KeyButton);
                }
            }
        }

        public MouseKeyboardButton MoveLeft {
            get {
                var value = GetValue<int>(MOUSE_KEYBOARD, MOVE_LEFT);
                return
                    value < 0 ? new MouseKeyboardButton((MouseButtons)value) : new MouseKeyboardButton((KeyList)value);
            }
            private set {
                if (value.KeyButton == 0) {
                    SetValue(MOUSE_KEYBOARD, MOVE_LEFT, (int)value.MouseButtons);
                } else {
                    SetValue(MOUSE_KEYBOARD, MOVE_LEFT, (int)value.KeyButton);
                }
            }
        }

        public MouseKeyboardButton MoveBack {
            get {
                var value = GetValue<int>(MOUSE_KEYBOARD, MOVE_BACK);
                return
                    value < 0 ? new MouseKeyboardButton((MouseButtons)value) : new MouseKeyboardButton((KeyList)value);
            }
            private set {
                if (value.KeyButton == 0) {
                    SetValue(MOUSE_KEYBOARD, MOVE_BACK, (int)value.MouseButtons);
                } else {
                    SetValue(MOUSE_KEYBOARD, MOVE_BACK, (int)value.KeyButton);
                }
            }
        }

        public MouseKeyboardButton MoveRight {
            get {
                var value = GetValue<int>(MOUSE_KEYBOARD, MOVE_RIGHT);
                return
                    value < 0 ? new MouseKeyboardButton((MouseButtons)value) : new MouseKeyboardButton((KeyList)value);
            }
            private set {
                if (value.KeyButton == 0) {
                    SetValue(MOUSE_KEYBOARD, MOVE_RIGHT, (int)value.MouseButtons);
                } else {
                    SetValue(MOUSE_KEYBOARD, MOVE_RIGHT, (int)value.KeyButton);
                }
            }
        }

        public MouseKeyboardButton CameraMouseCenter {
            get {
                var value = GetValue<int>(MOUSE_KEYBOARD, CAMERA_CENTER);
                return
                    value < 0 ? new MouseKeyboardButton((MouseButtons)value) : new MouseKeyboardButton((KeyList)value);
            }
            private set {
                if (value.KeyButton == 0) {
                    SetValue(MOUSE_KEYBOARD, CAMERA_CENTER, (int)value.MouseButtons);
                } else {
                    SetValue(MOUSE_KEYBOARD, CAMERA_CENTER, (int)value.KeyButton);
                }
            }
        }

        public MouseKeyboardButton MouseKeyboardJump {
            get {
                var value = GetValue<int>(MOUSE_KEYBOARD, JUMP);
                return
                    value < 0 ? new MouseKeyboardButton((MouseButtons)value) : new MouseKeyboardButton((KeyList)value);
            }
            private set {
                if (value.KeyButton == 0) {
                    SetValue(MOUSE_KEYBOARD, JUMP, (int)value.MouseButtons);
                } else {
                    SetValue(MOUSE_KEYBOARD, JUMP, (int)value.KeyButton);
                }
            }
        }

        #endregion

        public ControlsConfig(
            string path,
            string pass,
            bool useDefaults
        ) : base(path, pass, useDefaults, new Dictionary<string, Dictionary<string, object>> {
            [GAMEPAD] = new() {
                [BUTTON_THEME]    = (int)ButtonTheme.Xbox,
                [ROTATE_H]        = .02f,
                [ROTATE_V]        = .02f,
                [INVERTED]        = false,
                [STICKS_SWITCHED] = false,
                [CAMERA_CENTER]   = GamepadButtonEventFactory.IndexRight,
                [JUMP]            = GamepadButtonEventFactory.IndexA,
            },
            [MOUSE_KEYBOARD] = new() {
                [ROTATE_H]      = .005f,
                [ROTATE_V]      = .005f,
                [MOVE_FORWARD]  = KeyList.W,
                [MOVE_LEFT]     = KeyList.A,
                [MOVE_BACK]     = KeyList.S,
                [MOVE_RIGHT]    = KeyList.D,
                [CAMERA_CENTER] = MouseButtons.MiddleButton,
                [JUMP]          = KeyList.Space,
            }
        }) {
        }

        private static IEnumerable<string> CameraActions { get; } = new List<string> {
            CAMERA_UP, CAMERA_LEFT, CAMERA_DOWN, CAMERA_RIGHT
        };

        private static IEnumerable<string> MoveActions { get; } = new List<string> {
            MOVE_FORWARD, MOVE_LEFT, MOVE_BACK, MOVE_RIGHT
        };

        public override void ApplySettings() {
            var actions = InputMap.GetActions();
            foreach (string action in actions) {
                if (action.BeginsWith("ui_") || action.Contains("pause")) {
                    continue;
                }

                InputMap.ActionEraseEvents(action);
                InputMap.EraseAction(action);
            }

            GamepadButtonMetaSelector.Theme = GamepadButtonTheme;
            AddMouseKeyboardActions();
            AddGamepadActions();
        }

        private void AddMouseKeyboardActions() {
            CheckAddAction(JUMP, MouseKeyboardJump);
            CheckAddAction(CAMERA_CENTER, CameraMouseCenter);
            CheckAddAction(MOVE_FORWARD, MoveForward);
            CheckAddAction(MOVE_LEFT, MoveLeft);
            CheckAddAction(MOVE_BACK, MoveBack);
            CheckAddAction(MOVE_RIGHT, MoveRight);

            static void CheckAddAction(string action, MouseKeyboardButton ev) {
                if (!InputMap.HasAction(action)) {
                    InputMap.AddAction(action);
                }

                var input = ev.GetInputEvent();
                InputMap.ActionAddEvent(action, input);
            }
        }

        public void SetAction(string name, InputEvent e) {
            if (e is InputEventJoypadButton jb) {
                SetJoyAction(name, jb);
            } else {
                SetMouseKeyAction(name, new MouseKeyboardButton(e));
            }
            
        }

        private void SetJoyAction(string name, InputEventJoypadButton jb) {
            switch (name) {
                case CAMERA_CENTER:
                    CameraJoyCenter = (JoystickList)jb.ButtonIndex;
                    break;
                default:
                    Log.Logger.Warning("??? {Name}", name);
                    break;
            }
        }

        private void SetMouseKeyAction(string name, MouseKeyboardButton button) {
            switch (name) {
                case CAMERA_CENTER:
                    CameraMouseCenter = button;
                    break;
                case MOVE_FORWARD:
                    MoveForward = button;
                    break;
                case MOVE_LEFT:
                    MoveLeft = button;
                    break;
                case MOVE_BACK:
                    MoveBack = button;
                    break;
                case MOVE_RIGHT:
                    MoveRight = button;
                    break;
                default:
                    Log.Logger.Warning("??? {Name}", name);
                    break;
            }
        }

        private void AddGamepadActions() {
            SetSticksActions(SticksSwitched);

            SetButtons();
        }

        private void SetButtons() {
            CheckAddAction(JUMP, JoyJump);
            CheckAddAction(CAMERA_CENTER, CameraJoyCenter);

            static void CheckAddAction(string action, JoystickList index) {
                if (!InputMap.HasAction(action)) {
                    InputMap.AddAction(action);
                }
            
                InputMap.ActionAddEvent(action, GamepadButtonEventFactory.ForIndex(index));
            }
        }

        private static void SetSticksActions(bool switched) {
            var builder = new GamepadStickEventBuilder();

            SetCameraActions(!switched ? builder.RightStick() : builder.LeftStick());
            SetMovementActions(!switched ? builder.LeftStick() : builder.RightStick());
        }

        private static void SetMovementActions(IHasSticks builder) {
            foreach (var action in MoveActions) {
                if (!InputMap.HasAction(action)) {
                    InputMap.AddAction(action);
                }
            }

            InputMap.ActionAddEvent(MOVE_RIGHT, builder.Right().Build());
            InputMap.ActionAddEvent(MOVE_LEFT, builder.Left().Build());
            InputMap.ActionAddEvent(MOVE_BACK, builder.Down().Build());
            InputMap.ActionAddEvent(MOVE_FORWARD, builder.Up().Build());
        }

        private static void SetCameraActions(IHasSticks builder) {
            foreach (var action in CameraActions) {
                if (!InputMap.HasAction(action)) {
                    InputMap.AddAction(action);
                }
            }

            InputMap.ActionAddEvent(CAMERA_LEFT, builder.Right().Build());
            InputMap.ActionAddEvent(CAMERA_RIGHT, builder.Left().Build());
            InputMap.ActionAddEvent(CAMERA_UP, builder.Down().Build());
            InputMap.ActionAddEvent(CAMERA_DOWN, builder.Up().Build());
        }

        public static string MouseButtonString(int index) =>
            (MouseButtons)(-index) switch {
                MouseButtons.LeftButton   => "Left MB",
                MouseButtons.RightButton  => "Right MB",
                MouseButtons.MiddleButton => "Middle MB",
                MouseButtons.WheelUp      => "Wheel Up",
                MouseButtons.WheelDown    => "Wheel Down",
                _                         => "???"
            };
    }
}
