using System;
using System.Collections.Generic;
using Godot;
using Serilog;

namespace CyberBlood.Scripts.Settings.Config {
    public abstract class FileConfig {
        private const string CHECKING = "checking";
        private const string DEFAULT_HASH = "hash";

        private readonly ConfigFile _config = new();
        private readonly string _path;
        private readonly string _pass;
        private Dictionary<string, Dictionary<string, object>> _current;
        private readonly Dictionary<string, Dictionary<string, object>> _defaults;

        public static T LoadConfig<T>(string path, string pass) where T : FileConfig {
            if (System.IO.File.Exists(path)) {
                return (T)Activator.CreateInstance(typeof(T), path, pass, false);
            }

            var config = (T)Activator.CreateInstance(typeof(T), path, pass, true);
            config.SaveConfigToFile();
            return config;
        }

        protected FileConfig(string path, string pass, bool useDefaults,
            Dictionary<string, Dictionary<string, object>> defaults) {
            _path     = path;
            _pass     = pass;
            _defaults = defaults;

            if (useDefaults) {
                _current = _defaults;
            } else {
                lock (_config) {
#if DEBUG || EXPORTDEBUG
                    var error = _config.Load(_path);
#else
                    Error error;
                    if (string.IsNullOrWhiteSpace(pass)) {
                        error = _config.Load(path);
                    } else {
                        error = _config.LoadEncryptedPass(path, pass);
                    }
#endif

                    if (error != Error.Ok) {
                        Log.Logger.Error(
                            "Failed to load {Path} with {Pass}, error: {Error}",
                            _path, _pass, error
                        );
                        throw new ConfigException(error);
                    }
                }

                _current = LoadConfig();
                SaveConfigToFile();
            }
        }

        protected T GetValue<T>(string section, string key) {
            if (_current.ContainsKey(section) && _current[section].ContainsKey(key)) {
                return (T)_current[section][key];
            }

            return (T)_defaults[section][key];
        }

        protected void SetValue<T>(string section, string key, T value) {
            lock (_config) {
                if (!_current.ContainsKey(section)) {
                    _current[section] = new Dictionary<string, object>();
                }

                _current[section][key] = value;
                _config.SetValue(section, key, value);
            }
        }

        protected void SaveConfigToFile() {
            lock (_config) {
                Dictionary2Config(_current, _config);
#if DEBUG || EXPORTDEBUG
                _config.Save(_path);
#else
                if (string.IsNullOrWhiteSpace(_pass)) {
                    _config.Save(_path);
                } else {
                    _config.SaveEncryptedPass(_path, _pass);
                }
#endif
            }
        }

        private Dictionary<string, Dictionary<string, object>> LoadDefaults() {
            lock (_config) {
                _config.Clear();
                Dictionary2Config(_defaults, _config);
                return LoadConfig();
            }
        }

        public void SetDefaults() {
            _current = LoadDefaults();
        }

        private Dictionary<string, Dictionary<string, object>> LoadConfig() {
            var current = new Dictionary<string, Dictionary<string, object>>(_defaults.Count);

            lock (_config) {
                foreach (var section in _defaults.Keys) {
                    if (_config.HasSection(section)) {
                        current[section] = new Dictionary<string, object>();
                        foreach (var key in _defaults[section].Keys) {
                            if (_config.HasSectionKey(section, key)) {
                                current[section][key] = _config.GetValue(section, key);
                            }
                        }
                    }
                }
            }

            return current;
        }

        private static void Dictionary2Config(
            Dictionary<string, Dictionary<string, object>> dict,
            ConfigFile config
        ) {
            config.Clear();

            foreach (var section in dict.Keys) {
                foreach (var key in dict[section].Keys) {
                    config.SetValue(section, key, dict[section][key]);
                }
            }
        }
    }
}
