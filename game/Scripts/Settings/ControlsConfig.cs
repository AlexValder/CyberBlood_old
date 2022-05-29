using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Serilog;

namespace CyberBlood.Scripts.Settings {
    public class ControlsConfig {
        private static readonly Dictionary<string, Dictionary<string, object>> s_defaults =
            new() {
                ["camera"] = new Dictionary<string, object> {
                    ["rotate_horizontal"]  = 1f,
                    ["rotate_vertical"] = 1f,
                    ["inverted"] = false,
                },
            };

        public float CameraRotateHorizontal {
            get => GetValue<float>("camera", "rotate_horizontal");
            set => SetValue("camera", "rotate_horizontal", value);
        }

        public float CameraRotateVertical {
            get => GetValue<float>("camera", "rotate_vertical");
            set => SetValue("camera", "rotate_vertical", value);
        }

        public bool CameraInverted {
            get => GetValue<bool>("camera", "inverted");
            set => SetValue("camera", "inverted", value);
        }

        private Dictionary<string, Dictionary<string, object>> _current;
        private readonly ConfigFile _config = new();
        private readonly string _path;
        private readonly string _pass;

        public static ControlsConfig LoadConfig(string path, string pass) {
            if (System.IO.File.Exists(path)) {
                return new ControlsConfig(path, pass);
            }

            var config = new ControlsConfig(path, pass, true);
            config.SaveConfigToFile();
            return config;
        }

        private ControlsConfig(string path, string pass = "", bool useDefaults = false) {
            var actions = InputMap.GetActions();

            _path = path;
            _pass = pass;

            if (useDefaults) {
                _current = s_defaults;
            } else {
                lock (_config) {
                    #if DEBUG
                    var error = _config.Load(path);
                    #else
                    var error = _config.LoadEncryptedPass(path, pass);
                    #endif

                    if (error != Error.Ok) {
                        Log.Logger.Error(
                            "Failed to load {Path} with {Pass}, error: {Error}",
                            path, pass, error
                        );
                        throw new ConfigException(error);
                    }
                }

                _current = LoadConfig();
            }
        }

        private T GetValue<T>(string section, string key) {
            if (_current.ContainsKey(section) && _current[section].ContainsKey(key)) {
                return (T)_current[section][key];
            }

            return (T)s_defaults[section][key];
        }

        private void SetValue<T>(string section, string key, T value) {
            Debug.Assert(value != null, nameof(value) + " != null");

            lock (_config) {
                if (!_current.ContainsKey(section)) {
                    _current[section] = new Dictionary<string, object>();
                }

                _current[section][key] = value;
                _config.SetValue(section, key, value);
            }
        }

        private Dictionary<string, Dictionary<string, object>> LoadConfig() {
            var current = new Dictionary<string, Dictionary<string, object>>(s_defaults.Count);

            lock (_config) {
                foreach (var section in s_defaults.Keys) {
                    if (_config.HasSection(section)) {
                        current[section] = new Dictionary<string, object>();
                        foreach (var key in s_defaults[section].Keys) {
                            if (_config.HasSectionKey(section, key)) {
                                current[section][key] = _config.GetValue(section, key);
                            }
                        }
                    }
                }
            }

            return current;
        }

        public void SetDefaults() {
            lock (_config) {
                _config.Clear();
                Dictionary2Config(s_defaults, _config);
                _current = LoadConfig();
            }
        }

        private void Dictionary2Config(Dictionary<string, Dictionary<string, object>> dict, ConfigFile config) {
            config.Clear();

            foreach (var section in dict.Keys) {
                foreach (var key in dict[section].Keys) {
                    config.SetValue(section, key, dict[section][key]);
                }
            }
        }

        public void SaveConfigToFile() {
            lock (_config) {
                Dictionary2Config(_current, _config);
                #if DEBUG
                _config.Save(_pass);
                #else
                    _config.SaveEncryptedPass(_path, _pass);
                #endif
            }
        }
    }
}
