using System;
using CyberBlood.Scripts.Settings;
using Godot;
using GodotCSToolbox;
using Serilog;

namespace CyberBlood.Scenes.Entities {
    public class Player : KinematicBody {
        private const string MOVE_FORWARD = "move_forward";
        private const string MOVE_LEFT = "move_left";
        private const string MOVE_RIGHT = "move_right";
        private const string MOVE_BACK = "move_back";
        private const string JUMP = "jump";

        private const float WALK_SPEED = 4.5f * 1.2f;
        private const float ANGULAR_ACCELERATION = 10f;
        private const float JUMP_COEFFICIENT = 10f * 1.5f;
        private const float GRAVITY = 15f * 1.5f;

        private Vector3 _velocity = Vector3.Zero;

        private Vector3 _snapVector = Vector3.Down;

        private PlayerState State {
            get => _state;
            set {
                _state = value;
                if (_statusLabel != null) {
                    _statusLabel.Text = _state.ToString();
                }
            }
        }

        public Vector3 MeshRotation => _mesh.Rotation;
        

        [Flags]
        private enum PlayerState {
            Idle = 1,
            Running = 2,
            Falling = 4,
            Jump = 8,
            DoubleJump = 16,
        }

        private Vector3 _direction = Vector3.Back;
        private PlayerState _state = PlayerState.Idle;

#pragma warning disable 649
        [NodePath("mesh")] private Spatial _mesh;
        [NodePath("PlayerCamera")] private PlayerCamera _camera;
        [NodePath("HUD/status_label")] private Label _statusLabel;
#pragma warning restore 649

        public override void _Ready() {
            this.SetupNodeTools();
        }

        public void Reset() {
            _state     = PlayerState.Idle;
            _direction = Vector3.Back;
            _velocity  = Vector3.Zero;
            _camera.Reset();
        }

        public void SetSpawn(Position3D spawnPoint) {
            var rot = spawnPoint.Rotation;

            Transform = spawnPoint.Transform;
            Rotation  = _camera.Arm.Rotation = rot;
            _mesh.Rotation = new Vector3(
                _mesh.Rotation.x,
                rot.y + Mathf.Pi / 2,
                _mesh.Rotation.z
            );
        }

        public override void _Process(float delta) {
            _camera.Translation = Translation;
        }

        public override void _PhysicsProcess(float delta) {
            var dir = Input.GetVector(MOVE_RIGHT, MOVE_LEFT, MOVE_BACK, MOVE_FORWARD);
            _direction = new Vector3(-dir[0], 0, -dir[1]).Rotated(Vector3.Up, _camera.Arm.Rotation.y).Normalized();

            var velocity = _velocity;
            velocity.x =  _direction.x * WALK_SPEED;
            velocity.z =  _direction.z * WALK_SPEED;
            velocity.y -= GRAVITY * delta;

            var justLanded = IsOnFloor() && _snapVector == Vector3.Zero;

            if (Input.IsActionJustPressed(JUMP)) {
                if (State <= PlayerState.DoubleJump) {
                    velocity.y  = JUMP_COEFFICIENT;
                    _snapVector = Vector3.Zero;
                }

                if (IsOnFloor()) {
                    State |= PlayerState.Jump;
                } else {
                    State |= PlayerState.DoubleJump;
                }
            } else if (justLanded) {
                State       &= ~PlayerState.Jump;
                State       &= ~PlayerState.DoubleJump;
                _snapVector =  Vector3.Down;
            }

            _velocity = MoveAndSlideWithSnap(velocity, _snapVector, Vector3.Up, true);

            if (_direction.Length() > .2f) {
                _camera.MovementTimer.Start();
                State &= ~PlayerState.Idle;
                State |= PlayerState.Running;
                var lookDir = new Vector2(_velocity.z, _velocity.x);
                var meshRot = _mesh.Rotation;
                meshRot.y      = Mathf.LerpAngle(_mesh.Rotation.y, lookDir.Angle() - Mathf.Pi / 2f, delta * ANGULAR_ACCELERATION);
                _mesh.Rotation = meshRot;
            } else {
                State &= ~PlayerState.Running;
                State |= PlayerState.Idle;
            }
        }
    }
}
