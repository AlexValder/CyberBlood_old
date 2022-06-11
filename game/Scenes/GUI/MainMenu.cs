using CyberBlood.Scripts;
using CyberBlood.Scripts.Settings;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.GUI {
    public class MainMenu : Control {
#pragma warning disable 649
        [NodePath("SettingsMenu")] private SettingsMenu.SettingsMenu _settings;
#pragma warning restore 649

        public override void _Ready() {
            this.SetupNodeTools();

            Input.SetMouseMode(Input.MouseMode.Visible);
            var firstButton = GetNode<Control>("vbox/vbox/start");
            firstButton.GrabFocus();
        }

        private void _on_start_button_up() {
            var gm = GetNode<GameManager>("/root/GameManager");
            gm.ChangeScene("debug");
            QueueFree();
        }

        private void _on_settings_button_up() {
            _settings.PopupCentered();
        }

        private void _on_exit_button_up() {
            QueueFree();
            GetTree().Quit(0);
        }
    }
}
