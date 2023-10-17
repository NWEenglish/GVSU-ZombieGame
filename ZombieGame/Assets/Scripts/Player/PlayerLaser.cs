using Assets.Scripts.Constants.Names;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerLaser : MonoBehaviour
    {
        public LaserWeapon Weapon { get; private set; }

        private void Start()
        {
            var bullet = GameObject.Find(ObjectNames.Beam);
            var muzzle = gameObject;
            var reloadSound = gameObject.GetComponent<AudioSource>();
            var sprite = GameObject.Find(ObjectNames.PlayerLaser).GetComponent<SpriteRenderer>().sprite;

            Weapon = new LaserWeapon(bullet, muzzle, reloadSound, sprite);
        }
    }
}
