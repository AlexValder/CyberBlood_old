using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace CyberBlood.Scenes.Entities.Player.States {
    public class StateMachine : Node {
        [Signal]
        public delegate void StateChanged(State prevState, State newState);

        [Export(PropertyHint.Enum, "Idle,Walking,Running,Falling,WallStuck,FinalFalling")]
        private State _initialState = State.Idle;
        [Export] private NodePath _playerNode;

        public State CurrentState { get; private set; }
        private Player _player;
        private IReadOnlyDictionary<State, BaseState> _states;

        public override void _Ready() {
            _player = GetNode<Player>(_playerNode);
            PopulateStates();
            CurrentState = _initialState;
            _states[CurrentState].OnEntry();
        }

        private void PopulateStates() {
            var states = new Dictionary<State, BaseState>(GetChildCount());
            var names  = Enum.GetValues(typeof(State));
            foreach (State state in names) {
                var name = state.ToString();
                states[state]        = GetNode<BaseState>(name);
                states[state].Player = _player;
                Debug.Assert(states[state] != null);
            }

            _states = states;
        }
        
        public void Reset() {
            TransitionTo(State.Idle);
        }

        public void TransitionTo(State state) {
            if (!_states[state].Enabled) {
                return;
            }

            _states[CurrentState].OnExit();
            var prev = CurrentState;
            CurrentState = state;
            _states[CurrentState].OnEntry();

            EmitSignal(nameof(StateChanged), prev, state);
        }

        public void Input(InputEvent @event) {
            _states[CurrentState].HandleInput(@event);
        }

        public void Process(float delta) {
            _states[CurrentState].HandleProcess(delta);
        }

        public void PhysicsProcess(float delta) {
            _states[CurrentState].HandlePhysicsProcess(delta);
        }
    }
}
