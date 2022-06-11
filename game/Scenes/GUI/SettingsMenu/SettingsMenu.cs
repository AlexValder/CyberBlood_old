using System.Collections.Generic;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.GUI.SettingsMenu {
    public class SettingsMenu : PopupDialog, IConfigMenu {
#pragma warning disable 649
        [NodePath("vbox/tabs")] private TabContainer _tabContainer;
        [NodePath("vbox/tabs/Game")] private IConfigMenu _game;
        [NodePath("vbox/tabs/Graphics")] private IConfigMenu _graphics;
        [NodePath("vbox/tabs/Controls")] private IConfigMenu _controls;

        private IReadOnlyList<IConfigMenu> _tabs;
#pragma warning restore 649

        bool IConfigMenu.NeedsConfirmation => false;

        public override void _Ready() {
            this.SetupNodeTools();

            _tabs = new List<IConfigMenu> {
                _game, _graphics, _controls
            };

            _tabContainer.SetTabDisabled(0, true);
            _tabContainer.CurrentTab = 1;

            SetupFromConfig();

            GetTree().Root.Connect("size_changed", this, nameof(_on_SettingsMenu_resized));
        }

        public void SetupFromConfig() {
            _tabs[_tabContainer.CurrentTab].SetupFromConfig();
        }

        public void ApplyCurrentSettings() {
            _tabs[_tabContainer.CurrentTab].ApplyCurrentSettings();
        }

        public void SetDefaults() {
            foreach (var tab in _tabs) {
                tab.SetDefaults();
            }
        }

        private void _on_ok_button_up() {
            ApplyCurrentSettings();
            if (!_tabs[_tabContainer.CurrentTab].NeedsConfirmation) {
                Hide();
            }
        }

        private void _on_apply_button_up() {
            ApplyCurrentSettings();
        }

        private void _on_cancel_button_up() {
            SetupFromConfig();
            this.Hide();
        }

        private void _on_defaults_button_up() {
            var tab = _tabs[_tabContainer.CurrentTab];
            tab.SetDefaults();
            tab.SetupFromConfig();
        }

        private void _on_SettingsMenu_resized() {
            var size = GetTree().Root.Size;
            RectSize = size * .6f;
            PopupCentered();
        }
    }
}
