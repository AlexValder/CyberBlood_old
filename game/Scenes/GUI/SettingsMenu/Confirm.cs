using CyberBlood.Scripts.Settings.Config;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.GUI.SettingsMenu;

public class Confirm : ConfirmationDialog {
    private InputEvent _input;
    private string _action;

#pragma warning disable CS0649
    [NodePath("button")] private Button _button;
#pragma warning restore CS0649

    [Signal]
    public delegate void ControlSelected(string name, InputEvent action);

    public override void _Ready() {
        this.SetupNodeTools();

        _button.Connect("got_input", this, nameof(HandleInput));
    }

    public void ShowUp(string action, string current) {
        _action     = action;
        _button.Text = current;
        PopupCentered();
    }

    private void HandleInput(InputEvent @event) {
        switch (@event) {
            case InputEventMouseButton mb:
                _input      = mb;
                _button.Text = ControlsConfig.MouseButtonString(mb.ButtonIndex);
                break;
            case InputEventKey key:
                _input      = key;
                _button.Text = OS.GetScancodeString(key.Scancode);
                break;
        }
    }

    private void _on_Confirm_confirmed() {
        EmitSignal(nameof(ControlSelected), _action, _input);
    }

    private void _on_Confirm_about_to_show() {
        _input = null;
    }
}
