using CyberBlood.Scripts;
using Godot;

namespace CyberBlood.Scenes.GUI {
    public class MainMenu : Node {
        public override void _Ready() {
            Input.SetMouseMode(Input.MouseMode.Visible);
            var firstButton = GetNode<Control>("vbox/vbox/start");
            firstButton.GrabFocus();
        }

        private void _on_start_button_up() {
            var gm = GetNode<GameManager>("/root/GameManager");
            gm.ChangeScene("debug");
            QueueFree();
        }


        private void _on_exit_button_up() {
            QueueFree();
            GetTree().Quit(0);
        }
    }
}
