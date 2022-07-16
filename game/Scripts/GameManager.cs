using System.Collections.Generic;
using System.Diagnostics;
using CyberBlood.addons.playable_spatial;
using CyberBlood.Scenes.Entities.Player;
using CyberBlood.Scenes.GUI;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scripts {
    public class GameManager : Node {
#pragma warning disable 649
        [NodePath("/root/")] private Node _root;
        [NodePath("/root/GuiManager")] private GuiManager _guiManager;
#pragma warning restore 649

        public static bool IsPlaying { get; private set; }
        
        private readonly PackedScene _mainMenu;
        private readonly Player _player;
        private string _name = "menu";
        private PlayableSpatial _currentScene;

        public static IReadOnlyDictionary<string, string> Levels => s_levels;
        private static readonly Dictionary<string, string> s_levels = new() {
            ["debug"] = "res://Scenes/Static/DebugLevel.tscn",
        };

        public GameManager() {
            _mainMenu = GD.Load<PackedScene>("res://Scenes/GUI/MainMenu.tscn");
            _player   = GD.Load<PackedScene>("res://Scenes/Entities/Player/Player.tscn").Instance<Player>();
        }

        public override void _Ready() {
            this.SetupNodeTools();
        }

        public void ChangeScene(string name) {
            Debug.Assert(s_levels.ContainsKey(name));

            _name = name;

            if (_currentScene != null) {
                _root.RemoveChild(_currentScene);
                _currentScene.RemoveChild(_player);
                _currentScene.QueueFree();
                _player.Reset();
            } else {
                IsPlaying = true;
                _guiManager.Switch2Gameplay();
            }

            var scene = GD.Load<PackedScene>(s_levels[name]);
            _currentScene = scene.Instance<PlayableSpatial>();
            _currentScene.AddChild(_player);
            _root.AddChild(_currentScene);
        }

        public void Restart() {
            ChangeScene(_name);
        }

        public void QuitToMenu() {
            GetTree().Paused = false;
            IsPlaying        = false;
            _root.RemoveChild(_currentScene);
            _currentScene.RemoveChild(_player);
            _currentScene.QueueFree();
            _currentScene = null;

            _guiManager.Switch2Menu();

            _root.AddChild(_mainMenu.Instance());
        }

        public void TogglePause(bool paused) {
            GetTree().Paused = paused;
        }

        public override void _ExitTree() {
            _player.QueueFree();
            _currentScene?.QueueFree();
        }
    }
}
