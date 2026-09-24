using System.Collections.Generic;
using NUnit.Framework;
using Trykli.Data;
using Trykli.Objectives;

namespace Trykli.Tests
{
    public class ObjectiveTests
    {
        private static LevelRunStats Stats(bool reachedGoal = true)
        {
            return new LevelRunStats { ReachedGoal = reachedGoal, ElapsedTime = 6f, Attempts = 1, TotalCrystalsInLevel = 3 };
        }

        private static StarObjective Make(string type, int count = 0, float value = 0f, Comparison comparison = Comparison.AtLeast,
            string target = "", bool invert = false)
        {
            return ObjectiveFactory.Create(new ObjectiveDefinition
            {
                type = type, count = count, value = value, comparison = comparison, target = target, invert = invert
            });
        }

        [Test]
        public void FactoryKnowsEveryType()
        {
            foreach (string type in ObjectiveFactory.KnownTypes)
            {
                StarObjective objective = ObjectiveFactory.CreateDefault(type);
                Assert.IsNotNull(objective, type);
                Assert.AreEqual(type, objective.ToDefinition().type, type);
            }
        }

        [Test]
        public void UnknownTypeFallsBackToComplete()
        {
            Assert.IsInstanceOf<CompleteLevelObjective>(Make("DoesNotExist"));
        }

        [Test]
        public void CompleteLevel()
        {
            StarObjective objective = Make(ObjectiveDefinition.Complete);
            Assert.IsTrue(objective.Evaluate(Stats()));
            Assert.IsFalse(objective.Evaluate(Stats(false)));
        }

        [Test]
        public void MaxObjects_CountsUnits()
        {
            StarObjective objective = Make(ObjectiveDefinition.MaxObjects, count: 2);
            LevelRunStats stats = Stats();
            stats.SetItemUnits("spring", 1);
            stats.SetItemUnits("portal_ab", 1);
            Assert.IsTrue(objective.Evaluate(stats));
            stats.SetItemUnits("fan", 1);
            Assert.IsFalse(objective.Evaluate(stats));
        }

        [Test]
        public void ItemUsage_SpecificItemAndExclusion()
        {
            LevelRunStats stats = Stats();
            stats.SetItemUnits("magnet", 2);
            stats.SetItemUnits("spring", 1);

            Assert.IsTrue(Make(ObjectiveDefinition.ItemUsage, 2, target: "magnet").Evaluate(stats));
            Assert.IsFalse(Make(ObjectiveDefinition.ItemUsage, 1, comparison: Comparison.AtMost, target: "magnet").Evaluate(stats));
            // "Only magnets": every item except magnets must be at most 0.
            Assert.IsFalse(Make(ObjectiveDefinition.ItemUsage, 0, comparison: Comparison.AtMost, target: "magnet", invert: true).Evaluate(stats));
            stats.SetItemUnits("spring", 0);
            Assert.IsTrue(Make(ObjectiveDefinition.ItemUsage, 0, comparison: Comparison.AtMost, target: "magnet", invert: true).Evaluate(stats));
        }

        [Test]
        public void Crystals_CountAllOrSpecific()
        {
            LevelRunStats stats = Stats();
            stats.AddCrystal("c1");
            stats.AddCrystal("c2");

            Assert.IsTrue(Make(ObjectiveDefinition.CollectCrystals, 2).Evaluate(stats));
            Assert.IsFalse(Make(ObjectiveDefinition.CollectCrystals, 0).Evaluate(stats), "count 0 = every crystal of the level");
            Assert.IsTrue(Make(ObjectiveDefinition.CollectCrystals, target: "c2").Evaluate(stats));
            Assert.IsFalse(Make(ObjectiveDefinition.CollectCrystals, target: "c3").Evaluate(stats));
            stats.AddCrystal("c3");
            Assert.IsTrue(Make(ObjectiveDefinition.CollectCrystals, 0).Evaluate(stats));
        }

        [Test]
        public void Time()
        {
            Assert.IsTrue(Make(ObjectiveDefinition.Time, value: 8f).Evaluate(Stats()));
            Assert.IsFalse(Make(ObjectiveDefinition.Time, value: 5f).Evaluate(Stats()));
        }

        [Test]
        public void BounceCount_BySource()
        {
            LevelRunStats stats = Stats();
            stats.AddBounce(BounceSource.Spring);
            stats.AddBounce(BounceSource.Bumper);
            stats.AddBounce(BounceSource.Bumper);

            Assert.AreEqual(3, stats.GetBounces(BounceSource.Any));
            Assert.IsTrue(Make(ObjectiveDefinition.BounceCount, 2, comparison: Comparison.Exactly, target: "Bumper").Evaluate(stats));
            Assert.IsTrue(Make(ObjectiveDefinition.BounceCount, 1, comparison: Comparison.Exactly, target: "Spring").Evaluate(stats));
            Assert.IsFalse(Make(ObjectiveDefinition.BounceCount, 2, comparison: Comparison.AtMost).Evaluate(stats));
        }

        [Test]
        public void NoHazard_ByCategory()
        {
            LevelRunStats stats = Stats();
            Assert.IsTrue(Make(ObjectiveDefinition.NoHazard, target: "Any").Evaluate(stats));
            stats.AddHazardContact(HazardCategory.Laser);
            Assert.IsFalse(Make(ObjectiveDefinition.NoHazard, target: "Laser").Evaluate(stats));
            Assert.IsFalse(Make(ObjectiveDefinition.NoHazard, target: "Any").Evaluate(stats));
            Assert.IsTrue(Make(ObjectiveDefinition.NoHazard, target: "Spikes").Evaluate(stats));
        }

