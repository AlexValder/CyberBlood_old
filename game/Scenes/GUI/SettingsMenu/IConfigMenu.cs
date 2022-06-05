namespace CyberBlood.Scenes.GUI.SettingsMenu {
    public interface IConfigMenu {
        void SetupFromConfig();
        void ApplyCurrentSettings();
        void SetDefaults();
    }
}
