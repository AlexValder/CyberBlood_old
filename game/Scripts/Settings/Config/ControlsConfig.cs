using System.Collections.Generic;
using CyberBlood.Scripts.Utils;
using Godot;
using Serilog;

namespace CyberBlood.Scripts.Settings.Config;

public class ControlsConfig : FileConfig {
    private const string CAMERA_CENTER = "camera_center";
    private const string ROTATE_H = "rotate_horizontal";
    private const string ROTATE_V = "rotate_vertical";
    private const string CAMERA_UP = "camera_up";
    private const string CAMERA_LEFT = "camera_left";
    private const string CAMERA_DOWN = "camera_down";
    private const string CAMERA_RIGHT = "camera_right";
    private const string INVERTED = "inverted";
    private const string GAMEPAD = "joypad";
    private const string MOUSE_KEYBOARD = "mouse_keyboard";
    private const string MOVE_LEFT = "move_left";
    private const string MOVE_RIGHT = "move_right";
    private const string MOVE_FORWARD = "move_forward";
    private const string MOVE_BACK = "move_back";

    #region Joybad

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
    
    public MouseKeyboardButton CameraCenter {
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

    #endregion

    public ControlsConfig(
        string path,
        string pass,
        bool useDefaults
    ) : base(path, pass, useDefaults, new Dictionary<string, Dictionary<string, object>> {
        [GAMEPAD] = new() {
            [ROTATE_H] = 2f,
            [ROTATE_V] = 2f,
            [INVERTED] = false,
        },
        [MOUSE_KEYBOARD] = new() {
            [ROTATE_H]      = .005f,
            [ROTATE_V]      = .005f,
            [MOVE_FORWARD]  = KeyList.W,
            [MOVE_LEFT]     = KeyList.A,
            [MOVE_BACK]     = KeyList.S,
            [MOVE_RIGHT]    = KeyList.D,
            [CAMERA_CENTER] = MouseButtons.MiddleButton,
        }
    }) { }

    public override void ApplySettings() {
        var cameras = new List<string> {
            CAMERA_UP, CAMERA_LEFT, CAMERA_DOWN, CAMERA_RIGHT
        };
        var actions = InputMap.GetActions();
        foreach (string action in actions) {
            if (cameras.Contains(action) || action.BeginsWith("ui_") || action.Contains("pause")) {
                continue;
            }
            
            InputMap.ActionEraseEvents(action);
            InputMap.EraseAction(action);
        }

        AddMouseKeyboardActions();
        AddGamepadActions();
    }
    
    private void AddMouseKeyboardActions() {
        CheckAddAction(CAMERA_CENTER, CameraCenter);
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
        switch (name) {
            case CAMERA_CENTER:
                CameraCenter = new MouseKeyboardButton(e);
                break;
            case MOVE_FORWARD:
                MoveForward = new MouseKeyboardButton(e);
                break;
            case MOVE_LEFT:
                MoveLeft = new MouseKeyboardButton(e);
                break;
            case MOVE_BACK:
                MoveBack = new MouseKeyboardButton(e);
                break;
            case MOVE_RIGHT:
                MoveRight = new MouseKeyboardButton(e);
                break;
            default:
                Log.Logger.Warning("??? {Name}", name);
                break;
        }
    }

    private static void AddGamepadActions() {
        // TODO
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
