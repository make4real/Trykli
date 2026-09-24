using System;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Deterministic simulation clock. Physics2D runs in Script mode and is stepped here with a fixed step
    /// (0.02 s): mechanisms are stepped first, then the physics. The same placement therefore always gives
    /// the same result, independently of the frame rate. x2 speed simply runs two steps per step time.
    /// </summary>
    public sealed class SimulationRunner : MonoBehaviour
    {
        private LevelContext _context;
        private float _accumulator;
        private float _step = 0.02f;
        private int _maxStepsPerFrame = 8;
        private PhysicsScene2D _physicsScene;
        private bool _customPhysicsScene;

        /// <summary>Interpolation factor between the last two physics states (visual smoothing only).</summary>
        public static float InterpolationAlpha { get; private set; } = 1f;

        public bool Running { get; private set; }
        public bool Paused { get; set; }
        public float SpeedMultiplier { get; set; } = 1f;
        public float StepDuration => _step;

        public event Action StepCompleted;

        public void Configure(float step, int maxStepsPerFrame)
        {
            _step = Mathf.Max(0.005f, step);
            _maxStepsPerFrame = Mathf.Max(1, maxStepsPerFrame);
        }

        /// <summary>Simulates a specific physics scene instead of the default one (solution verifier, tests).</summary>
        public void SetPhysicsScene(PhysicsScene2D scene)
        {
            _physicsScene = scene;
            _customPhysicsScene = true;
        }

        public void Begin(LevelContext context)
        {
            _context = context;
            _accumulator = 0f;
            Running = true;
            Paused = false;
            InterpolationAlpha = 0f;
        }

        public void Stop()
        {
            Running = false;
            InterpolationAlpha = 1f;
        }

        private void Update()
        {
            if (!Running || Paused || _context == null) return;
            _accumulator += Time.deltaTime * SpeedMultiplier;
            float maxAccumulated = _step * _maxStepsPerFrame * Mathf.Max(1f, SpeedMultiplier);
            if (_accumulator > maxAccumulated) _accumulator = maxAccumulated;

            int steps = 0;
            int maxSteps = Mathf.CeilToInt(_maxStepsPerFrame * Mathf.Max(1f, SpeedMultiplier));
            while (_accumulator >= _step && steps < maxSteps && Running)
            {
                Step();
                _accumulator -= _step;
                steps++;
            }

            InterpolationAlpha = Running ? Mathf.Clamp01(_accumulator / _step) : 1f;
        }

        /// <summary>Advances the simulation by exactly one fixed step (also used by the solution verifier).</summary>
        public void Step()
        {
            if (_context == null || !_context.IsSimulating) return;
            _context.StepElements(_step);
            if (!_context.IsSimulating) return;

            TrykliController trykli = _context.Trykli;
            if (trykli != null) trykli.BeforePhysicsStep();
            if (_customPhysicsScene) _physicsScene.Simulate(_step);
            else Physics2D.Simulate(_step);
            if (trykli != null) trykli.AfterPhysicsStep();
            StepCompleted?.Invoke();
        }
    }
}
