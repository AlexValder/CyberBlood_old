using Godot;

namespace CyberBlood.Scripts.Settings.Config.Gamepad {
    /// <summary>
    /// Static factory to create controller button events.
    /// Note: It uses Xbox controller button names, but actually returns generic events. 
    /// </summary>
    public static class GamepadButtonEventFactory {
        public static int IndexA => (int)JoystickList.Button0;
        public static int IndexB => (int)JoystickList.Button1;
        public static int IndexX => (int)JoystickList.Button2;
        public static int IndexY => (int)JoystickList.Button3;
        public static int IndexLB => (int)JoystickList.Button4;
        public static int IndexRB => (int)JoystickList.Button5;
        public static int IndexLT => (int)JoystickList.Button6;
        public static int IndexBT => (int)JoystickList.Button7;
        public static int IndexLeft => (int)JoystickList.Button8;
        public static int IndexRight => (int)JoystickList.Button9;
        public static int IndexBack => (int)JoystickList.Button10;
        public static int IndexStart => (int)JoystickList.Button11;
        public static int IndexDUp => (int)JoystickList.Button12;
        public static int IndexDDown => (int)JoystickList.Button13;
        public static int IndexDLeft => (int)JoystickList.Button14;
        public static int IndexDRight => (int)JoystickList.Button15;

        public static InputEventJoypadButton ForIndex(int index) => new() {
            ButtonIndex = index,
        };

        public static InputEventJoypadButton ForIndex(JoystickList index) => ForIndex((int)index);

        public static InputEventJoypadButton A() => new() { ButtonIndex = IndexA };

        public static InputEventJoypadButton B() => new() { ButtonIndex = IndexB };

        public static InputEventJoypadButton X() => new() { ButtonIndex = IndexX };

        public static InputEventJoypadButton Y() => new() { ButtonIndex = IndexY };

        public static InputEventJoypadButton LB() => new() { ButtonIndex = IndexLB };

        public static InputEventJoypadButton RB() => new() { ButtonIndex = IndexRB };

        public static InputEventJoypadButton LT() => new() { ButtonIndex = IndexLT };

        public static InputEventJoypadButton RT() => new() { ButtonIndex = IndexBT };

        public static InputEventJoypadButton LeftStick() => new() { ButtonIndex = IndexLeft };

        public static InputEventJoypadButton RightStick() => new() { ButtonIndex = IndexRight };

        public static InputEventJoypadButton Back() => new() { ButtonIndex = IndexBack };

        public static InputEventJoypadButton Start() => new() { ButtonIndex = IndexStart };

        public static InputEventJoypadButton DPadUp() => new() { ButtonIndex = IndexDUp };

        public static InputEventJoypadButton DPadDown() => new() { ButtonIndex = IndexDDown };

        public static InputEventJoypadButton DPadLeft() => new() { ButtonIndex = IndexDLeft };

        public static InputEventJoypadButton DPadRight() => new() { ButtonIndex = IndexDRight };
    }
}
