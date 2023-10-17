using Assets.Scripts.Constants.Names;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerRifle : MonoBehaviour
    {
        public RifleWeapon Weapon { get; private set; }

        private void Start()
        {
            var bullet = GameObject.Find(ObjectNames.Bullet);
            var muzzle = gameObject;
            var reloadSound = gameObject.GetComponent<AudioSource>();
            var sprite = GameObject.Find(ObjectNames.PlayerRifle).GetComponent<SpriteRenderer>().sprite;

            Weapon = new RifleWeapon(bullet, muzzle, reloadSound, sprite);
        }
    }
}
