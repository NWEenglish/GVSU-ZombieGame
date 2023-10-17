using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.HUD;
using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace Assets.Editor.HUD
{
    public class PointsHUDTests
    {
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(10)]
        [TestCase(0)]
        public void UpdateHUDTests(int points)
        {
            // Arrange
            TextMeshProUGUI textMesh = new();

            PointsHUD pointsHud = new PointsHUD(textMesh);

            // Act
            pointsHud.UpdateHUD(points);

            // Assert
            Assert.That(textMesh.text.Contains(points.ToString()));
        }
    }
}
