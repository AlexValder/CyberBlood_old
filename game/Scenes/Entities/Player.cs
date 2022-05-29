using Godot;
using System;
using System.Data;
using GodotCSToolbox;
using Serilog;
using Serilog.Core;

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
    public float MovementSpeed { get; private set; } = 0f;

    public float VerticalVelocity { get; private set; } = 0f;
    public float Gravity { get; private set; } = 20f;

    private enum PlayerState {
        Idle = 0,
        Walking = 1,
        Running = 2,
        InAir = 3,
    }

    private PlayerState _state = PlayerState.Idle;
    private PlayerCamera _camera;
    private Spatial _mesh;

    public override void _Ready() {
        _mesh          = GetNode<Spatial>("mesh");
        _camera        = GetNode<PlayerCamera>("PlayerCamera");
        _camera.Player = this;
        this.SetupNodeTools();
    }

    public override void _PhysicsProcess(float delta) {
        var hRot = _camera.H.GlobalTransform.basis.GetEuler().y;

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
            case PlayerState.InAir: {
            }
                break;
            default:
                var msg = $"Unknown state: {_state}";
                Log.Logger.Error(msg);
                throw new ArgumentOutOfRangeException(msg);
        }

        Direction = Direction.Rotated(Vector3.Up, hRot).Normalized();
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
