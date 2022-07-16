using Godot;

namespace CyberBlood.Scenes.Entities.Player.States {
    public abstract class BaseJump : BaseState {
        private readonly State _fallState;
        private readonly float _coefficient;

        protected BaseJump(State state, float coefficient = 1f) {
            _fallState   = state;
            _coefficient = coefficient;
        }

        public override void OnEntry() {
            Player.SnapVector = Vector3.Zero;
            Player.AnimTree.Set("parameters/jump/OneShot/active", true);
            AnimStateMachine.Travel("jump");
        }

        public override void HandlePhysicsProcess(float delta) {
            var velocity = Player.Velocity;
            velocity.y      = Player.JumpCoefficient * _coefficient;
            Player.Velocity = Player.MoveAndSlideWithSnap(velocity, Player.SnapVector, Vector3.Up);
            Player.Machine.TransitionTo(_fallState);
        }
    }
}
