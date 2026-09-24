using System;

namespace Trykli.Core
{
    /// <summary>Global game states (GDD 77). Interactions available depend on the current state.</summary>
    public enum GameState
    {
        Boot = 0,
        Menu = 1,
        Placement = 2,
        Simulation = 3,
        Victory = 4,
        Failure = 5,
        Pause = 6
    }

    /// <summary>
    /// Small explicit state machine. Invalid transitions are rejected, which prevents interaction bugs
    /// (e.g. moving an object during the simulation).
    /// </summary>
    public sealed class GameStateMachine
    {
        public GameState Current { get; private set; } = GameState.Boot;
        public GameState Previous { get; private set; } = GameState.Boot;

        /// <summary>State restored when leaving <see cref="GameState.Pause"/>.</summary>
        public GameState ResumeState { get; private set; } = GameState.Placement;

        public event Action<GameState, GameState> Changed;

        public bool IsPlaying => Current == GameState.Placement || Current == GameState.Simulation;

        public static bool IsTransitionAllowed(GameState from, GameState to)
        {
            if (to == GameState.Menu || to == GameState.Boot) return true;
            switch (from)
            {
                case GameState.Boot:
                    return to == GameState.Placement;
                case GameState.Menu:
                    return to == GameState.Placement;
                case GameState.Placement:
                    return to == GameState.Simulation || to == GameState.Pause || to == GameState.Placement;
                case GameState.Simulation:
                    return to == GameState.Victory || to == GameState.Failure || to == GameState.Pause ||
                           to == GameState.Placement || to == GameState.Simulation;
                case GameState.Victory:
                    return to == GameState.Placement;
                case GameState.Failure:
                    return to == GameState.Simulation || to == GameState.Placement || to == GameState.Pause;
                case GameState.Pause:
                    return to == GameState.Placement || to == GameState.Simulation || to == GameState.Failure;
                default:
                    return false;
            }
        }

        public bool TrySet(GameState next)
        {
            if (!IsTransitionAllowed(Current, next)) return false;
            if (next == GameState.Pause && Current != GameState.Pause) ResumeState = Current;
            Previous = Current;
            Current = next;
            Changed?.Invoke(Previous, Current);
            return true;
        }

        /// <summary>Leaves the pause and returns to the state that was active before it.</summary>
        public bool Resume()
        {
            return Current == GameState.Pause && TrySet(ResumeState);
        }
    }
}
