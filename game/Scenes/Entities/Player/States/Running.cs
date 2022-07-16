using Godot;

namespace CyberBlood.Scenes.Entities.Player.States {
    public class Running : BaseState {
        public override void OnEntry() {
            AnimStateMachine.Travel("run");
        }

        public override void HandlePhysicsProcess(float delta) {
            var look = Input.GetVector(
                "move_right", "move_left", "move_back", "move_forward"
            );

            Player.LookDirection = look;

            if (look.Length() <= .35f) {
                Player.Machine.TransitionTo(State.Walking);
            }

            var dir = new Vector3(-look[0], 0, -look[1])
                .Rotated(Vector3.Up, Player.Camera.Arm.Rotation.y)
                .Normalized();

            var velocity = Player.Velocity;
            velocity.x =  dir.x * Player.RunSpeed;
            velocity.z =  dir.z * Player.RunSpeed;
            velocity.y -= Player.Gravity * delta;

            if (Input.IsActionJustPressed("jump")) {
                Player.Machine.TransitionTo(State.Jumping);
            }

            var lookDir = new Vector2(velocity.z, velocity.x);
            var meshRot = Player.Mesh.Rotation;
            meshRot.y = Mathf.LerpAngle(meshRot.y, lookDir.Angle() - Mathf.Pi / 2f,
                                        delta * Player.AngularAcceleration);
            Player.Mesh.Rotation = meshRot;

            Player.Velocity = Player.MoveAndSlideWithSnap(velocity, Player.SnapVector, Vector3.Up);

            var speed = dir.Length();
            if (speed <= .1f) {
                Player.Machine.TransitionTo(State.Idle);
            }

            if (!Player.IsOnFloor()) {
                Player.Machine.TransitionTo(State.Falling);
            }

            Player.AnimTree.Set("parameters/run/TimeScale/scale", speed);
        }

        public override void OnExit() {
            Player.AnimTree.Set("parameters/run/TimeScale/scale", 1f);
        }
    }
}