        [Test]
        public void AvoidContact_BySurface()
        {
            LevelRunStats stats = Stats();
            stats.AddContact(SurfaceKind.Wall);
            Assert.IsFalse(Make(ObjectiveDefinition.AvoidContact, target: "Wall").Evaluate(stats));
            Assert.IsTrue(Make(ObjectiveDefinition.AvoidContact, target: "Ground").Evaluate(stats));
        }

        [Test]
        public void SpecificInteraction_UsesCounters()
        {
            LevelRunStats stats = Stats();
            stats.Increment(LevelRunStats.Counters.ButtonPressed);
            stats.Increment(LevelRunStats.Counters.Button("b2"));
            stats.Increment(LevelRunStats.Counters.GravityFlip, 2);

            Assert.IsTrue(Make(ObjectiveDefinition.Interaction, 1, target: "button").Evaluate(stats));
            Assert.IsTrue(Make(ObjectiveDefinition.Interaction, 1, target: "button:b2").Evaluate(stats));
            Assert.IsFalse(Make(ObjectiveDefinition.Interaction, 1, target: "button:b1").Evaluate(stats));
            Assert.IsTrue(Make(ObjectiveDefinition.Interaction, 2, comparison: Comparison.Exactly, target: "gravity_flip").Evaluate(stats));
            Assert.IsFalse(Make(ObjectiveDefinition.Interaction, 1, comparison: Comparison.Exactly, target: "gravity_flip").Evaluate(stats));
        }

        [Test]
        public void Attempts()
        {
            LevelRunStats stats = Stats();
            stats.Attempts = 3;
            Assert.IsTrue(Make(ObjectiveDefinition.Attempts, 3).Evaluate(stats));
            Assert.IsFalse(Make(ObjectiveDefinition.Attempts, 2).Evaluate(stats));
            Assert.IsFalse(Make(ObjectiveDefinition.Attempts, 1).Evaluate(stats));
        }

        [Test]
        public void NoStop()
        {
            LevelRunStats stats = Stats();
            stats.LongestIdleTime = 0.4f;
            Assert.IsTrue(Make(ObjectiveDefinition.NoStop, value: 1f).Evaluate(stats));
            stats.LongestIdleTime = 2f;
            Assert.IsFalse(Make(ObjectiveDefinition.NoStop, value: 1f).Evaluate(stats));
        }

        [Test]
        public void DefinitionRoundTrip()
        {
            var definition = new ObjectiveDefinition
            {
                type = ObjectiveDefinition.BounceCount, count = 2, comparison = Comparison.Exactly, target = "Bumper"
            };
            ObjectiveDefinition back = ObjectiveFactory.Create(definition).ToDefinition();
            Assert.AreEqual(definition.type, back.type);
            Assert.AreEqual(definition.count, back.count);
            Assert.AreEqual(definition.comparison, back.comparison);
            Assert.AreEqual(definition.target, back.target);
        }

        [Test]
        public void Comparisons()
        {
            Assert.IsTrue(Comparison.AtLeast.Evaluate(3, 2));
            Assert.IsFalse(Comparison.AtLeast.Evaluate(1, 2));
            Assert.IsTrue(Comparison.AtMost.Evaluate(2, 2));
            Assert.IsFalse(Comparison.AtMost.Evaluate(3, 2));
            Assert.IsTrue(Comparison.Exactly.Evaluate(2, 2));
            Assert.IsFalse(Comparison.Exactly.Evaluate(1, 2));
        }
    }

    public class StarEvaluatorTests
    {
        private static List<StarObjective> Objectives(params ObjectiveDefinition[] definitions)
        {
            var list = new List<StarObjective>();
            foreach (ObjectiveDefinition definition in definitions) list.Add(ObjectiveFactory.Create(definition));
            return list;
        }

        [Test]
        public void OneStarPerFulfilledObjective()
        {
            List<StarObjective> objectives = Objectives(
                new ObjectiveDefinition { type = ObjectiveDefinition.Complete },
                new ObjectiveDefinition { type = ObjectiveDefinition.MaxObjects, count = 1 },
                new ObjectiveDefinition { type = ObjectiveDefinition.CollectCrystals, count = 1 });
            var stats = new LevelRunStats { ReachedGoal = true, TotalCrystalsInLevel = 1 };
            stats.SetItemUnits("spring", 1);

            LevelResult twoStars = StarEvaluator.Evaluate(4, objectives, stats);
            Assert.AreEqual(2, twoStars.stars);
            Assert.IsTrue(twoStars.IsObjectiveMet(0));
            Assert.IsTrue(twoStars.IsObjectiveMet(1));
            Assert.IsFalse(twoStars.IsObjectiveMet(2));

            stats.AddCrystal("c1");
            LevelResult threeStars = StarEvaluator.Evaluate(4, objectives, stats);
            Assert.AreEqual(3, threeStars.stars);
            Assert.AreEqual(1, threeStars.crystals);
            Assert.AreEqual(4, threeStars.levelId);
        }

        [Test]
        public void FailureGivesNoStar()
        {
            List<StarObjective> objectives = Objectives(new ObjectiveDefinition { type = ObjectiveDefinition.Complete });
            LevelResult result = StarEvaluator.Evaluate(1, objectives, new LevelRunStats { ReachedGoal = false });
            Assert.IsFalse(result.completed);
            Assert.AreEqual(0, result.stars);
        }

        [Test]
        public void VictoryAlwaysGivesAtLeastOneStar()
        {
            LevelResult result = StarEvaluator.Evaluate(1, new List<StarObjective>(), new LevelRunStats { ReachedGoal = true });
            Assert.AreEqual(1, result.stars);
        }
    }
}
