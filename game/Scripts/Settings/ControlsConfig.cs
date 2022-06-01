using System.Collections.Generic;

namespace CyberBlood.Scripts.Settings {
    public class ControlsConfig : FileConfig {
        private const string CAMERA = "camera";
        private const string ROTATE_H = "rotate_horizontal";
        private const string ROTATE_V = "rotate_vertical";
        private const string INVERTED = "inverted";

        public float CameraRotateHorizontal {
            get => GetValue<float>(CAMERA, ROTATE_H);
            set => SetValue(CAMERA, ROTATE_H, value);
        }

        public float CameraRotateVertical {
            get => GetValue<float>(CAMERA, ROTATE_V);
            set => SetValue(CAMERA, ROTATE_V, value);
        }

        public bool CameraInverted {
            get => GetValue<bool>(CAMERA, INVERTED);
            set => SetValue(CAMERA, INVERTED, value);
        }

        public ControlsConfig(
            string path,
            string pass,
            bool useDefaults = false
        ) : base(path, pass, useDefaults, new Dictionary<string, Dictionary<string, object>> {
            [CAMERA] = new() {
                [ROTATE_H] = 1f,
                [ROTATE_V] = 1f,
                [INVERTED] = false,
            },
        }) { }
    }
}
