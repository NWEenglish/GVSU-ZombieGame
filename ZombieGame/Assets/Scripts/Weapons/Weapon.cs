using Assets.Scripts.Constants.Types;
using Assets.Scripts.Stores;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public abstract class Weapon
    {
        public abstract int AmmoClip { get; protected set; }
        public abstract int RemainingAmmo { get; protected set; }
        public abstract int AmmoClipSize { get; protected set; }
        public abstract AudioSource ReloadSound { get; protected set; }
        public abstract Sprite Sprite { get; protected set; }
        public abstract WeaponType Type { get; protected set;  }

        public abstract bool IsReloading();
        public abstract bool CanShoot();
        public abstract void Shoot(float angle);
        public abstract void PurchaseAmmo(ref int Points, StoreHelper store);
        
        public void Reload()
        {
            if (AmmoClip != AmmoClipSize && RemainingAmmo > 0 && !ReloadSound.isPlaying)
            {
                while (RemainingAmmo > 0 && AmmoClip < AmmoClipSize)
                {
                    RemainingAmmo--;
                    AmmoClip++;
                }
                ReloadSound.Play();
            }
        }

        // Need to destroy PolygonCollider2D and re-add for the new physics to take effect.
        public void Equip(GameObject player)
        {
            player.GetComponent<SpriteRenderer>().sprite = Sprite;
        }
    }
}
