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
        private const float ACCELERATION = 6f * 1.2f;
        private const float ANGULAR_ACCELERATION = 10f;
        private const float JUMP_COEFFICIENT = 25f * 1.5f;
        private const float GRAVITY = 15f * 1.5f;

        public Vector3 Velocity { get; private set; } = Vector3.Zero;
        public Vector3 MeshRotation => _mesh.Rotation;
        private bool IsFalling => _verticalVelocity < 0;


        [Flags]
        private enum PlayerState {
            Idle = 1,
            Running = 2,
            Falling = 4,
            Jump = 8,
            DoubleJump = 16,
        }

        private float _hRot;
        private Vector3 _direction = Vector3.Back;
        private Vector3 _strafeDir = Vector3.Zero;
        private Vector3 _strafe = Vector3.Zero;
        private float _aimTurn;
        private float _movementSpeed;
        private float _verticalVelocity;
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
            _hRot      = 0;
            _state     = PlayerState.Idle;
            _direction = Vector3.Back;
            Velocity   = _strafeDir = _strafe = Vector3.Zero;
            _camera.Reset();
        }

        public void SetSpawn(Position3D spawnPoint) {
            var rot = spawnPoint.Rotation;

            Transform = spawnPoint.Transform;
            Rotation  = _camera.Rotation = _camera.H.Rotation = _camera.V.Rotation = rot;
            _mesh.Rotation = new Vector3(
                _mesh.Rotation.x,
                rot.y + Mathf.Pi / 2,
                _mesh.Rotation.z
            );
        }

        public override void _Process(float delta) {
            _statusLabel.Text = IsOnWall() ? "ON WALL" : "NOT ON WALL";
        }

        public override void _PhysicsProcess(float delta) {
            var dir = Input.GetVector(MOVE_RIGHT, MOVE_LEFT, MOVE_BACK, MOVE_FORWARD);
            _direction = new Vector3(dir[0], 0, dir[1]);

            if ((_state & PlayerState.Idle) == PlayerState.Idle) {
                if (dir.Length() > 0) {
                    _state &= ~PlayerState.Idle;
                    _state |= PlayerState.Running;

                    _hRot = _camera.H.GlobalTransform.basis.GetEuler().y;

                    _movementSpeed = WALK_SPEED;
                    _strafeDir     = _direction;
                }
            } else if ((_state & PlayerState.Running) == PlayerState.Running) {
                if (
                    Input.IsActionJustReleased(MOVE_FORWARD) ||
                    Input.IsActionJustReleased(MOVE_RIGHT) ||
                    Input.IsActionJustReleased(MOVE_LEFT) ||
                    Input.IsActionJustReleased(MOVE_BACK)
                ) {
                    _state &= ~PlayerState.Running;
                    _state |= PlayerState.Idle;

                    _movementSpeed = 0;
                    _strafeDir     = Vector3.Zero;
                }
            }

            if (IsOnFloor()) {
                _state &= ~PlayerState.Jump;
                _state &= ~PlayerState.DoubleJump;
            }

            if (Input.IsActionJustPressed(JUMP) && _verticalVelocity >= 0) {
                switch (_state) {
                    case < PlayerState.Jump:
                        _state            |= PlayerState.Jump;
                        _verticalVelocity =  -JUMP_COEFFICIENT * GRAVITY * delta;
                        _hRot             =  _camera.H.GlobalTransform.basis.GetEuler().y;
                        break;
                    case < PlayerState.DoubleJump:
                        _state            |= PlayerState.DoubleJump;
                        _verticalVelocity =  -JUMP_COEFFICIENT * GRAVITY * delta;
                        _hRot             =  _camera.H.GlobalTransform.basis.GetEuler().y;
                        if (IsOnWall()) {
                            var normal = GetSlideCollision(0).Normal;
                        }
                        break;
                }
            }

            _direction = _direction.Rotated(Vector3.Up, _hRot).Normalized();
            Velocity   = Velocity.LinearInterpolate(_direction * _movementSpeed, delta * ACCELERATION);

            MoveAndSlide(-Velocity + Vector3.Down * _verticalVelocity, Vector3.Up);
            if (!IsOnFloor()) {
                _verticalVelocity += GRAVITY * delta;
            } else {
                _verticalVelocity = 0;
            }

            var angle =
                (-_mesh.Transform.basis.x).SignedAngleTo(Velocity, _mesh.Transform.origin)
                * delta * ANGULAR_ACCELERATION;
            _mesh.RotateY(angle);

            _strafe  = _strafe.LinearInterpolate(_strafeDir + Vector3.Right * _aimTurn, delta * ACCELERATION);
            _aimTurn = 0f;
        }
    }
}
