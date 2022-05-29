using Godot;
using System;
using CyberBlood.Scripts.Settings;
using GodotCSToolbox;

public class PlayerCamera : Spatial {
    private const string CAMERA_UP = "camera_up";
    private const string CAMERA_LEFT = "camera_left";
    private const string CAMERA_RIGHT = "camera_right";
    private const string CAMERA_DOWN = "camera_down";
    private const string CAMERA_CENTER = "camera_center";

#pragma warning disable 649
    [NodePath("h")] private Spatial _h;
    [NodePath("h/v")] private Spatial _v;
    [NodePath("h/v/ClippedCamera")] private ClippedCamera _camera;
    [NodePath("PlayerMoves")] private Timer _timer;
#pragma warning restore 649

    private Player _player;
    public Player Player {
        get => _player;
        set {
            if (_player != null) {
                _camera.RemoveException(_player);
            }

            _player      = value;
            _camera.AddException(_player);
        }
    }

    public Spatial H => _h;
    public Spatial V => _v;

    private const float MAX_UP = (float)(-65f * Math.PI / 180);
    private const float MAX_DOWN = (float)(5f * Math.PI / 180);
    private float _cameraV = 0f;
    private float _cameraH = 0f;

    private static float IsInverted => GameSettings.Controls.CameraInverted ? -1f : 1f;
    private static float VAcceleration => GameSettings.Controls.CameraRotateVertical;
    private static float HAcceleration => GameSettings.Controls.CameraRotateHorizontal;

    public override void _Ready() {
        this.SetupNodeTools();
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (!GameSettings.JoyConnected && @event is InputEventMouseMotion mouse) {
            // _timer.Start();
            _cameraH += mouse.Relative.x * HAcceleration * IsInverted;
            _cameraV += mouse.Relative.y * VAcceleration * IsInverted;
        }
    }

    public override void _PhysicsProcess(float delta) {
        if (GameSettings.JoyConnected) {
            if (Input.IsActionPressed(CAMERA_LEFT)) {
                _cameraH = Input.GetActionStrength(CAMERA_LEFT) * HAcceleration * delta;
            } else if (Input.IsActionPressed(CAMERA_RIGHT)) {
                _cameraH = -Input.GetActionStrength(CAMERA_RIGHT) * HAcceleration * delta;
            }

            if (Input.IsActionPressed(CAMERA_UP)) {
                _cameraV = -Input.GetActionStrength(CAMERA_UP) * VAcceleration * delta;
            } else if (Input.IsActionPressed(CAMERA_DOWN)) {
                _cameraV = Input.GetActionStrength(CAMERA_DOWN) * VAcceleration * delta;
            }
        }

        _v.Rotation = new Vector3(
            x: Mathf.Clamp(_v.Rotation.x + _cameraV, MAX_UP, MAX_DOWN),
            y: _v.Rotation.y,
            z: _v.Rotation.z
        );
        // var meshFront = Player.GlobalTransform.basis.z;

        _h.RotateY(_cameraH);
        _cameraH = 0;
        _cameraV = 0;
    }
}
