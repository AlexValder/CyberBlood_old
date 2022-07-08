using Godot;

namespace CyberBlood.Scenes.GUI {
    public class GuiManager : Node {
        private Control _pauseMenu;

        public override void _Ready() {
            _pauseMenu = GD.Load<PackedScene>("res://Scenes/GUI/PauseMenu.tscn").Instance<Control>();
            SetupFpsCounter();
        }

        private void SetupFpsCounter() {
            var res   = GD.Load<PackedScene>("res://Scenes/GUI/FpsLabel.tscn");
            var scene = res.Instance<Label>();
            AddChild(scene);
        }

        public void Switch2Gameplay() {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            AddChild(_pauseMenu);
        }

        public void Switch2Menu() {
            Input.MouseMode = Input.MouseModeEnum.Captured;
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
