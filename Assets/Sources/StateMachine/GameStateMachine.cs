﻿using System;
using System.Collections.Generic;

namespace CarSumo.StateMachine
{
    public class GameStateMachine
    {
        private readonly Dictionary<Type, IState> _states;
        private IState _activeState;

        public GameStateMachine() : this(new Dictionary<Type, IState>()) { }

        public GameStateMachine(IEnumerable<IState> states) : this(GenerateStatesFromEnumerable(states)) { }

        public GameStateMachine(Dictionary<Type, IState> states)
        {
            _states = states;
            _activeState = new IState.Empty();
        }

        public void Enter<TState>() where TState : IState
        {
            _activeState.Exit();
            _activeState = _states[typeof(TState)];
            _activeState.Enter();
        }

        private static Dictionary<Type, IState> GenerateStatesFromEnumerable(IEnumerable<IState> states)
        {
            var generatedStates = new Dictionary<Type, IState>();
            
            foreach (IState state in states)
                Register(generatedStates, state);

            return generatedStates;
        }

        private static void Register(IDictionary<Type, IState> states, IState state)
        {
            Type stateKey = state.GetType();
            states.Add(stateKey, state);
        }
    }
}