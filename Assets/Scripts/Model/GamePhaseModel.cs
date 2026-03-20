using System;
using System.Collections.Generic;
using InfinitePorker.Enums;
using UniRx;

namespace InfinitePorker.Model
{
    public class GamePhaseModel
    {
        private readonly ReactiveProperty<GamePhase> _currentPhase = new(GamePhase.CardSelection);
        public IReadOnlyReactiveProperty<GamePhase> CurrentPhase => _currentPhase;
        private Stack<GamePhase> _phaseHistory = new();

        public void SetPhase(GamePhase phase)
        {
            _phaseHistory.Clear();
            _currentPhase.Value = phase;
        }

        public IDisposable PushPhase(GamePhase phase)
        {
            _phaseHistory.Push(_currentPhase.Value);
            _currentPhase.Value = phase;
            return Disposable.Create(PopPhase);
        }

        public void PopPhase()
        {
            if (_phaseHistory.Count > 0)
            {
                _currentPhase.Value = _phaseHistory.Pop();
            }
        }
    }
}