using System;
using System.Collections.Generic;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Stores;
using Assets.Scripts.Weapons;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor.Weapons
{
    public class WeaponTests // TODO -  Rewrite?
    {
        [TestCaseSource(nameof(ReloadTestCases))]
        public void ReloadTests(int ammoInClip, int remainingAmmo, int expectedAmmoInClip, int expectedRemainingAmmo)
        {
            // Arrange
            int clipSize = 10;
            WeaponTest weapon = new WeaponTest(clipSize, ammoInClip, remainingAmmo);

            // Act
            weapon.Reload();

            // Assert
            Assert.AreEqual(expectedRemainingAmmo, weapon.RemainingAmmo);
            Assert.AreEqual(expectedAmmoInClip, weapon.AmmoClip);
        }

        private static IEnumerable<TestCaseData> ReloadTestCases()
        {
            yield return new TestCaseData(0, 10, 10, 0);
            yield return new TestCaseData(10, 10, 10, 10);
            yield return new TestCaseData(10, 0, 10, 0);
            yield return new TestCaseData(5, 0, 5, 0);
        }
    }

    internal class WeaponTest : Weapon
    {
        public WeaponTest(int clipSize, int ammoInClip, int remainingAmmo)
        {
            AmmoClipSize = clipSize;
            AmmoClip = ammoInClip;
            RemainingAmmo = remainingAmmo;
            ReloadSound = GameObject.Find(ObjectNames.Pistol).GetComponent<AudioSource>();
        }

        public override int AmmoClip { get; protected set; }
        public override int RemainingAmmo { get; protected set; }
        public override int AmmoClipSize { get; protected set; }
        public override AudioSource ReloadSound { get; protected set; }
        public override Sprite Sprite { get; protected set; }
        public override WeaponType Type => WeaponType.Pistol;

        public override bool CanShoot()
        {
            throw new NotImplementedException();
        }

        public override void PurchaseAmmo(ref int Points, BaseStore store)
        {
            throw new NotImplementedException();
        }

        public override void Shoot(float angle)
        {
            throw new NotImplementedException();
        }
    }
}
