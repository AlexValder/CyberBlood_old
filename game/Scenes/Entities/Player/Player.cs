using System;
using CyberBlood.Scenes.Entities.Player.States;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.Entities.Player {
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

        public float WalkSpeed => WALK_SPEED;
        public float AngularAcceleration => ANGULAR_ACCELERATION;
        public float JumpCoefficient => JUMP_COEFFICIENT;
        public float Gravity => GRAVITY;

        public Vector3 Velocity { get; set; } = Vector3.Zero;
        public Vector3 SnapVector { get; set; } = Vector3.Down;
        public Vector2 LookDirection { get; set; }

        private PlayerState State {
            get => _state;
            set {
                _state = value;
                if (_statusLabel != null) {
                    _statusLabel.Text = _state.ToString();
                }
            }
        }

        [Flags]
        private enum PlayerState {
            Idle = 1,
            Running = 2,
            Jump = 4,
            WallJump = 8,
            DoubleJump = 16,
        }

        private Vector3 _direction = Vector3.Back;
        private PlayerState _state = PlayerState.Idle;

        public Vector3 Front => -Mesh?.GlobalTransform.basis.x ?? Vector3.Forward;

#pragma warning disable 649
        [NodePath("tween")] private Tween _tween;
        [NodePath("mesh")] public Spatial Mesh { get; private set; }
        [NodePath("HUD/status_label")] private Label _statusLabel;
        [NodePath("PlayerCamera")] public PlayerCamera Camera { get; set; }
        [NodePath("StateMachine")] public StateMachine Machine { get; set; }
#pragma warning restore 649

        public override void _Ready() {
            this.SetupNodeTools();

            _statusLabel.Text = Machine.CurrentState.ToString();
            Machine.Connect("StateChanged", this, nameof(SetStateLabel));
        }

        private void SetStateLabel(State _, State next) {
            _statusLabel.Text = next.ToString();
        }

        public void Reset() {
            _state     = PlayerState.Idle;
            _direction = Vector3.Back;
            Velocity  = Vector3.Zero;
            Camera.Reset();
        }

        public void SetSpawn(Position3D spawnPoint) {
            var rot = spawnPoint.Rotation;

            Transform = spawnPoint.Transform;
            Rotation  = Camera.Arm.Rotation = rot;
            Mesh.Rotation = new Vector3(
                Mesh.Rotation.x,
                rot.y + Mathf.Pi / 2,
                Mesh.Rotation.z
            );
        }

        public override void _Process(float delta) {
            Camera.Translation = Translation;
            Machine.Process(delta);
        }

        public override void _PhysicsProcess(float delta) {
            Machine.PhysicsProcess(delta);
            /*
            var dir = Input.GetVector(MOVE_RIGHT, MOVE_LEFT, MOVE_BACK, MOVE_FORWARD);
            _direction = new Vector3(-dir[0], 0, -dir[1]).Rotated(Vector3.Up, Camera.Arm.Rotation.y).Normalized();

            var velocity = Velocity;
            velocity.x =  _direction.x * WALK_SPEED;
            velocity.z =  _direction.z * WALK_SPEED;
            velocity.y -= GRAVITY * delta;

            var justLanded = IsOnFloor() && SnapVector == Vector3.Zero;

            if (Input.IsActionJustPressed(JUMP)) {
                if (State <= PlayerState.WallJump) {
                    velocity.y  = JUMP_COEFFICIENT;
                    SnapVector = Vector3.Zero;
                }

                if (IsOnFloor()) {
                    State |= PlayerState.Jump;
                } else {
                    State |= PlayerState.DoubleJump;
                }
            } else if (justLanded) {
                State       &= ~PlayerState.Jump;
                State       &= ~PlayerState.WallJump;
                State       &= ~PlayerState.DoubleJump;
                SnapVector =  Vector3.Down;
            }

            Velocity = MoveAndSlideWithSnap(velocity, SnapVector, Vector3.Up, true);

            if (_direction.Length() > .2f) {
                Camera.MovementTimer.Start();
                State &= ~PlayerState.Idle;
                State |= PlayerState.Running;
                var lookDir = new Vector2(Velocity.z, Velocity.x);
                var meshRot = _mesh.Rotation;
                meshRot.y = Mathf.LerpAngle(_mesh.Rotation.y, lookDir.Angle() - Mathf.Pi / 2f,
                                            delta * ANGULAR_ACCELERATION);
                _mesh.Rotation = meshRot;
            } else {
                State &= ~PlayerState.Running;
                State |= PlayerState.Idle;
            }
            */
        }
    }
}
