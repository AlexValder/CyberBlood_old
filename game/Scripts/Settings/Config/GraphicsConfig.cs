using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace CyberBlood.Scripts.Settings.Config {
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
                [WINDOW_MODE] = (int)WindowMode.Windowed,
            },
        }) { }

        public Vector2 Resolution {
            get => GetValue<Vector2>(QUALITY, RESOLUTION);
            set {
                // Vector2.Zero == native resolution
                _viewport.Size = value == Vector2.Zero ? OS.GetScreenSize() : value;
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
                SetValue(WINDOW, WINDOW_MODE, (int)value);
            }
        }

        private static void ApplyWindowMode(WindowMode value) {
            OS.WindowResizable = false;
            OS.WindowMaximized = true;

            switch (value) {
                case WindowMode.Windowed:
                    OS.WindowBorderless = false;
                    OS.WindowFullscreen = false;
                    break;
                case WindowMode.Borderless:
                    OS.WindowBorderless = true;
                    OS.WindowFullscreen = false;
                    break;
                case WindowMode.Fullscreen:
                    OS.WindowBorderless = false;
                    OS.WindowFullscreen = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public override void ApplySettings() {
            var size = GetValue<Vector2>(QUALITY, RESOLUTION);
            _viewport.Size = size == Vector2.Zero ? OS.GetScreenSize() : size;
            _viewport.Msaa = (Viewport.MSAA)GetValue<int>(QUALITY, ANTIALIASING);
            _viewport.Fxaa = GetValue<bool>(QUALITY, FXAA);
            ApplyWindowMode((WindowMode)GetValue<int>(WINDOW, WINDOW_MODE));
        }

        public static IList<Vector2> GetScreenResolution() {
            var current = OS.GetScreenSize();

            var possible = new HashSet<Vector2> {
                current,
                new(3840, 2160),
                new(2560, 1440),
                new(1920, 1080),
                new(1600, 900),
                new(1536, 864),
                new(1440, 900),
                new(1366, 768),
                new(1366, 760),
                new(1280, 1024),
                new(1280, 720),
                new(1024, 768),
            };

            var diagonal = current.Length();
            return possible.Where(res => res.Length() <= diagonal).ToList();
        }
    }
}
