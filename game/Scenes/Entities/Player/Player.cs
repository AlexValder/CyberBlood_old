using System;
using CyberBlood.addons.playable_spatial;
using CyberBlood.Scenes.Entities.Enemies;
using CyberBlood.Scenes.Entities.Player.States;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.Entities.Player {
    public class Player : KinematicBody {
        private const float WALK_SPEED = 4.5f * 1.2f;
        private const float RUN_SPEED = WALK_SPEED * 1.5f;
        private const float ANGULAR_ACCELERATION = 10f;
        private const float JUMP_COEFFICIENT = 10f * 1.5f;
        private const float GRAVITY = 15f * 1.5f;

        private DummyEnemy _currentEnemy;

        public static float WalkSpeed => WALK_SPEED;
        public static float RunSpeed => RUN_SPEED;
        public static float AngularAcceleration => ANGULAR_ACCELERATION;
        public static float JumpCoefficient => JUMP_COEFFICIENT;
        public static float Gravity => GRAVITY;

        public Vector3 Velocity { get; set; } = Vector3.Zero;
        public Vector3 SnapVector { get; set; } = Vector3.Down;
        public Vector2 LookDirection { get; set; }
        public AnimationTree AnimTree { get; private set; }

        public Vector3 Front => -Mesh?.GlobalTransform.basis.x ?? Vector3.Forward;

#pragma warning disable 649
        [NodePath("mesh")] public Spatial Mesh { get; private set; }
        [NodePath("HUD/status_label")] private Label _statusLabel;
        [NodePath("HUD/target")] private TextureRect _targetRect;
        [NodePath("PlayerCamera")] public PlayerCamera Camera { get; set; }
        [NodePath("StateMachine")] public StateMachine Machine { get; set; }
#pragma warning restore 649

        public override void _Ready() {
            this.SetupNodeTools();

            _targetRect.Visible = false;

            AnimTree   = Mesh.GetNode<AnimationTree>("basic_man/animation_tree");

            _statusLabel.Text = Machine.CurrentState.ToString();
            Machine.Connect("StateChanged", this, nameof(SetStateLabel));
            Machine.PopulateStates();
        }

        private void SetStateLabel(State _, State next) {
            _statusLabel.Text = next.ToString();
        }

        public void Reset() {
            Machine.Reset();
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

        public override void _UnhandledInput(InputEvent @event) {
            if (@event.IsActionPressed("lockon")) {
                _currentEnemy            = FindEnemy();
                _currentEnemy.IsTargeted = true;
                _targetRect.Visible      = true;
            } else if (@event.IsActionReleased("lockon")) {
                _currentEnemy.IsTargeted = false;
                _currentEnemy            = null;
                _targetRect.Visible      = false;
            }
        }

        // TODO: Change to Enemy base class;
        private DummyEnemy FindEnemy() {
            var enemies = GetTree().GetNodesInGroup("enemy");
            // TODO: actual targeting
            return enemies.Count > 0 ? (DummyEnemy)enemies[0] : null;
        }

        public override void _Process(float delta) {
            if (_currentEnemy != null) {
                var screenPos = Camera.Camera.UnprojectPosition(_currentEnemy.Target.GlobalTransform.origin);
                _targetRect.RectPosition = screenPos - _targetRect.RectSize / 2;
            }

            Camera.Translation       = Translation;
            Machine.Process(delta);
        }

        public override void _PhysicsProcess(float delta) {
            Machine.PhysicsProcess(delta);
        }
    }
}
