#if TOOLS
using Godot;

namespace CyberBlood.addons.playable_spatial {
    [Tool]
    public class Plugin : EditorPlugin {
        private const string PLAYABLE_SPATIAL = "PlayableSpatial";
        private const string SCRIPT_PATH = "res://addons/playable_spatial/PlayableSpatial.cs";
        private const string ICON_PATH = "res://addons/playable_spatial/icon.svg";

        public override void _EnterTree() {
            var script = GD.Load<Script>(SCRIPT_PATH);
            var icon   = GD.Load<Texture>(ICON_PATH);
            AddCustomType(
                PLAYABLE_SPATIAL,
                "Spatial",
                script,
                icon
            );
        }

        public override void _ExitTree() {
            RemoveCustomType(PLAYABLE_SPATIAL);
        }
    }
}
#endif
