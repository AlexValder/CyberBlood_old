#if TOOLS
using Godot;

namespace CyberBlood.addons {
    public abstract class CustomTypePlugin : EditorPlugin {
        private readonly string _newType;
        private readonly string _baseType;
        private readonly string _scriptPath;
        private readonly string _iconPath;

        protected CustomTypePlugin(
            string typeName,
            string baseName,
            string script,
            string icon
        ) {
            _newType    = typeName;
            _baseType   = baseName;
            _scriptPath = script;
            _iconPath   = icon;
        }

        public override void _EnterTree() {
            var script = GD.Load<Script>(_scriptPath);
            var icon   = GD.Load<Texture>(_iconPath);
            AddCustomType(
                _newType,
                _baseType,
                script,
                icon
            );
        }

        public override void _ExitTree() {
            RemoveCustomType(_newType);
        }
    }
}
#endif
