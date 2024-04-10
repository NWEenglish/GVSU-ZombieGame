using System;
using System.Collections.Generic;
using Assets.Scripts.Player;
using Assets.Scripts.Stores.WeaponStores;
using Assets.Scripts.Weapons;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor.Player
{
    public class PlayerStatusTests
    {
        [Test]
        public void PlayerStatusTest()
        {
            // Arrange
            int expectedHealth = 100;
            int expectedEnergy = 1000;
            int expectedPoints = 0;

            // Act
            PlayerStatus playerStatus = new();

            // Assert
            Assert.AreEqual(expectedHealth, playerStatus.Health);
            Assert.AreEqual(expectedEnergy, playerStatus.Energy);
            Assert.AreEqual(expectedPoints, playerStatus.Points);
        }

        [TestCase(5, ExpectedResult = true)] // 5 hits and player has died
        [TestCase(1, ExpectedResult = false)] // 1 hit and player still alive
        [TestCase(0, ExpectedResult = false)] // No hits and player still alive
        public bool IsPlayerDeadTest(int numberOfHits)
        {
            // Arrange
            PlayerStatus playerStatus = new();

            for (int hits = 0; hits < numberOfHits; hits++)
            {
                playerStatus.TakeZombieHit();
            }

            // Act
            bool isPlayerDead = playerStatus.IsPlayerDead;

            // Assert
            return isPlayerDead;
        }

        [TestCase(true, true, ExpectedResult = false)] // Moving and hilding sprint, energy decreased
        [TestCase(false, true, ExpectedResult = true)] // Not moving and holding sprint, energy was unchanged
        [TestCase(false, false, ExpectedResult = true)] // Not moving and not holding sprint, energy was unchanged
        [TestCase(true, false, ExpectedResult = true)] // Moving and not holding sprint, energy was unchanged
        public bool UpdateEnergyTest(bool isMoving, bool isSprinting)
        {
            // Arrange
            PlayerStatus playerStatus = new();
            int startingEnergy = playerStatus.Energy;

            // Act
            playerStatus.Update(isMoving, isSprinting);

            // Assert
            return playerStatus.Energy >= startingEnergy;
        }

        [Test]
        public void UpdateHealthTest()
        {
            // Arrange
            bool isMoving = false;
            bool isSprinting = false;
            double secondsToWait = 2;

            PlayerStatus playerStatus = new();
            playerStatus.TakeZombieHit();
            int startingHealth = playerStatus.Health;
            DateTime startingTime = DateTime.Now;

            while (startingTime.AddSeconds(secondsToWait) > DateTime.Now) { }

            // Act
            playerStatus.Update(isMoving, isSprinting);

            // Assert
            Assert.That(playerStatus.Health > startingHealth);
        }

        [Test]
        public void TakeHitTest()
        {
            // Arrange
            int startingHealth = 100;
            int damage = 34;
            int expectedHealth = startingHealth - damage;
            PlayerStatus playerStatus = new();

            // Act
            playerStatus.TakeZombieHit();

            // Assert
            Assert.AreEqual(expectedHealth, playerStatus.Health);
        }

        [TestCaseSource(nameof(AwardPointsTestCases))]
        public int AwardPointsTests(int points)
        {
            // Arrange
            PlayerStatus playerStatus = new();

            // Act
            playerStatus.AwardPoints(points);

            // Assert
            return playerStatus.Points;
        }

        public void AwardPointsTest()
        {
            // Arrange
            PlayerStatus playerStatus = new();
            List<int> points = new()
            {
                25,
                50,
                20,
                5
            };
            int expectedPoints = 100;

            // Act
            foreach (int point in points)
            {
                playerStatus.AwardPoints(point);
            }

            // Assert
            Assert.AreEqual(expectedPoints, playerStatus.Points);
        }

        [TestCase(200, 0, 250)] // Enough points, can afford ammo
        [TestCase(100, 100, 200)] // Not enough points, can't afford ammo
        [TestCase(0, 0, 200)] // No points, can't afford to buy ammo
        public void HandleAmmoPurchaseTests(int startingPoints, int expectedPoints, int expectedAmmo)
        {
            // Arrange
            BaseWeapon weapon = new PistolWeapon(null, null, null, null);

            var gameObject = new GameObject();
            gameObject.AddComponent<PistolStore>();
            gameObject.AddComponent<AudioSource>();

            PlayerStatus playerStatus = new();
            playerStatus.AwardPoints(startingPoints);

            // Act
            playerStatus.HandleAmmoPurchase(weapon, gameObject.GetComponent<BaseWeaponStore>());

            // Assert
            Assert.AreEqual(expectedPoints, playerStatus.Points);
            Assert.AreEqual(expectedAmmo, weapon.RemainingTotalAmmo);
        }

        private static IEnumerable<TestCaseData> AwardPointsTestCases()
        {
            yield return new TestCaseData(0).Returns(0);
            yield return new TestCaseData(-100).Returns(-100);
            yield return new TestCaseData(100).Returns(100);
            yield return new TestCaseData(int.MaxValue).Returns(int.MaxValue);
            yield return new TestCaseData(int.MinValue).Returns(int.MinValue);
        }

        private BaseWeapon GetWeapon()
        {
            return new PistolWeapon(null, null, null, null);
        }
    }
}
