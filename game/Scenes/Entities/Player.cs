using System;
using CyberBlood.Scripts.Settings;
using Godot;
using GodotCSToolbox;
using Serilog;

namespace CyberBlood.Scenes.Entities;

public class Player : KinematicBody {
    private const string MOVE_FORWARD = "move_forward";
    private const string MOVE_LEFT = "move_left";
    private const string MOVE_RIGHT = "move_right";
    private const string MOVE_BACK = "move_back";

    private const float WALK_SPEED = 4.5f;
    private const float ACCELERATION = 6f;
    private const float ANGULAR_ACCELERATION = 10f;

    public Vector3 Direction { get; private set; } = Vector3.Back;
    public Vector3 Velocity { get; private set; } = Vector3.Zero;
    public Vector3 StrafeDir { get; private set; } = Vector3.Zero;
    public Vector3 Strafe { get; private set; } = Vector3.Zero;

    public float AimTurn { get; private set; }
    public float MovementSpeed { get; private set; }

    public float VerticalVelocity { get; private set; }
    public float Gravity { get; private set; } = 20f;
    public Vector3 MeshRotation => _mesh.Rotation;

    private enum PlayerState {
        Idle = 0,
        Walking = 1,
        Running = 2,
    }

    private PlayerState _state = PlayerState.Idle;
    private float _hRot;

#pragma warning disable 649
    [NodePath("mesh")] private Spatial _mesh;
    [NodePath("PlayerCamera")] private PlayerCamera _camera;
#pragma warning restore 649

    public override void _Ready() {
        this.SetupNodeTools();
    }

    public void Reset() {
        _hRot     = 0;
        _state    = PlayerState.Idle;
        Direction = Vector3.Back;
        Velocity  = StrafeDir = Strafe = Vector3.Zero;
        _camera.Reset();
    }

    public void SetSpawn(Position3D spawnPoint) {
        var rot = spawnPoint.Rotation;

        Transform = spawnPoint.Transform;
        Rotation  = _camera.Rotation = _camera.H.Rotation = _camera.V.Rotation = rot;
        _mesh.Rotation = new Vector3(
            _mesh.Rotation.x,
            rot.y + Mathf.Pi / 2,
            _mesh.Rotation.z
        );
    }

    public override void _PhysicsProcess(float delta) {
        Direction = new Vector3(
            Input.GetActionStrength(MOVE_LEFT) - Input.GetActionStrength(MOVE_RIGHT),
            0,
            Input.GetActionStrength(MOVE_FORWARD) - Input.GetActionStrength(MOVE_BACK)
        );

        switch (_state) {
            case PlayerState.Idle: {
                if (
                    Input.IsActionPressed(MOVE_FORWARD) ||
                    Input.IsActionPressed(MOVE_RIGHT) ||
                    Input.IsActionPressed(MOVE_LEFT) ||
                    Input.IsActionPressed(MOVE_BACK)
                ) {
                    _state = PlayerState.Walking;

                    _hRot = _camera.H.GlobalTransform.basis.GetEuler().y;

                    MovementSpeed = WALK_SPEED;
                    StrafeDir     = Direction;
                }
            }
                break;
            case PlayerState.Walking:
            case PlayerState.Running: {
                if (
                    Input.IsActionJustReleased(MOVE_FORWARD) ||
                    Input.IsActionJustReleased(MOVE_RIGHT) ||
                    Input.IsActionJustReleased(MOVE_LEFT) ||
                    Input.IsActionJustReleased(MOVE_BACK)
                ) {
                    _state = PlayerState.Idle;

                    MovementSpeed = 0;
                    StrafeDir     = Vector3.Zero;
                }
            }
                break;
            default:
                Log.Logger.Error("Unknown state: {State}", _state);
                throw new ArgumentOutOfRangeException(nameof(_state), _state, "Unknown player state");
        }

        Direction = Direction.Rotated(Vector3.Up, _hRot).Normalized();
        Velocity  = Velocity.LinearInterpolate(Direction * MovementSpeed, delta * ACCELERATION);

        MoveAndSlide(-Velocity - Vector3.Down * VerticalVelocity, Vector3.Up);

        var angle =
            (-_mesh.Transform.basis.x).SignedAngleTo(Velocity, _mesh.Transform.origin)
            * delta * ANGULAR_ACCELERATION;
        _mesh.RotateY(angle);

        Strafe  = Strafe.LinearInterpolate(StrafeDir + Vector3.Right * AimTurn, delta * ACCELERATION);
        AimTurn = 0f;
    }
}
