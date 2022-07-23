using CyberBlood.Scripts.Settings;
using Godot;
using GodotCSToolbox;
using System.Collections.Generic;
using System.Diagnostics;
using CyberBlood.Scripts.Settings.Config.Gamepad;
using CyberBlood.Scripts.Utils;
using Serilog;
using GArray = Godot.Collections.Array;

namespace CyberBlood.Scenes.GUI.SettingsMenu {
    public class Controls : Control, IConfigMenu {
        public bool NeedsConfirmation => false;

        private bool _wasChanged;
        private bool _sticksSwitched;
        private readonly Color _redColor = new (.7f, .07f, 0.07f);
        private readonly Dictionary<string, (Button, VBoxContainer)> _tabs = new();
        private readonly Dictionary<string, Button> _keyboardButtons = new();
        private readonly Dictionary<string, Button> _gamepadButtons = new();
#pragma warning disable CS0649
        [NodePath("hbox/panel/viewer/center/keyboard/hsens/hbox/hslider")]
        private Slider _mouseHSensitivity;
        [NodePath("hbox/panel/viewer/center/keyboard/vsens/hbox/hslider")]
        private Slider _mouseVSensitivity;
        [NodePath("hbox/panel/viewer/center/gamepad/grid1/hsens/hslider")]
        private Slider _stickHSensitivity;
        [NodePath("hbox/panel/viewer/center/gamepad/grid1/vsens/hslider")]
        private Slider _stickVSensitivity;
        [NodePath("hbox/panel/viewer/center/gamepad/grid1/inverted")]
        private CheckBox _invertedCheck;
        [NodePath("hbox/panel/viewer/center/gamepad/grid2/icon_set")]
        private OptionButton _iconThemeOptions;
#pragma warning restore CS0649

        public override void _Ready() {
            this.SetupNodeTools();

            PopulateTabs();
            ConnectTabButtons();
            ConnectControlKeys();
            ConnectControlGamepadButtons();
            SetupFromConfig();

            _tabs["gamepad"].Item1.Pressed  = GameSettings.JoyConnected;
            _tabs["keyboard"].Item1.Pressed = !GameSettings.JoyConnected;
        }

        private void PopulateTabs() {
            var buttons = GetNode("hbox/buttons");
            var panels  = GetNode("hbox/panel/viewer/center");
            var count   = buttons.GetChildCount();
            for (var i = 0; i < count; ++i) {
                var button = buttons.GetChild<Button>(i);
                var panel  = panels.GetChild<VBoxContainer>(i);
                _tabs[button.Name] = (button, panel);
            }
        }

        private void ConnectTabButtons() {
            foreach (var (button, _) in _tabs.Values) {
                button.Connect(
                    "toggled", this, nameof(HandleTabChange), new GArray { button.Name }
                );
            }
        }

        private void ConnectControlKeys() {
            var root    = GetNode<Control>("hbox/panel/viewer/center/keyboard");
            var confirm = GetNode<ConfirmationDialog>("Confirm");
            var count   = root.GetChildCount();
            for (var i = 0; i < count; ++i) {
                var child = root.GetChild<Control>(i);
                if (child is not HBoxContainer hbox) {
                    continue;
                }

                var button = hbox.GetChild<Control>(1);
                if (button is Slider slider) {
                    slider.Connect("value_changed", this, nameof(SetWasChangedTrue));
                } else if (button is Button ctrlBtn) {
                    ctrlBtn.Connect("button_up", this, nameof(SetWasChangedTrue));
                    ctrlBtn.Connect(
                        "button_up", confirm, "ShowUp", new GArray {
                            child.Name, ctrlBtn.Text, Confirm.InputMode.MouseKeyboard
                        }
                    );
                    _keyboardButtons[child.Name] = ctrlBtn;
                }
            }
        }

        private void ConnectControlGamepadButtons() {
            var root            = GetNode<Control>("hbox/panel/viewer/center/gamepad");
            var confirm         = GetNode<ConfirmationDialog>("Confirm");
            var switchButton    = root.GetNode<Button>("grid0/switch");
            var leftStickLabel  = root.GetNode<Label>("grid0/label0");
            var rightStickLabel = root.GetNode<Label>("grid0/label2");

            switchButton.Connect(
                "button_up",
                this,
                nameof(SwapSticks),
                new GArray { leftStickLabel, rightStickLabel }
            );

            var subroot = root.GetNode<Control>("grid2");
            var count   = subroot.GetChildCount();
            for (var i = 0; i < count; ++i) {
                var child = subroot.GetChild<Control>(i);

                if (child is not Button btn) {
                    continue;
                }

                _gamepadButtons[btn.Name] = btn;
                btn.Connect("button_up", this, nameof(SetWasChangedTrue));
                if (btn is not OptionButton) {
                    btn.Connect(
                        "button_up", confirm, "ShowUp", new GArray {
                            child.Name, btn.Text, Confirm.InputMode.Gamepad
                        }
                    );
                }
            }
        }

        private void SwapSticks(Label left, Label right) {
            (left.Text, right.Text) = (right.Text, left.Text);
            _sticksSwitched         = !_sticksSwitched;
            _wasChanged             = true;
        }

        private void _on_Confirm_ControlSelected(string name, InputEvent action, Confirm.InputMode mode) {
            Debug.Assert(InputMap.HasAction(name));

            if (action == null) {
                return;
            }

            GameSettings.Controls.SetAction(name, action);
            if (mode == Confirm.InputMode.MouseKeyboard) {
                SetText(_keyboardButtons[name], action);
            } else if (action is InputEventJoypadButton jb) {
                var btn = _gamepadButtons[name];
                btn.Text = GamepadButtonMetaSelector.GetName(jb.ButtonIndex);
                btn.Icon = GamepadButtonMetaSelector.GetTexture(jb.ButtonIndex);
                btn.Update();
            }

            CheckRepeatingButtons();
        }

