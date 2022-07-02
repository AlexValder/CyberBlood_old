using Godot;

namespace CyberBlood.Scenes.Entities.Player.States {
    public class Idle : BaseState {
        public override void OnEntry() {
            Player.Velocity = Vector3.Zero;
        }

        public override void HandlePhysicsProcess(float delta) {
            if (Input.IsActionJustPressed("jump")) {
                Player.Machine.TransitionTo(State.Jumping);
            }

            var look = Input.GetVector(
                "move_right", "move_left", "move_back", "move_forward"
            );

            var velocity = Player.Velocity;
            velocity.y           -= Player.Gravity * delta;
            Player.Velocity      =  velocity;
            Player.LookDirection =  look;

            if (look.Length() > .1f) {
                Player.Machine.TransitionTo(State.Moving);
            }
        }
    }
}
