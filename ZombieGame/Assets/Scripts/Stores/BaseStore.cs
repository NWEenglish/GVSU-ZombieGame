using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using UnityEngine;

namespace Assets.Scripts.Stores
{
    public class BaseStore : MonoBehaviour
    {
        public StoreHelper Store { get; private set; }

        private TextMesh TextMesh;
        private AudioSource AmmoPurchaseSound;

        protected void OnStart(int costToBuy, int costForAmmo, int ammoReplenish, WeaponType weaponType)
        {
            AmmoPurchaseSound = gameObject.GetComponent<AudioSource>();

            Store = new StoreHelper(costToBuy, costForAmmo, ammoReplenish, AmmoPurchaseSound, weaponType);

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