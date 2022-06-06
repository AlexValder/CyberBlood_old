#if TOOLS
using Godot;

namespace CyberBlood.addons.confirm_timeout {
    [Tool]
    public class Plugin : CustomTypePlugin {
        private const string CONFIRM_DIALOG = "ConfirmDialogTimeout";
        private const string BASE_TYPE = "ConfirmationDialog";
        private const string SCRIPT_PATH = "res://addons/confirm_timeout/ConfirmDialogTimeout.cs";
        private const string ICON_PATH = "res://addons/confirm_timeout/icon.svg";

        public Plugin() : base(CONFIRM_DIALOG, BASE_TYPE, SCRIPT_PATH, ICON_PATH) { }
    }
}
#endif
