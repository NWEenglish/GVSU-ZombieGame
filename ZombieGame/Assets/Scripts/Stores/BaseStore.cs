using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using UnityEngine;

namespace Assets.Scripts.Stores
{
    public abstract class BaseStore : MonoBehaviour
    {
        public abstract int CostToBuy { get; }
        public abstract int CostForAmmo { get; }
        public abstract int AmmoReplenish { get; }
        public abstract WeaponType Type { get; }

        public StoreHelper Store { get; private set; }

        private TextMesh TextMesh;
        private AudioSource AmmoPurchaseSound;

        private void Start()
        {
            AmmoPurchaseSound = gameObject.GetComponent<AudioSource>();

            Store = new StoreHelper(CostToBuy, CostForAmmo, AmmoReplenish, AmmoPurchaseSound, Type);

            TextMesh = gameObject.GetComponentInChildren<TextMesh>();
            TextMesh.color = Color.clear;
            TextMesh.text = $"Buy: {Store.BuyCost}\nAmmo: {Store.AmmoCost}\nPress 'B' to buy";
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.name.Contains(ObjectNames.Player))
            {
                TextMesh.color = Color.yellow;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.name.Contains(ObjectNames.Player))
            {
                TextMesh.color = Color.clear;
            }
        }
    }
}