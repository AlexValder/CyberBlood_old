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

#pragma warning disable 649
        [NodePath("timer")] private Timer _timer;
        [NodePath("tween")] private Tween _tween;
        [NodePath("arm")] private SpringArm _arm;
#pragma warning restore 649
        public SpringArm Arm => _arm;
        public Timer MovementTimer => _timer;

        private Player.Player _player;

        private const float MAX_UP = (float)(-85f * Math.PI / 180);
        private const float MAX_DOWN = (float)(-5f * Math.PI / 180);

        private static float IsInverted => GameSettings.JoyConnected && GameSettings.Controls.CameraInverted ? -1f : 1f;
        private static float VJoyAcceleration => GameSettings.Controls.CameraJoyRotateVertical;
        private static float HJoyAcceleration => GameSettings.Controls.CameraJoyRotateHorizontal;
        private static float VMouseAcceleration => GameSettings.Controls.CameraMouseRotateVertical;
        private static float HMouseAcceleration => GameSettings.Controls.CameraMouseRotateHorizontal;

        public override void _Ready() {
            this.SetupNodeTools();
            _player = GetParent<Player.Player>();

            SetAsToplevel(true);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (@event is InputEventMouseMotion mouse) {
                _tween.Stop(Arm);
                _timer.Start(5f);
                var rotation = Arm.Rotation;
                rotation.x -= mouse.Relative.y * HMouseAcceleration;
                rotation.x =  Mathf.Clamp(rotation.x, MAX_UP, MAX_DOWN);

                rotation.y -= mouse.Relative.x * VMouseAcceleration;
                rotation.y =  Mathf.Wrap(rotation.y, 0f, 2f * Mathf.Pi);

                Arm.Rotation = rotation;
            } else if (@event.IsActionPressed(CAMERA_CENTER)) {
                CenterCamera();
            }
        }

        private void CenterCamera(float duration = .5f) {
            _tween.InterpolateProperty(
                Arm,
                "rotation:y",
                null,
                Mathf.LerpAngle(Arm.Rotation.y, _player.Mesh.Rotation.y - Mathf.Pi / 2, 1f),
                duration,
                Tween.TransitionType.Quart
            );
            _tween.Start();
        }

        public override void _PhysicsProcess(float delta) {
            var start = false;
            var rot   = Arm.Rotation;
            if (Input.IsActionPressed(CAMERA_LEFT)) {
                start =  true;
                rot.y -= Input.GetActionStrength(CAMERA_LEFT) * HJoyAcceleration * IsInverted;
            } else if (Input.IsActionPressed(CAMERA_RIGHT)) {
                start =  true;
                rot.y += Input.GetActionStrength(CAMERA_RIGHT) * HJoyAcceleration * IsInverted;
            }
            rot.y = Mathf.Wrap(rot.y, 0f, 2f * Mathf.Pi);

            if (Input.IsActionPressed(CAMERA_UP)) {
                start =  true;
                rot.x -= Input.GetActionStrength(CAMERA_UP) * VJoyAcceleration * IsInverted;
            } else if (Input.IsActionPressed(CAMERA_DOWN)) {
                start =  true;
                rot.x += Input.GetActionStrength(CAMERA_DOWN) * VJoyAcceleration * IsInverted;
            }
            rot.x = Mathf.Clamp(rot.x, MAX_UP, MAX_DOWN);

            if (start) {
                _tween.Stop(Arm);
                _timer.Start(5f);
            }

            Arm.Rotation = rot;
        }

        private void _on_timer_timeout() {
            CenterCamera(2f);
        }

        public void Reset() {
            _tween.StopAll();
            var rot = Arm.Rotation;
            rot.y        = _player.Mesh.Rotation.y - Mathf.Pi / 2;
            Arm.Rotation = rot;
        }
    }
}
