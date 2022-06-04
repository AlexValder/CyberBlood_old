using System;
using CyberBlood.Scripts.Settings;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.Entities {
    public class PlayerCamera : Spatial {
        private const string CAMERA_UP = "camera_up";
        private const string CAMERA_LEFT = "camera_left";
        private const string CAMERA_RIGHT = "camera_right";
        private const string CAMERA_DOWN = "camera_down";
        private const string CAMERA_CENTER = "camera_center";

        private const float ROTATION_SPEED = 0.15f;

#pragma warning disable 649
        [NodePath("h")] private Spatial _h;
        [NodePath("h/v")] private Spatial _v;
        [NodePath("h/v/ClippedCamera")] private ClippedCamera _camera;
        [NodePath("PlayerMoves")] private Timer _timer;
#pragma warning restore 649
        public Spatial H => _h;
        public Spatial V => _v;

        private const float MAX_UP = (float)(-65f * Math.PI / 180);
        private const float MAX_DOWN = (float)(5f * Math.PI / 180);

        private Player _player;
        private float _cameraV;
        private float _cameraH;

        private static float IsInverted => GameSettings.JoyConnected && GameSettings.Controls.CameraInverted ? -1f : 1f;
        private static float VJoyAcceleration => GameSettings.Controls.CameraJoyRotateVertical;
        private static float HJoyAcceleration => GameSettings.Controls.CameraJoyRotateHorizontal;
        private static float VMouseAcceleration => GameSettings.Controls.CameraMouseRotateVertical;
        private static float HMouseAcceleration => GameSettings.Controls.CameraMouseRotateHorizontal;

        public override void _Ready() {
            this.SetupNodeTools();
            _player = GetParent<Player>();
            _camera.AddException(_player);

            _camera.Rotation = new Vector3(
                _camera.Rotation.x,
                0,
                _camera.Rotation.z
            );
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (!GameSettings.JoyConnected && @event is InputEventMouseMotion mouse) {
                _timer.Start();
                _cameraH -= mouse.Relative.x * HMouseAcceleration;
                _cameraV -= mouse.Relative.y * VMouseAcceleration;
            }
        }

        public override void _PhysicsProcess(float delta) {
            if (GameSettings.JoyConnected) {
                var start = false;
                if (Input.IsActionPressed(CAMERA_LEFT)) {
                    start    = true;
                    _cameraH = Input.GetActionStrength(CAMERA_LEFT) * HJoyAcceleration * IsInverted * delta;
                } else if (Input.IsActionPressed(CAMERA_RIGHT)) {
                    start    = true;
                    _cameraH = -Input.GetActionStrength(CAMERA_RIGHT) * HJoyAcceleration * IsInverted * delta;
                }

                if (Input.IsActionPressed(CAMERA_UP)) {
                    start    = true;
                    _cameraV = Input.GetActionStrength(CAMERA_UP) * VJoyAcceleration * IsInverted * delta;
                } else if (Input.IsActionPressed(CAMERA_DOWN)) {
                    start    = true;
                    _cameraV = -Input.GetActionStrength(CAMERA_DOWN) * VJoyAcceleration * IsInverted * delta;
                }

                if (start) {
                    _timer.Start();
                }
            }

            if (_timer.IsStopped()) {
                var meshFront = -_player.MeshRotation;
                var autoRotateSpeed = (Mathf.Pi - meshFront.AngleTo(
                    _h.GlobalTransform.basis.z
                )) * ROTATION_SPEED * _player.Velocity.Length();

                _h.Rotation = new Vector3(
                    _h.Rotation.x,
                    Mathf.LerpAngle(
                        _h.Rotation.y,
                        _player.MeshRotation.y,
                        delta * autoRotateSpeed
                    ),
                    _h.Rotation.z
                );
            } else {
                _h.RotateY(_cameraH);
            }

            _v.Rotation = new Vector3(
                x: Mathf.Clamp(_v.Rotation.x + _cameraV, MAX_UP, MAX_DOWN),
                y: _v.Rotation.y,
                z: _v.Rotation.z
            );

            _cameraH = 0;
            _cameraV = 0;
        }
    }
}