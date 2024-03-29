using CyberBlood.addons.confirm_timeout;
using CyberBlood.Scripts.Settings;
using CyberBlood.Scripts.Settings.Config;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.Scenes.GUI.SettingsMenu {
    public class Graphics : VBoxContainer, IConfigMenu {
#pragma warning disable 649
        [NodePath("grid0/resolution")] private OptionButton _resolution;
        [NodePath("grid0/antialiasing")] private OptionButton _antialiasing;
        [NodePath("grid0/fxaa")] private CheckBox _fxaa;
        [NodePath("grid0/vsync")] private CheckBox _vsync;
        [NodePath("grid1/mode")] private OptionButton _mode;
        [NodePath("confirm")] private ConfirmDialogTimeout _dialog;
#pragma warning restore 649
        private bool _wasChanged;

        private Vector2 _prevRes;
        private Viewport.MSAA _prevMsaa;
        private bool _prevFxaa;
        private bool _prevVsync;
        private WindowMode _prevMode;

        public bool NeedsConfirmation => _dialog.Visible;

        public override void _Ready() {
            this.SetupNodeTools();
        }

        private void SetWasChanged() {
            _wasChanged = true;
        }

        public void SetupFromConfig() {
            _wasChanged = false;
            PopulateAndSetResolution(_resolution);

            _antialiasing.Selected = (int)GameSettings.Graphics.AntiAliasing;
            _fxaa.Pressed          = GameSettings.Graphics.Fxaa;
            _vsync.Pressed         = GameSettings.Graphics.Vsync;
            _mode.Selected         = (int)GameSettings.Graphics.WindowMode;
        }

        private static void PopulateAndSetResolution(OptionButton options) {
            options.Clear();
            options.AddItem("Native", 0);

            var resolutions = GraphicsConfig.GetScreenResolution();

            var id  = 0;
            var res = GameSettings.Graphics.Resolution;

            for (var i = 0; i < resolutions.Count; ++i) {
                options.AddItem($"{resolutions[i].x}x{resolutions[i].y}", i + 1);

                if (res == resolutions[i]) {
                    id = i + 1;
                }
            }

            options.Selected = id;
        }

        public void ApplyCurrentSettings() {
            if (!_wasChanged) {
                return;
            }

            _wasChanged = false;

            var gr = GameSettings.Graphics;

            _prevMsaa       = gr.AntiAliasing;
            gr.AntiAliasing = (Viewport.MSAA)_antialiasing.Selected;
            _prevFxaa       = gr.Fxaa;
            gr.Fxaa         = _fxaa.Pressed;
            _prevVsync      = gr.Vsync;
            gr.Vsync        = _vsync.Pressed;
            _prevMode       = gr.WindowMode;
            gr.WindowMode   = (WindowMode)_mode.Selected;

            _prevRes = gr.Resolution;
            var id = _resolution.Selected;
            if (id == 0) {
                // native resolution
                gr.Resolution = Vector2.Zero;
            } else {
                var text = _resolution.GetItemText(id);
                var nums = text.Split('x');
                gr.Resolution = new Vector2(float.Parse(nums[0]), float.Parse(nums[1]));
            }

            gr.ApplySettings();

            _dialog.PopupCentered();
        }

        public void SetDefaults() {
            GameSettings.Graphics.SetDefaults();
            GameSettings.Graphics.SaveConfigToFile();
            SetupFromConfig();
        }

        private void _on_confirm_confirmed() {
            GameSettings.Graphics.SaveConfigToFile();
        }

        private void _on_confirm_cancel() {
            var gr = GameSettings.Graphics;

            gr.Vsync        = _prevVsync;
            gr.Fxaa         = _prevFxaa;
            gr.AntiAliasing = _prevMsaa;
            gr.WindowMode   = _prevMode;
            gr.Resolution   = _prevRes;

            SetupFromConfig();
        }

        private void _on_timer_timeout() {
            _dialog.Hide();
            SetupFromConfig();
        }
    }
}
