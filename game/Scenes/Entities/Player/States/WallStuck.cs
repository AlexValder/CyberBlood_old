using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.Entities.Player.States {
    public class WallStuck : BaseState {
#pragma warning disable CS0649
        [NodePath("timer")] private Timer _timer;
#pragma warning restore CS0649

        public override void _Ready() {
            this.SetupNodeTools();
        }

        public override void OnEntry() {
            _timer.Start(.5f);
            AnimStateMachine.Travel("wall-stuck");
        }

        public override void HandlePhysicsProcess(float delta) {
            if (!Player.IsOnWall()) {
                return;
            }

            var original = Player.Front;
            var wall     = Player.GetLastSlideCollision().Normal;

            var angle = original.SignedAngleTo(wall, Vector3.Up);
            Player.Mesh.RotateY(angle * delta * 15f);

            if (Input.IsActionJustPressed("jump")) {
                Player.Machine.TransitionTo(State.WallJump);
            }
        }

        public override void OnExit() {
            _timer.Stop();
        }

        private void _on_timer_timeout() {
            Player.Machine.TransitionTo(State.FinalFall);
        }
    }
}
