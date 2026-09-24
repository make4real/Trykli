using System.Collections.Generic;
using NUnit.Framework;
using Trykli.Data;
using Trykli.Placement;
using UnityEngine;

namespace Trykli.Tests
{
    public class InventoryTests
    {
        [Test]
        public void TakeAndReturnPieces()
        {
            var inventory = new InventoryModel();
            inventory.Add("spring", 2);

            Assert.AreEqual(2, inventory.RemainingPieces("spring"));
            Assert.IsTrue(inventory.TryTake("spring"));
            Assert.IsTrue(inventory.TryTake("spring"));
            Assert.IsFalse(inventory.TryTake("spring"), "No more springs.");
            Assert.AreEqual(2, inventory.UnitsUsed("spring"));

            inventory.Return("spring");
            Assert.AreEqual(1, inventory.RemainingPieces("spring"));
            inventory.ReturnAll();
            Assert.AreEqual(2, inventory.RemainingPieces("spring"));
            Assert.AreEqual(0, inventory.TotalUnitsUsed);
        }

        [Test]
        public void PortalPairIsOneUnitOfTwoPieces()
        {
            var inventory = new InventoryModel();
            inventory.Add("portal_ab", 1, 2);

            Assert.AreEqual(2, inventory.TotalPieces("portal_ab"));
            inventory.TryTake("portal_ab");
            Assert.AreEqual(1, inventory.UnitsUsed("portal_ab"));
            inventory.TryTake("portal_ab");
            Assert.AreEqual(1, inventory.UnitsUsed("portal_ab"), "Both ends of the pair count as a single object.");
            Assert.IsFalse(inventory.HasRemaining("portal_ab"));
        }

        [Test]
        public void UnknownItemsAreIgnored()
        {
            var inventory = new InventoryModel();
            Assert.IsFalse(inventory.TryTake("magnet"));
            inventory.Return("magnet");
            Assert.AreEqual(0, inventory.RemainingPieces("magnet"));
        }

        [Test]
        public void ClearEmptiesTheInventory()
        {
            var inventory = new InventoryModel();
            inventory.Add("fan", 1);
            inventory.Clear();
            Assert.AreEqual(0, inventory.TotalPieces("fan"));
            CollectionAssert.IsEmpty(new List<string>(inventory.ItemIds));
        }
    }

    public class PlacementRulesTests
    {
        private static readonly PlacementRules.Settings Settings = PlacementRules.Settings.Default;

        private static PlacementZoneData Point(float x, float y, params string[] items)
        {
            return new PlacementZoneData { shape = ZoneShape.Point, position = new Vector2(x, y), allowedItems = new List<string>(items) };
        }

        [Test]
        public void PointZoneSnapsToItsCenter()
        {
            PlacementZoneData zone = Point(1f, 2f, "spring");
            Assert.IsTrue(PlacementRules.TryResolve(zone, "spring", new Vector2(1.5f, 2.3f), null, Settings, out Vector2 position, out _));
            Assert.AreEqual(new Vector2(1f, 2f), position);
            Assert.IsFalse(PlacementRules.TryResolve(zone, "spring", new Vector2(4f, 2f), null, Settings, out _, out _), "Too far.");
        }

        [Test]
        public void ZoneRejectsForbiddenItems()
        {
            PlacementZoneData zone = Point(0f, 0f, "spring");
            Assert.IsFalse(PlacementRules.TryResolve(zone, "fan", Vector2.zero, null, Settings, out _, out _));
            Assert.IsTrue(Point(0f, 0f).Accepts("fan"), "An empty list accepts every item.");
        }

        [Test]
        public void CapacityIsRespected()
        {
            PlacementZoneData zone = Point(0f, 0f);
            var occupied = new List<Vector2> { Vector2.zero };
            Assert.IsFalse(PlacementRules.TryResolve(zone, "spring", Vector2.zero, occupied, Settings, out _, out _));
        }

        [Test]
        public void RectZoneClampsAndKeepsSpacing()
        {
            var zone = new PlacementZoneData
            {
                shape = ZoneShape.Rect, position = Vector2.zero, size = new Vector2(2f, 1f), capacity = 2
            };

            Assert.IsTrue(PlacementRules.TryResolve(zone, "ramp", new Vector2(1.2f, 0f), null, Settings, out Vector2 position, out _));
            Assert.LessOrEqual(position.x, 1f + 1e-4f, "Clamped inside the rectangle.");

            var occupied = new List<Vector2> { new Vector2(0.9f, 0f) };
            Assert.IsFalse(PlacementRules.TryResolve(zone, "ramp", new Vector2(1f, 0f), occupied, Settings, out _, out _),
                "Too close to the other item.");
            Assert.IsTrue(PlacementRules.TryResolve(zone, "ramp", new Vector2(-0.8f, 0f), occupied, Settings, out _, out _));
        }

        [Test]
        public void RailZoneProjectsOnTheSegment()
        {
            var zone = new PlacementZoneData { shape = ZoneShape.Rail, position = Vector2.zero, size = new Vector2(4f, 0f) };
            Assert.IsTrue(PlacementRules.TryResolve(zone, "fan", new Vector2(1f, 0.5f), null, Settings, out Vector2 position, out _));
            Assert.AreEqual(0f, position.y, 1e-4f);
            Assert.AreEqual(1f, position.x, 1e-4f);
        }

        [Test]
        public void RotationIsSnappedAndClamped()
        {
            var zone = new PlacementZoneData { minRotation = -45f, maxRotation = 45f, allowRotation = true };
            Assert.AreEqual(15f, PlacementRules.ClampRotation(zone, 17f, 15f, true), 1e-4f);
            Assert.AreEqual(45f, PlacementRules.ClampRotation(zone, 80f, 15f, true), 1e-4f);
            Assert.AreEqual(-45f, PlacementRules.ClampRotation(zone, -170f, 15f, true), 1e-4f);

            zone.allowRotation = false;
            zone.defaultRotation = 30f;
            Assert.AreEqual(30f, PlacementRules.ClampRotation(zone, 0f, 15f, true), 1e-4f);
        }

        [Test]
        public void RotateStopsAtZoneLimits()
        {
            var zone = new PlacementZoneData { minRotation = -30f, maxRotation = 30f, allowRotation = true };
            Assert.AreEqual(15f, PlacementRules.Rotate(zone, 0f, 15f, 1, true), 1e-4f);
            Assert.AreEqual(30f, PlacementRules.Rotate(zone, 30f, 15f, 1, true), 1e-4f);
        }
    }
}
