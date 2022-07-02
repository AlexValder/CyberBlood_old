using Godot;

namespace CyberBlood.Scenes.Entities.Player.States {
    public class Falling : BaseFall {
        public override void HandlePhysicsProcess(float delta) {
            if (Input.IsActionJustPressed("jump")) {
                Player.Machine.TransitionTo(State.DoubleJump);
            }

            if (Player.IsOnWall()) {
                Player.Machine.TransitionTo(State.WallStuck);
            }

            base.HandlePhysicsProcess(delta);
        }
    }
}
