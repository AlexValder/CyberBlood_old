#if TOOLS
using Godot;

namespace CyberBlood.addons.playable_spatial {
    [Tool]
    public class Plugin : CustomTypePlugin {
        private const string PLAYABLE_SPATIAL = "PlayableSpatial";
        private const string BASE_TYPE = "Spatial";
        private const string SCRIPT_PATH = "res://addons/playable_spatial/PlayableSpatial.cs";
        private const string ICON_PATH = "res://addons/playable_spatial/icon.svg";

        public Plugin() : base(PLAYABLE_SPATIAL, BASE_TYPE, SCRIPT_PATH, ICON_PATH) { }
    }
}
#endif
