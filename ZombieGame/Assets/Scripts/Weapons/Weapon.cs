using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
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
        public abstract WeaponType Type { get; }

        public bool IsReloading() => ReloadSound.isPlaying;
        public abstract bool CanShoot();
        public abstract void Shoot(float angle);
        public abstract void PurchaseAmmo(ref int Points, BaseStore store);

        public void Reload()
        {
            if (AmmoClip != AmmoClipSize && RemainingAmmo > 0 && !ReloadSound.isPlaying)
            {
                while (RemainingAmmo > 0 && AmmoClip < AmmoClipSize)
                {
                    RemainingAmmo--;
                    AmmoClip++;
                }
                ReloadSound.TryPlay();
            }
        }

        // Need to destroy PolygonCollider2D and re-add for the new physics to take effect.
        public void Equip(GameObject player)
        {
            player.GetComponent<SpriteRenderer>().sprite = Sprite;
        }
    }
}
