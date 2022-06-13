using System.Diagnostics;
using Godot;

namespace CyberBlood.Scripts.Settings.Config.Gamepad;

public class GamepadStickEventBuilder : IHasSticks, IBuild {
    private enum Sticks {
        Invalid = -1,
        Left = 0,
        Right = 1,
    }

    private enum Direction {
        Invalid = -1,
        Horizontal = 0,
        Vertical = 1,
    }

    private Sticks _stick = Sticks.Invalid;
    private Direction _direction = Direction.Invalid;
    private float _sign;

    public IHasSticks LeftStick() {
        _stick = Sticks.Left;
        return this;
    }

    public IHasSticks RightStick() {
        _stick = Sticks.Right;
        return this;
    }

    IBuild IHasSticks.Up() {
        _direction = Direction.Vertical;
        _sign      = -1f;
        return this;
    }

    IBuild IHasSticks.Left() {
        _direction = Direction.Horizontal;
        _sign      = -1f;
        return this;
    }

    IBuild IHasSticks.Down() {
        _direction = Direction.Vertical;
        _sign      = 1f;
        return this;
    }

    IBuild IHasSticks.Right() {
        _direction = Direction.Horizontal;
        _sign      = 1f;
        return this;
    }

    InputEventJoypadMotion IBuild.Build() {
        Debug.Assert(_stick != Sticks.Invalid);
        Debug.Assert(_direction != Direction.Invalid);
        Debug.Assert(!Mathf.IsZeroApprox(_sign));

        return new InputEventJoypadMotion {
            Axis      = CalculateAxis(),
            AxisValue = _sign,
        };
    }

    private int CalculateAxis() =>
        _stick switch {
            Sticks.Left  => (int)(_direction == Direction.Horizontal ? JoystickList.Axis0 : JoystickList.Axis1),
            Sticks.Right => (int)(_direction == Direction.Horizontal ? JoystickList.Axis2 : JoystickList.Axis3),
            _            => -1
        };
}
