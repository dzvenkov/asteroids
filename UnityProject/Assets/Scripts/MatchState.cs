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
        event Action OnUpdated;
    }

    public class MatchState : IMatchState
    {
        public IMatchState.Status State { get; private set; }
        public int Hearts { get; private set; }
        public int Score { get; private set; }

        public event Action OnUpdated;

        public MatchState(GameSettings settings)
        {
            Hearts = settings.StartHearts;
        }
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

        public void CheatAddHealth()
        {
            Hearts++;
            OnUpdated?.Invoke();
        }
    }
}