        private void CheckRepeatingButtons() {
            const string customFontColor      = "custom_colors/font_color";
            const string customFontFocusColor = "custom_colors/font_color_focus";

            var buttons = new List<Button>(_keyboardButtons.Values.Count);
            foreach (var button in _keyboardButtons.Values) {
                button.Set(customFontColor, null);
                button.Set(customFontFocusColor, null);
                buttons.Add(button);
            }

            for (var i = 0; i < buttons.Count; ++i)
            for (var j = i + 1; j < buttons.Count; ++j) {
                if (!buttons[i].Text.Equals(buttons[j].Text)) {
                    continue;
                }

                buttons[i].Set(customFontColor, _redColor);
                buttons[i].Set(customFontFocusColor, _redColor);
                buttons[j].Set(customFontColor, _redColor);
                buttons[j].Set(customFontFocusColor, _redColor);
            }
        }

        private static void SetText(Button button, InputEvent action) {
            switch (action) {
                case InputEventMouseButton:
                case InputEventKey:
                    button.Text = new MouseKeyboardButton(action).ToString();
                    break;
                default:
                    Log.Logger.Warning("Unknown type: {Type}", action.GetType());
                    button.Text = action.AsText();
                    break;
            }
        }

        private void HandleTabChange(bool toggle, string name) {
            _tabs[name].Item2.Visible = toggle;
        }

        public void SetupFromConfig() {
            _wasChanged = false;
            var ctrl = GameSettings.Controls;
            // mouse+keyboard
            _keyboardButtons["move_forward"].Text  = ctrl.MoveForward.ToString();
            _keyboardButtons["move_left"].Text     = ctrl.MoveLeft.ToString();
            _keyboardButtons["move_back"].Text     = ctrl.MoveBack.ToString();
            _keyboardButtons["move_right"].Text    = ctrl.MoveRight.ToString();
            _keyboardButtons["camera_center"].Text = ctrl.CameraMouseCenter.ToString();
            _keyboardButtons["jump"].Text          = ctrl.MouseKeyboardJump.ToString();
            _keyboardButtons["screenshot"].Text    = ctrl.TakeScreenshot.ToString();
            _keyboardButtons["lockon"].Text        = ctrl.MouseLockon.ToString();
            _mouseHSensitivity.Value               = ctrl.CameraMouseRotateHorizontal / ctrl.CameraMouseDenominator;
            _mouseVSensitivity.Value               = ctrl.CameraMouseRotateVertical / ctrl.CameraMouseDenominator;

            // gamepad
            _sticksSwitched                       = ctrl.SticksSwitched;
            _stickHSensitivity.Value              = ctrl.CameraJoyRotateHorizontal / ctrl.CameraJoyDenominator;
            _stickVSensitivity.Value              = ctrl.CameraJoyRotateVertical / ctrl.CameraJoyDenominator;
            _iconThemeOptions.Selected            = (int)ctrl.GamepadButtonTheme;
            _invertedCheck.Pressed                = ctrl.CameraInverted;
            _gamepadButtons["camera_center"].Text = GamepadButtonMetaSelector.GetName(ctrl.CameraJoyCenter);
            _gamepadButtons["camera_center"].Icon = GamepadButtonMetaSelector.GetTexture(ctrl.CameraJoyCenter);
            _gamepadButtons["jump"].Text          = GamepadButtonMetaSelector.GetName(ctrl.JoyJump);
            _gamepadButtons["jump"].Icon          = GamepadButtonMetaSelector.GetTexture(ctrl.JoyJump);
            _gamepadButtons["lockon"].Text        = GamepadButtonMetaSelector.GetName(ctrl.JoyLockon);
            _gamepadButtons["lockon"].Icon        = GamepadButtonMetaSelector.GetTexture(ctrl.JoyLockon);

            foreach (var b in _gamepadButtons.Values) {
                b.Update();
            }
            CheckRepeatingButtons();
        }

        public void ApplyCurrentSettings() {
            if (!_wasChanged) {
                return;
            }

            _wasChanged = false;

            var ctrl = GameSettings.Controls;

            ctrl.CameraMouseRotateHorizontal = (float)(_mouseHSensitivity.Value * ctrl.CameraMouseDenominator);
            ctrl.CameraMouseRotateVertical   = (float)(_mouseVSensitivity.Value * ctrl.CameraMouseDenominator);

            ctrl.SticksSwitched            = _sticksSwitched;
            ctrl.CameraJoyRotateHorizontal = (float)(_stickHSensitivity.Value * ctrl.CameraJoyDenominator);
            ctrl.CameraJoyRotateVertical   = (float)(_stickVSensitivity.Value * ctrl.CameraJoyDenominator);
            ctrl.GamepadButtonTheme        = (ButtonTheme)_iconThemeOptions.Selected;
            ctrl.CameraInverted            = _invertedCheck.Pressed;

            ctrl.SaveConfigToFile();
            ctrl.ApplySettings();
            SetupFromConfig();
        }

        public void SetDefaults() {
            _wasChanged = true;
            GameSettings.Controls.SetDefaults();
            SetupFromConfig();
        }

        private void SetWasChangedTrue(bool _) => SetWasChangedTrue();

        private void SetWasChangedTrue() {
            _wasChanged = true;
        }
    }
}
