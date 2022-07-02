using Godot;

namespace CyberBlood.Scenes.Entities.Player.States {
    public abstract class BaseState : Node {
        public bool Enabled { get; set; } = true;
        public Player Player { get; set; }

        public virtual void OnEntry() {
            // nothing specific
        }

        public virtual void OnExit() {
            // nothing specific
        }

        public virtual void HandleInput(InputEvent input) {
            // nothing specific
        }

        public virtual void HandleProcess(float delta) {
            // nothing specific
        }

        public virtual void HandlePhysicsProcess(float delta) {
            // nothing specific
        }
    }
}
