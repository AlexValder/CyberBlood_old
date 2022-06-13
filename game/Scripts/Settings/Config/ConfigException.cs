using System;
using Godot;

namespace CyberBlood.Scripts.Settings.Config {
    public class ConfigException : Exception {
        public Error Error { get; }

        public ConfigException(Error error) : this(error, $"Failed with error: {error}") {
        }

        public ConfigException(Error error, string msg) : base(msg) {
            Error = error;
        }
    }
}