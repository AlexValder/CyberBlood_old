using Godot;

namespace CyberBlood.Scenes.Entities.Player.States {
    public class BaseFall : BaseState {
        public override void OnEntry() {
            Player.SnapVector = Vector3.Down;
        }

        public override void HandlePhysicsProcess(float delta) {
            var velocity = Player.Velocity;
            velocity.y -= Player.Gravity * delta;

            var look = Input.GetVector(
                "move_right", "move_left", "move_back", "move_forward"
            );

            if (look.Length() > .1f) {

                Player.LookDirection = look;

                var dir = new Vector3(-look[0], 0, -look[1])
                    .Rotated(Vector3.Up, Player.Camera.Arm.Rotation.y)
                    .Normalized();

                velocity.x = dir.x * Player.WalkSpeed;
                velocity.z = dir.z * Player.WalkSpeed;

                var lookDir = new Vector2(dir.z, dir.x);
                var meshRot = Player.Mesh.Rotation;
                meshRot.y = Mathf.LerpAngle(meshRot.y, lookDir.Angle() - Mathf.Pi / 2f,
                                            delta * Player.AngularAcceleration);
                Player.Mesh.Rotation = meshRot;
            }

            Player.Velocity = Player.MoveAndSlideWithSnap(velocity, Player.SnapVector, Vector3.Up);

            if (Player.IsOnFloor()) {
                Player.Machine.TransitionTo(State.Idle);
            }
        }
    }
}
