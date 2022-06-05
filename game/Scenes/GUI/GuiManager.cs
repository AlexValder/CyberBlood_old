using System.Diagnostics;
using Godot;

namespace CyberBlood.Scenes.GUI {
    public class GuiManager : Node {
        private Control _pauseMenu;
        public Theme DefaultTheme { get; private set; }

        public override void _Ready() {
            DefaultTheme = GD.Load<Theme>("res://Assets/themes/default.tres");
            _pauseMenu   = GD.Load<PackedScene>("res://Scenes/GUI/PauseMenu.tscn").Instance<Control>();
            SetupFpsCounter();
        }

        [Conditional("DEBUG")]
        private void SetupFpsCounter() {
            var res   = GD.Load<PackedScene>("res://Scenes/GUI/FpsLabel.tscn");
            var scene = res.Instance<Label>();
            AddChild(scene);
        }

        public void Switch2Gameplay() {
            Input.SetMouseMode(Input.MouseMode.Captured);
            AddChild(_pauseMenu);
        }

        public void Switch2Menu() {
            Input.SetMouseMode(Input.MouseMode.Captured);
            RemoveChild(_pauseMenu);
        }

        public override void _ExitTree() {
            if (_pauseMenu.IsInsideTree()) {
                RemoveChild(_pauseMenu);
            }

            _pauseMenu.QueueFree();
        }
    }
}
