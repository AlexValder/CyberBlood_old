using System;
using CyberBlood.Scripts.Settings.Config;
using Godot;

namespace CyberBlood.Scripts.Utils; 

public class MouseKeyboardButton {
    public KeyList KeyButton { get; }
    public MouseButtons MouseButtons { get; }

    public MouseKeyboardButton(InputEvent e) {
        switch (e) {
            case InputEventMouseButton mb:
                KeyButton    = 0;
                MouseButtons = (MouseButtons)(-mb.ButtonIndex);
                break;
            case InputEventKey key:
                KeyButton    = (KeyList)key.Scancode;
                MouseButtons = 0;
                break;
            default:
                throw new ArgumentException(nameof(e));
        }
    }

    public MouseKeyboardButton(KeyList key) {
        KeyButton    = key;
        MouseButtons = 0;
    }

    public MouseKeyboardButton(MouseButtons mouse) {
        MouseButtons = mouse;
        KeyButton    = 0;
    }
    
    public InputEvent GetInputEvent() {
        if (KeyButton == 0) {
            // mouse
            var mb = new InputEventMouseButton {
                ButtonIndex = -(int)MouseButtons,
            };
            return mb;
        }
        
        // keyboard
        var key = new InputEventKey {
            Scancode = (uint)KeyButton,
        };
        return key;
    }

    public override string ToString() {
        if (KeyButton == 0) {
            return ControlsConfig.MouseButtonString(-(int)MouseButtons);
        }

        return OS.GetScancodeString((uint)KeyButton);
    }
}
