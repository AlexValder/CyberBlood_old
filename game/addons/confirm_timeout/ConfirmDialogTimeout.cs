using System.Diagnostics;
using Godot;
using GodotCSToolbox;

namespace CyberBlood.addons.confirm_timeout {
    [Tool]
    public class ConfirmDialogTimeout : ConfirmationDialog {
        private readonly Timer _timer = new() {
            Autostart = false,
            OneShot   = true,
        };
        private Label _label;
        private int _lastValue = -1;

        [Signal]
        // ReSharper disable once InconsistentNaming
        // Reason: signal
        public delegate void cancel();

        [Export(PropertyHint.Range, "0,4098")] private float _waitTime = 15.5f;

        public override void _EnterTree() {
            AddChild(_timer);
        }

        public override void _Ready() {
            _label             = GetLabel();
            _label.Align       = Label.AlignEnum.Center;
            _label.Valign      = Label.VAlign.Center;
            _label.RectMinSize = new Vector2(400, 50);
            WindowTitle        = "";

            var error0 = GetCancel().Connect("button_up", this, nameof(EmitCancel));
            Debug.Assert(error0 == Error.Ok);
            var error1 = Connect("about_to_show", this, nameof(StartTimer));
            Debug.Assert(error1 == Error.Ok);
            var error2 = Connect("popup_hide", this, nameof(StopTimer));
            Debug.Assert(error2 == Error.Ok);
            var error3 = _timer.Connect("timeout", this, nameof(CancelConfirmation));
            Debug.Assert(error3 == Error.Ok);
        }

        public override void _Process(float _) {
            if (_lastValue != (int)_timer.TimeLeft) {
                _lastValue  = (int)_timer.TimeLeft;
                _label.Text = $"Confirm, or changing back in... {_lastValue}s";
            }
        }

        private void EmitCancel() {
            EmitSignal("cancel");
            _lastValue = 0;
        }

        private void CancelConfirmation() {
            SetProcess(false);
            Hide();
        }

        private void StartTimer() {
            SetProcess(true);
            _lastValue = -1;
            _timer.Start(_waitTime);
        }

        private void StopTimer() {
            SetProcess(false);
            _lastValue = -1;
            _timer.Stop();
        }
    }
}
