using CyberBlood.Scripts.Settings.GodotSink;
using Godot;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Display;

namespace CyberBlood.Scripts.Settings {
    public class GameSettings : Node {
        private const string CONFIG_INI = "config.ini";
        private const string CONTROLS_INI = "control.ini";

        private static int s_connectedJoys = 0;

        public static bool JoyConnected => s_connectedJoys > 0;

        public static ControlsConfig Controls { get; }

        static GameSettings() {
            ConfigureLogger();
            Controls = LoadControls();
        }

        public override void _Ready() {
            Input.Singleton.Connect(
                "joy_connection_changed",
                this,
                nameof(ToggleJoystickConnection)
            );

            s_connectedJoys = Input.GetConnectedJoypads().Count;
        }

        private static void ConfigureLogger() {
            const string template =
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

            var config = new LoggerConfiguration()
                    #if DEBUG || EXPORTDEBUG
                    .WriteTo.GodotSink(outputTemplate: template)
                    .MinimumLevel.Debug()
                    #else
                    .WriteTo.Console(outputTemplate: template)
                    .MinimumLevel.Warning()
                    #endif
                ;

            Log.Logger = config.CreateLogger();
        }

        private static ControlsConfig LoadControls() {
            return ControlsConfig.LoadConfig(CONTROLS_INI, CONTROLS_INI);
        }

        private void ToggleJoystickConnection(int _, bool connected) {
            if (connected) {
                s_connectedJoys += 1;
            } else {
                s_connectedJoys -= 1;
            }
        }
    }
}
