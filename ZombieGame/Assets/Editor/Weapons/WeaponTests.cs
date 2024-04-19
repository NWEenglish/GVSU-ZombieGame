using System.Collections.Generic;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Weapons;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor.Weapons
{
    public class WeaponTests
    {
        [TearDown]
        public void EndTest()
        {
            GameObject.Find(ObjectNames.Pistol).GetComponent<AudioSource>().Stop();
        }

        [TestCaseSource(nameof(ReloadTestCases))]
        public void ReloadTests(int timesToShoot, int expectedAmmoInClip, int expectedRemainingAmmo)
        {
            // Arrange
            GameObject bullet = GameObject.Find(ObjectNames.Bullet);
            GameObject muzzle = GameObject.Find(ObjectNames.Pistol);
            AudioSource reloadSound = GameObject.Find(ObjectNames.Pistol).GetComponent<AudioSource>();
            Sprite sprite = GameObject.Find(ObjectNames.HumanPistol).GetComponent<Sprite>();

            PistolWeapon weapon = new(bullet, muzzle, reloadSound, sprite);

            // Act
            for (int i = 0; i < timesToShoot; i++)
            {
                // Wait until the weapon can be shot again
                while (!weapon.CanShoot()) { }
                weapon.Shoot(angle: 0, teamSource: TeamType.PlayerTeam);
            }

            weapon.Reload();

            // Assert
            Assert.AreEqual(expectedRemainingAmmo, weapon.RemainingTotalAmmo);
            Assert.AreEqual(expectedAmmoInClip, weapon.RemainingClipAmmo);
        }

        private static IEnumerable<TestCaseData> ReloadTestCases()
        {
            yield return new TestCaseData(0, 10, 200);
            yield return new TestCaseData(5, 10, 195);
            yield return new TestCaseData(10, 10, 190);
            yield return new TestCaseData(11, 10, 190);
        }
    }
}
