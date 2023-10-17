using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using UnityEngine;

namespace Assets.Scripts.Stores
{
    public class RifleStore : MonoBehaviour
    {
        private const int CostToBuy = 1500;
        private const int CostForAmmo = 500;
        private const int AmmoReplenish = 100;
        private const WeaponType Type = WeaponType.Rifle;

        private TextMesh TextMesh;
        private AudioSource AmmoPurchaseSound;

        public StoreHelper Store { get; private set; }

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
