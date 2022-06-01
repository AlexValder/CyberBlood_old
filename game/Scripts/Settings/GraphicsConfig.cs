using System.Collections.Generic;
using Godot;

namespace CyberBlood.Scripts.Settings {
    public class GraphicsConfig : FileConfig {
        private const string GRAPHICS = "graphics";
        private const string RESOLUTION = "resolution";
        private const string ANTIALIASING = "anti-aliasing";
        private const string FXAA = "fxaa";

        private Viewport _viewport;

        public void SetViewport(Viewport viewport) {
            _viewport = viewport;
        }

        public GraphicsConfig(
            string path,
            string pass,
            bool useDefaults = false
        ) : base(path, pass, useDefaults, new Dictionary<string, Dictionary<string, object>> {
            [GRAPHICS] = new() {
                [RESOLUTION]   = Vector2.Zero,
                [ANTIALIASING] = (int)Viewport.MSAA.Msaa2x,
                [FXAA]         = true,
            },
        }) { }

        public Vector2 Resolution {
            get {
                var saved = GetValue<Vector2>(GRAPHICS, RESOLUTION);
                // Vector2.Zero == native resolution
                return saved == Vector2.Zero ? OS.WindowSize : saved;
            }
            set {
                // Vector2.Zero == native resolution
                _viewport.Size = value == Vector2.Zero ? OS.WindowSize : value;
                SetValue(GRAPHICS, RESOLUTION, value);
            }
        }

        public Viewport.MSAA AntiAliasing {
            get => (Viewport.MSAA)GetValue<int>(GRAPHICS, ANTIALIASING);
            set {
                _viewport.Msaa = value;
                SetValue(GRAPHICS, ANTIALIASING, (int)value);
            }
        }

        public bool Fxaa {
            get => GetValue<bool>(GRAPHICS, FXAA);
            set {
                SetValue(GRAPHICS, FXAA, value);
                _viewport.Fxaa = value;
            }
        }

        public void ApplySettings() {
            var size = GetValue<Vector2>(GRAPHICS, RESOLUTION);
            _viewport.Size = size == Vector2.Zero ? OS.WindowSize : size;
            _viewport.Msaa = (Viewport.MSAA)GetValue<int>(GRAPHICS, ANTIALIASING);
            _viewport.Fxaa = GetValue<bool>(GRAPHICS, FXAA);
        }
    }
}
