using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public abstract class BasePlayer : MonoBehaviour
    {
        public BaseWeapon Weapon { get; private set; }

        protected abstract string BulletObjectName { get; }
        protected abstract string PlayerWeaponObjectName { get; }
        protected abstract BaseWeapon GetWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite);

        private void Start()
        {
            GameObject bullet = GameObject.Find(BulletObjectName);
            GameObject muzzle = gameObject;
            AudioSource reloadSound = gameObject.GetComponent<AudioSource>();
            Sprite sprite = GameObject.Find(PlayerWeaponObjectName).GetComponent<SpriteRenderer>().sprite;

            Weapon = GetWeapon(bullet, muzzle, reloadSound, sprite);
        }
    }
}
