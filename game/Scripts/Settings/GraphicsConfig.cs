using System;
using System.Collections.Generic;
using Godot;

namespace CyberBlood.Scripts.Settings {
    public class GraphicsConfig : FileConfig {
        private const string QUALITY = "quality";
        private const string RESOLUTION = "resolution";
        private const string ANTIALIASING = "anti-aliasing";
        private const string FXAA = "fxaa";
        private const string WINDOW = "window";
        private const string WINDOW_MODE = "mode";

        private Viewport _viewport;

        public void SetViewport(Viewport viewport) {
            _viewport = viewport;
        }

        public GraphicsConfig(
            string path,
            string pass,
            bool useDefaults = false
        ) : base(path, pass, useDefaults, new Dictionary<string, Dictionary<string, object>> {
            [QUALITY] = new() {
                [RESOLUTION]   = Vector2.Zero,
                [ANTIALIASING] = (int)Viewport.MSAA.Msaa2x,
                [FXAA]         = true,
            },
            [WINDOW] = new() {
                [WINDOW_MODE] = (int)WindowMode.None,
            },
        }) { }

        public Vector2 Resolution {
            get {
                var saved = GetValue<Vector2>(QUALITY, RESOLUTION);
                // Vector2.Zero == native resolution
                return saved == Vector2.Zero ? OS.WindowSize : saved;
            }
            set {
                // Vector2.Zero == native resolution
                _viewport.Size = value == Vector2.Zero ? OS.WindowSize : value;
                SetValue(QUALITY, RESOLUTION, value);
            }
        }

        public Viewport.MSAA AntiAliasing {
            get => (Viewport.MSAA)GetValue<int>(QUALITY, ANTIALIASING);
            set {
                _viewport.Msaa = value;
                SetValue(QUALITY, ANTIALIASING, (int)value);
            }
        }

        public bool Fxaa {
            get => GetValue<bool>(QUALITY, FXAA);
            set {
                SetValue(QUALITY, FXAA, value);
                _viewport.Fxaa = value;
            }
        }

        public WindowMode WindowMode {
            get => (WindowMode)GetValue<int>(WINDOW, WINDOW_MODE);
            set {
                ApplyWindowMode(value);
                SetValue(QUALITY, ANTIALIASING, (int)value);
            }
        }

        private static void ApplyWindowMode(WindowMode value) {
            switch (value) {
                case WindowMode.None:
                    OS.WindowMaximized  = false;
                    OS.WindowBorderless = false;
                    OS.WindowFullscreen = false;
                    break;
                case WindowMode.Maximized:
                    OS.WindowMaximized  = true;
                    OS.WindowBorderless = false;
                    OS.WindowFullscreen = false;
                    break;
                case WindowMode.Borderless:
                    OS.WindowMaximized  = false;
                    OS.WindowBorderless = true;
                    OS.WindowFullscreen = false;
                    break;
                case WindowMode.Fullscreen:
                    OS.WindowMaximized  = false;
                    OS.WindowBorderless = false;
                    OS.WindowFullscreen = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public void ApplySettings() {
            var size = GetValue<Vector2>(QUALITY, RESOLUTION);
            _viewport.Size = size == Vector2.Zero ? OS.WindowSize : size;
            _viewport.Msaa = (Viewport.MSAA)GetValue<int>(QUALITY, ANTIALIASING);
            _viewport.Fxaa = GetValue<bool>(QUALITY, FXAA);
            ApplyWindowMode((WindowMode)GetValue<int>(WINDOW, WINDOW_MODE));
        }
    }
}
