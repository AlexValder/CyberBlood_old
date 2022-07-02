using Godot;

namespace CyberBlood.Scenes.Entities.Player.States {
    public class Moving : BaseState {
        public override void HandlePhysicsProcess(float delta) {
            var look = Input.GetVector(
                "move_right", "move_left", "move_back", "move_forward"
            );

            Player.LookDirection = look;

            var dir = new Vector3(-look[0], 0, -look[1])
                .Rotated(Vector3.Up, Player.Camera.Arm.Rotation.y)
                .Normalized();

            var velocity = Player.Velocity;
            velocity.x =  dir.x * Player.WalkSpeed;
            velocity.z =  dir.z * Player.WalkSpeed;
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
        }
    }
}
