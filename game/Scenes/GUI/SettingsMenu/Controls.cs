using CyberBlood.Scripts.Settings;
using Godot;
using GodotCSToolbox;
using System.Collections.Generic;
using System.Diagnostics;
using CyberBlood.Scripts.Utils;
using Serilog;
using GArray = Godot.Collections.Array;

namespace CyberBlood.Scenes.GUI.SettingsMenu {
    public class Controls : Control, IConfigMenu {
        public bool NeedsConfirmation => false;

        private bool _wasChanged;
        private readonly Color _redColor = new (.7f, .07f, 0.07f);
        private readonly Dictionary<string, (Button, VBoxContainer)> _tabs = new();
        private readonly Dictionary<string, Button> _keyboardButtons = new();
#pragma warning disable CS0649
        [NodePath("hbox/panel/viewer/center/keyboard/vsens/hbox/hslider")]
        private Slider _mouseVSensitivity;
        [NodePath("hbox/panel/viewer/center/keyboard/hsens/hbox/hslider")]
        private Slider _mouseHSensitivity;
#pragma warning restore CS0649

        public override void _Ready() {
            this.SetupNodeTools();

            PopulateTabs();
            ConnectControlButtons();
            ConnectTabButtons();
            SetupFromConfig();

            _tabs["gamepad"].Item1.Pressed  = GameSettings.JoyConnected;
            _tabs["keyboard"].Item1.Pressed = !GameSettings.JoyConnected;
        }

        private void ConnectControlButtons() {
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
                    // TODO
                    slider.Connect("value_changed", this, nameof(SetWasChangedTrue));
                } else if (button is Button ctrlBtn) {
                    ctrlBtn.Connect("button_up", this, nameof(SetWasChangedTrue));
                    ctrlBtn.Connect(
                        "button_up", confirm, "ShowUp", new GArray { child.Name, ctrlBtn.Text }
                    );
                    _keyboardButtons[child.Name] = ctrlBtn;
                }
            }
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

        private void _on_Confirm_ControlSelected(string name, InputEvent action) {
            Debug.Assert(InputMap.HasAction(name));

            if (action == null) {
                return;
            }

            GameSettings.Controls.SetAction(name, action);
            SetText(_keyboardButtons[name], action);
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
                case InputEventJoypadMotion jm:
                    // TODO
                    button.Text = "Unsupported 1";
                    break;
                case InputEventJoypadButton jb:
                    // TODO
                    button.Text = "Unsupported 2";
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
            var ctrl = GameSettings.Controls;
            _keyboardButtons["move_forward"].Text  = ctrl.MoveForward.ToString();
            _keyboardButtons["move_left"].Text     = ctrl.MoveLeft.ToString();
            _keyboardButtons["move_back"].Text     = ctrl.MoveBack.ToString();
            _keyboardButtons["move_right"].Text    = ctrl.MoveRight.ToString();
            _keyboardButtons["camera_center"].Text = ctrl.CameraCenter.ToString();
            _mouseHSensitivity.Value               = ctrl.CameraMouseRotateHorizontal / ctrl.CameraMouseDenominator;
            _mouseVSensitivity.Value               = ctrl.CameraMouseRotateVertical / ctrl.CameraMouseDenominator;

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
