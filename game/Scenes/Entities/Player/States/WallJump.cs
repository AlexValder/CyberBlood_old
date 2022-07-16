using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.Entities.Player.States {
    public class WallJump : BaseJump {
#pragma warning disable CS0649
        [NodePath("tween")] private Tween _tween;
#pragma warning restore CS0649

        public WallJump() : base(State.FinalFall) {
        }

        public override void _Ready() {
            this.SetupNodeTools();
        }

        public override void OnEntry() {
            _tween.StopAll();
            _tween.InterpolateProperty(
                Player.Mesh,
                "rotation:y",
                Player.Mesh.Rotation.y,
                Mathf.LerpAngle(Player.Mesh.Rotation.y, Player.Mesh.Rotation.y + Mathf.Pi, 1f),
                0.21f
            );

            var dir = Player.Front;
            Player.LookDirection = new Vector2(dir.z, dir.x);
            var velocity = Player.Velocity;
            velocity.x      = dir.x * Player.WalkSpeed;
            velocity.z      = dir.z * Player.WalkSpeed;
            Player.Velocity = velocity;
            base.OnEntry();
            _tween.Start();
            AnimStateMachine.Travel("wall-jump");
        }
    }
}
