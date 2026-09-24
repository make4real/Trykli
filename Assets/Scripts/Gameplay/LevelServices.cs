using System;
using System.Collections.Generic;
using Trykli.Data;
using Trykli.Objectives;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>Channel based signals between buttons and mechanisms (doors, lasers, platforms, magnets...).</summary>
    public sealed class SwitchBus
    {
        private readonly Dictionary<int, List<Action>> _listeners = new Dictionary<int, List<Action>>();

        public void Subscribe(int channel, Action listener)
        {
            if (channel < 0 || listener == null) return;
            if (!_listeners.TryGetValue(channel, out List<Action> list))
            {
                list = new List<Action>();
                _listeners[channel] = list;
            }

            list.Add(listener);
        }

        public void Emit(int channel)
        {
            if (channel < 0 || !_listeners.TryGetValue(channel, out List<Action> list)) return;
            // Copy: a listener may subscribe / emit while being notified.
            foreach (Action listener in list.ToArray()) listener();
        }

        public void Clear() => _listeners.Clear();
    }

    /// <summary>Gravity orientation of the level (Trykli and gravity-affected objects).</summary>
    public sealed class GravityService
    {
        public bool Inverted { get; private set; }

        public event Action<bool> Changed;

        public Vector2 Down => Inverted ? Vector2.up : Vector2.down;

        /// <summary>Applies a gravity switch. Returns true when the orientation actually changed.</summary>
        public bool Apply(GravityMode mode)
        {
            bool next;
            switch (mode)
            {
                case GravityMode.Invert: next = true; break;
                case GravityMode.Normal: next = false; break;
                default: next = !Inverted; break;
            }

            if (next == Inverted) return false;
            Inverted = next;
            Changed?.Invoke(Inverted);
            return true;
        }

        public void Reset()
        {
            Inverted = false;
        }
    }

    /// <summary>Collects the statistics of the current run for the star objectives.</summary>
    public sealed class RunTracker
    {
        private float _idleTimer;

        public LevelRunStats Stats { get; private set; } = new LevelRunStats();

        public void Begin(int attempt, int totalCrystals, IReadOnlyDictionary<string, int> itemUnits)
        {
            Stats = new LevelRunStats { Attempts = attempt, TotalCrystalsInLevel = totalCrystals };
            if (itemUnits != null)
            {
                foreach (KeyValuePair<string, int> pair in itemUnits) Stats.SetItemUnits(pair.Key, pair.Value);
            }

            _idleTimer = 0f;
        }

        public void RecordBounce(BounceSource source) => Stats.AddBounce(source);
        public void RecordCounter(string key) => Stats.Increment(key);
        public void RecordContact(SurfaceKind surface) => Stats.AddContact(surface);
        public void RecordHazard(HazardCategory category) => Stats.AddHazardContact(category);
        public void RecordCrystal(string id) => Stats.AddCrystal(id);

        public void Tick(float deltaTime, float speed, float idleThreshold)
        {
            Stats.ElapsedTime += deltaTime;
            if (speed < idleThreshold)
            {
                _idleTimer += deltaTime;
                if (_idleTimer > Stats.LongestIdleTime) Stats.LongestIdleTime = _idleTimer;
            }
            else
            {
                _idleTimer = 0f;
            }
        }
    }
}
