using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace CyberBlood.Scenes.Entities.Player.States {
    public class StateMachine : Spatial {
        [Signal]
        public delegate void StateChanged(State prevState, State newState);

        private State _initialState = State.Idle;

        public State CurrentState { get; private set; }
        private Player _player;
        private IReadOnlyDictionary<State, BaseState> _states;

        public override void _Ready() {
            _player = GetParent<Player>();
            _       = ToSignal(Owner, "ready");
            SetAsToplevel(true);
        }

        public void PopulateStates() {
            var states = new Dictionary<State, BaseState>(GetChildCount());
            var names  = Enum.GetValues(typeof(State));

            var machine = _player.AnimTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
            foreach (State state in names) {
                var name = state.ToString();
                states[state]                  = GetNode<BaseState>(name);
                states[state].Player           = _player;
                states[state].AnimStateMachine = machine;
                Debug.Assert(states[state] != null);
            }

            _states      = states;
            
            CurrentState = _initialState;
            _states[CurrentState].OnEntry();
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
