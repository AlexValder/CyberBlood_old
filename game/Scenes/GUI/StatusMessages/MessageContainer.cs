using Godot;

namespace CyberBlood.Scenes.GUI.StatusMessages {
    public class MessageContainer : VBoxContainer {
        [Export(PropertyHint.ExpRange, "1, 10, 1")]
        private int _maxSize = 5;
        private readonly PackedScene _statusLabel;

        public MessageContainer() {
            _statusLabel = GD.Load<PackedScene>("res://Scenes/GUI/StatusMessages/StatusMessage.tscn");
        }

        public void DisplayMessage(string message) {
            var count = GetChildCount();
            if (count == _maxSize) {
                var child = GetChild(0);
                RemoveChild(child);
                child.QueueFree();
            }

            var label = _statusLabel.Instance<Label>();
            AddChild(label);
            label.Call("display", message);
        }
    }
}
