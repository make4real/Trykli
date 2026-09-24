using System;
using Trykli.Objectives;

namespace Trykli.Core
{
    /// <summary>
    /// Global gameplay events. Useful hooks for future analytics (GDD 66): level started / completed,
    /// attempts, failures, abandon. No analytics SDK is included.
    /// </summary>
    public static class GameEvents
    {
        public static event Action<int> LevelStarted;
        public static event Action<int, int> AttemptStarted;
        public static event Action<int, string> LevelFailed;
        public static event Action<LevelResult> LevelCompleted;
        public static event Action<int> LevelAbandoned;

        public static void RaiseLevelStarted(int levelId) => LevelStarted?.Invoke(levelId);
        public static void RaiseAttemptStarted(int levelId, int attempt) => AttemptStarted?.Invoke(levelId, attempt);
        public static void RaiseLevelFailed(int levelId, string reason) => LevelFailed?.Invoke(levelId, reason);
        public static void RaiseLevelCompleted(LevelResult result) => LevelCompleted?.Invoke(result);
        public static void RaiseLevelAbandoned(int levelId) => LevelAbandoned?.Invoke(levelId);
    }
}
