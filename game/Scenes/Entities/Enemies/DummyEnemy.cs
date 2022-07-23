using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.Entities.Enemies {
    public class DummyEnemy : KinematicBody, EnemyBase {
#pragma warning disable CS0649
        [NodePath("health")] private Spatial _healthBar;
        [NodePath("area")] private Area _detectionArea;
        [NodePath("target")] private Position3D _target;
#pragma warning restore CS0649
        private bool _isTargeted;
        private bool _playerNear;

        public Position3D Target => _target;

        public bool IsTargeted {
            get => _isTargeted;
            set {
                if (!_playerNear) {
                    _healthBar.Visible = value;
                }
                _isTargeted = value;
            }
        }

        public override void _Ready() {
            this.SetupNodeTools();

            _healthBar.Visible = false;
            _playerNear        = false;
            _isTargeted        = false;
        }

        private void _on_area_body_entered(Node body) {
            if (body is not Player.Player) {
                return;
            }

            _playerNear        = true;
            _healthBar.Visible = true;
        }


        private void _on_area_body_exited(Node body) {
            if (body is not Player.Player) {
                return;
            }

            _playerNear        = false;
            if (!_isTargeted) {
                _healthBar.Visible = false;
            }
        }
    }
}
