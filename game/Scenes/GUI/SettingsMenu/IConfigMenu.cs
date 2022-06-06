namespace CyberBlood.Scenes.GUI.SettingsMenu {
    public interface IConfigMenu {
        bool NeedsConfirmation { get; }
        void SetupFromConfig();
        void ApplyCurrentSettings();
        void SetDefaults();
    }
}
