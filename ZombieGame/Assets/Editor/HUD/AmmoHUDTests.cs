using Assets.Scripts.HUD;
using NUnit.Framework;
using TMPro;

namespace Assets.Editor.HUD
{
    public class AmmoHUDTests
    {
        [TestCase(int.MinValue, int.MinValue)]
        [TestCase(int.MaxValue, int.MaxValue)]
        [TestCase(0, 10)]
        [TestCase(10, 0)]
        [TestCase(0, 0)]
        public void UpdateHUDTests(int ammoInClip, int remainingAmmo)
        {
            // Arrange
            TextMeshProUGUI textMeshPro = new();
            AmmoHUD ammoHud = new AmmoHUD(textMeshPro);

            // Act
            ammoHud.UpdateHUD(ammoInClip, remainingAmmo);

            // Assert
            Assert.That(textMeshPro.text.Contains(ammoInClip.ToString()));
            Assert.That(textMeshPro.text.Contains(remainingAmmo.ToString()));
        }
    }
}
