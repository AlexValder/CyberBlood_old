using System;
using CyberBlood.Scenes.GUI.StatusMessages;
using Godot;

namespace CyberBlood.Scenes.GUI {
    public class GuiManager : Node {
        private Control _pauseMenu;
        private MessageContainer _container;

        public override void _Ready() {
            _pauseMenu = GD.Load<PackedScene>("res://Scenes/GUI/PauseMenu.tscn").Instance<Control>();
            SetupFpsCounter();
            SetupInfoLabels();
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionReleased("screenshot")) {
                TakeScreenshot();
            }
        }

        private void TakeScreenshot() {
            var path = $"{OS.GetUserDataDir()}/screenshots/";
            if (!System.IO.Directory.Exists(path)) {
                System.IO.Directory.CreateDirectory(path);
            }
            
            var time = DateTime.Now;
            var name = $"screenshot_{time.Year}-{time.Month}-{time.Day}" +
                       $"-{time.Hour}-{time.Minute}-{time.Second}.{time.Millisecond}.png";
            var fullpath = path + name;

            var image = GetViewport().GetTexture().GetData();
            image.FlipY();
            var error = image.SavePng(fullpath);
            if (error == Error.Ok) {
                DisplayMessage($"Saved screenshot at: {fullpath}");
            } else {
                DisplayMessage($"Error saving screenshot: {error}");
            }
        }

        private void SetupFpsCounter() {
            var res   = GD.Load<PackedScene>("res://Scenes/GUI/FpsLabel.tscn");
            var scene = res.Instance<Label>();
            AddChild(scene);
        }

        private void SetupInfoLabels() {
            var res   = GD.Load<PackedScene>("res://Scenes/GUI/StatusMessages/MessageContainer.tscn");
            var scene = res.Instance<MessageContainer>();
            AddChild(scene);
            _container = scene;
        }

        public void DisplayMessage(string msg) {
            _container?.DisplayMessage(msg);
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
