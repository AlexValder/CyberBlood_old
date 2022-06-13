using CyberBlood.Scripts.Settings.Config;
using CyberBlood.Scripts.Settings.Config.Gamepad;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.GUI.SettingsMenu {
    public class Confirm : ConfirmationDialog {
        private InputEvent _input;
        private string _action;
        private InputMode _inputMode;

        public enum InputMode {
            MouseKeyboard,
            Gamepad,
        }

#pragma warning disable CS0649
        [NodePath("button")] private Button _button;
#pragma warning restore CS0649

        [Signal]
        public delegate void ControlSelected(string name, InputEvent action, InputMode mode);

        public override void _Ready() {
            this.SetupNodeTools();

            _button.Connect("got_input", this, nameof(HandleInput));
        }

        public void ShowUp(string action, string current, InputMode mode) {
            _action      = action;
            _button.Text = current;
            _inputMode   = mode;
            PopupCentered();
            GetOk().ReleaseFocus();
        }

        private void HandleInput(InputEvent @event) {
            if (_inputMode == InputMode.MouseKeyboard) {
                if (@event is InputEventMouseButton mb) {
                    _input       = mb;
                    _button.Text = ControlsConfig.MouseButtonString(mb.ButtonIndex);
                } else if (@event is InputEventKey key) {
                    _input       = key;
                    _button.Text = OS.GetScancodeString(key.Scancode);
                }
            } else if (@event is InputEventJoypadButton jb) {
                _input       = jb;
                _button.Text = GamepadButtonMetaSelector.GetName(jb.ButtonIndex);
            }
        }

        private void _on_Confirm_confirmed() {
            EmitSignal(nameof(ControlSelected), _action, _input, _inputMode);
        }

        private void _on_Confirm_about_to_show() {
            _input = null;
        }
    }
}
