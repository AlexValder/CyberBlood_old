using Godot;

namespace CyberBlood.Scripts.Settings.Config.Gamepad; 

public interface IHasSticks {
    IBuild Left();
    IBuild Up();
    IBuild Right();
    IBuild Down();
}

public interface IBuild {
    InputEventJoypadMotion Build();
}
