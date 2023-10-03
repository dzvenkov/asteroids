using System;
using UnityEditor;
using UnityEngine;

namespace Asteroids
{
    public interface IMatchState
    {
        public enum Status
        {
            InProgress,
            Win,
            Lose
        }
        Status State { get; }
        int Hearts { get; }
        int Score { get; }

        float ShieldNormalizedDurationLeft { get; }
        event Action OnUpdated;
    }

    public class MatchState : IMatchState
    {
        private readonly GameSettings _settings;
        public IMatchState.Status State { get; private set; }
        public int Hearts { get; private set; }
        public int Score { get; private set; }

        private float _shieldStartTime = -1000;

        public event Action OnUpdated;

        public MatchState(GameSettings settings)
        {
            _settings = settings;
            Hearts = settings.StartHearts;
        }

        public float ShieldNormalizedDurationLeft
            => Mathf.Clamp01(1 - (Time.time - _shieldStartTime) / _settings.ShieldDurationSec);

        public void RegisterPlayerDeath()
        {
            Debug.Assert(State == IMatchState.Status.InProgress);
            Hearts -= 1;
            if (Hearts == 0)
            {
                State = IMatchState.Status.Lose;
            }
            OnUpdated?.Invoke();
        }

        public void RegisterAsteroidKill(int level)
        {
            if (State == IMatchState.Status.InProgress)//no late kills
            {
                Score += Mathf.Clamp(6 - level, 0, 6);
            }
            OnUpdated?.Invoke();
        }

        public void RegisterVictory()
        {
            Debug.Assert(State == IMatchState.Status.InProgress);
            State = IMatchState.Status.Win;
            OnUpdated?.Invoke();
        }

        public void RegisterPickupCollection(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Shield:
                    _shieldStartTime = Time.time;
                    break;
                case PickupType.Heart:
                    Hearts++;
                    break;
            }
            OnUpdated?.Invoke();
        }
    }
}