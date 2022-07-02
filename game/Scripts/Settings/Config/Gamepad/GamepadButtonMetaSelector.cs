using System;
using Godot;

namespace CyberBlood.Scripts.Settings.Config.Gamepad {
    /// <summary>
    /// Class to select for specific icons.
    /// </summary>
    public static class GamepadButtonMetaSelector {
        private static AtlasTexture s_button0;
        private static AtlasTexture s_button1;
        private static AtlasTexture s_button2;
        private static AtlasTexture s_button3;
        private static AtlasTexture s_button4;
        private static AtlasTexture s_button5;
        private static AtlasTexture s_button6;
        private static AtlasTexture s_button7;
        private static AtlasTexture s_button8;
        private static AtlasTexture s_button9;
        private static AtlasTexture s_button10;
        private static AtlasTexture s_button11;
        private static AtlasTexture s_button12;
        private static AtlasTexture s_button13;
        private static AtlasTexture s_button14;
        private static AtlasTexture s_button15;

        static GamepadButtonMetaSelector() {
            LoadIcons();
        }

        private static ButtonTheme s_theme = ButtonTheme.Xbox;

        public static ButtonTheme Theme {
            get => s_theme;
            set {
                s_theme = value;
                ChangeTheme(value);
            }
        }

        private const string ROOT = "res://Assets/icons/gui/controller/";

        public static string GetName(JoystickList index) =>
            index switch {
                JoystickList.Button0  => Theme == ButtonTheme.Xbox ? "A" : "Cross",
                JoystickList.Button1  => Theme == ButtonTheme.Xbox ? "B" : "Circle",
                JoystickList.Button2  => Theme == ButtonTheme.Xbox ? "X" : "Square",
                JoystickList.Button3  => Theme == ButtonTheme.Xbox ? "Y" : "Triangle",
                JoystickList.Button4  => Theme == ButtonTheme.Xbox ? "LB" : "L1",
                JoystickList.Button5  => Theme == ButtonTheme.Xbox ? "RB" : "R1",
                JoystickList.Button6  => Theme == ButtonTheme.Xbox ? "LT" : "L2",
                JoystickList.Button7  => Theme == ButtonTheme.Xbox ? "RT" : "R2",
                JoystickList.Button8  => Theme == ButtonTheme.Xbox ? "L" : "L3",
                JoystickList.Button9  => Theme == ButtonTheme.Xbox ? "R" : "R3",
                JoystickList.Button10 => Theme == ButtonTheme.Xbox ? "Back" : "Share",
                JoystickList.Button11 => Theme == ButtonTheme.Xbox ? "Start" : "Options",
                JoystickList.Button12 => "DPad Up",
                JoystickList.Button13 => "DPad Down",
                JoystickList.Button14 => "DPad Left",
                JoystickList.Button15 => "DPad Right",
                _                     => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };

        public static string GetName(int index) => GetName((JoystickList)index);

        public static Texture GetTexture(JoystickList index) =>
            index switch {
                JoystickList.Button0  => s_button0,
                JoystickList.Button1  => s_button1,
                JoystickList.Button2  => s_button2,
                JoystickList.Button3  => s_button3,
                JoystickList.Button4  => s_button4,
                JoystickList.Button5  => s_button5,
                JoystickList.Button6  => s_button6,
                JoystickList.Button7  => s_button7,
                JoystickList.Button8  => s_button8,
                JoystickList.Button9  => s_button9,
                JoystickList.Button10 => s_button10,
                JoystickList.Button11 => s_button11,
                JoystickList.Button12 => s_button12,
                JoystickList.Button13 => s_button13,
                JoystickList.Button14 => s_button14,
                JoystickList.Button15 => s_button15,
                _                     => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };

        public static Texture GetTexture(int index) => GetTexture((JoystickList)index);

        private static void LoadIcons() {
            s_button0  = GD.Load<AtlasTexture>(ROOT + "button00.tres");
            s_button1  = GD.Load<AtlasTexture>(ROOT + "button01.tres");
            s_button2  = GD.Load<AtlasTexture>(ROOT + "button02.tres");
            s_button3  = GD.Load<AtlasTexture>(ROOT + "button03.tres");
            s_button4  = GD.Load<AtlasTexture>(ROOT + "button04.tres");
            s_button5  = GD.Load<AtlasTexture>(ROOT + "button05.tres");
            s_button6  = GD.Load<AtlasTexture>(ROOT + "button06.tres");
            s_button7  = GD.Load<AtlasTexture>(ROOT + "button07.tres");
            s_button8  = GD.Load<AtlasTexture>(ROOT + "button08.tres");
            s_button9  = GD.Load<AtlasTexture>(ROOT + "button09.tres");
            s_button10 = GD.Load<AtlasTexture>(ROOT + "button10.tres");
            s_button11 = GD.Load<AtlasTexture>(ROOT + "button11.tres");
            s_button12 = GD.Load<AtlasTexture>(ROOT + "button12.tres");
            s_button13 = GD.Load<AtlasTexture>(ROOT + "button13.tres");
            s_button14 = GD.Load<AtlasTexture>(ROOT + "button14.tres");
            s_button15 = GD.Load<AtlasTexture>(ROOT + "button15.tres");
        }

        private static void ChangeTheme(ButtonTheme value) {
            var path    = value == ButtonTheme.Xbox ? $"{ROOT}svg/xbox.svg" : $"{ROOT}svg/playstation.svg";
            var current = GD.Load<Texture>(path);
            s_button0.Atlas  = current;
            s_button0.Atlas  = current;
            s_button1.Atlas  = current;
            s_button2.Atlas  = current;
            s_button3.Atlas  = current;
            s_button4.Atlas  = current;
            s_button5.Atlas  = current;
            s_button6.Atlas  = current;
            s_button7.Atlas  = current;
            s_button8.Atlas  = current;
            s_button9.Atlas  = current;
            s_button10.Atlas = current;
            s_button11.Atlas = current;
        }
    }
}
