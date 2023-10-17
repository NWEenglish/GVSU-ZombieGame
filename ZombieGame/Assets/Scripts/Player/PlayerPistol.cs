using Assets.Scripts.Constants.Names;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerPistol : MonoBehaviour
    {
        public PistolWeapon Weapon { get; private set; }

        private void Start()
        {
            var bullet = GameObject.Find(ObjectNames.Bullet);
            var muzzle = gameObject;
            var reloadSound = gameObject.GetComponent<AudioSource>();
            var sprite = GameObject.Find(ObjectNames.PlayerPistol).GetComponent<SpriteRenderer>().sprite;

            Weapon = new PistolWeapon(bullet, muzzle, reloadSound, sprite);
        }
    }
}
