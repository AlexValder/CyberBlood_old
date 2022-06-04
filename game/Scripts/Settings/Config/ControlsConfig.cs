using System.Collections.Generic;

namespace CyberBlood.Scripts.Settings.Config {
    public class ControlsConfig : FileConfig {
        private const string CAMERA_JOYPAD = "camera_joypad";
        private const string ROTATE_H = "rotate_horizontal";
        private const string ROTATE_V = "rotate_vertical";
        private const string INVERTED = "inverted";
        private const string CAMERA_MOUSE = "camera_mouse";

        public float CameraJoyRotateHorizontal {
            get => GetValue<float>(CAMERA_JOYPAD, ROTATE_H);
            set => SetValue(CAMERA_JOYPAD, ROTATE_H, value);
        }

        public float CameraJoyRotateVertical {
            get => GetValue<float>(CAMERA_JOYPAD, ROTATE_V);
            set => SetValue(CAMERA_JOYPAD, ROTATE_V, value);
        }

        public bool CameraInverted {
            get => GetValue<bool>(CAMERA_JOYPAD, INVERTED);
            set => SetValue(CAMERA_JOYPAD, INVERTED, value);
        }

        public float CameraMouseRotateHorizontal {
            get => GetValue<float>(CAMERA_MOUSE, ROTATE_H);
            set => SetValue(CAMERA_MOUSE, ROTATE_H, value);
        }

        public float CameraMouseRotateVertical {
            get => GetValue<float>(CAMERA_MOUSE, ROTATE_V);
            set => SetValue(CAMERA_MOUSE, ROTATE_V, value);
        }

        public ControlsConfig(
            string path,
            string pass,
            bool useDefaults
        ) : base(path, pass, useDefaults, new Dictionary<string, Dictionary<string, object>> {
            [CAMERA_JOYPAD] = new() {
                [ROTATE_H] = 2f,
                [ROTATE_V] = 2f,
                [INVERTED] = false,
            },
            [CAMERA_MOUSE] = new() {
                [ROTATE_H] = .005f,
                [ROTATE_V] = .005f,
            }
        }) {
        }
    }
}
