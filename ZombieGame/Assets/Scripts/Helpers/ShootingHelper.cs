using Assets.Scripts.Constants.Types;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class ShootingHelper
    {
        private const float Speed = 1000f;

        // Borrowed from TankGame
        public static GameObject Shoot(GameObject bullet, Vector3 spawnLocation, float targetAngle, int damage, TeamType teamSource)
        {
            spawnLocation.z = 0;

            // Create new bullet
            GameObject firedBullet = Object.Instantiate(bullet, spawnLocation, Quaternion.AngleAxis(targetAngle - 90, Vector3.forward));
            firedBullet.GetComponent<Rigidbody2D>().AddForce(GetForceVector(targetAngle, Speed));
            firedBullet.GetComponent<AudioSource>().mute = false;
            firedBullet.GetComponent<BulletLogic>().InitValues(damage, teamSource);

            return firedBullet;
        }

        // Credit to GlassesGuy for the equations to compute this.
        // https://answers.unity.com/questions/1646067/can-you-add-a-force-to-a-rigidbody-at-an-angle.html
        private static Vector2 GetForceVector(float angle, float bulletSpeed)
        {
            float x_component = Mathf.Cos(angle * Mathf.PI / 180) * bulletSpeed;
            float y_component = Mathf.Sin(angle * Mathf.PI / 180) * bulletSpeed;

            Vector2 force = new Vector2(x_component, y_component);

            return force;
        }
    }
}
