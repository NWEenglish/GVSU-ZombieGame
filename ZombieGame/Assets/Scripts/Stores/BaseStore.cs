using System.Collections.Generic;
using Assets.Scripts.Extensions;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Stores
{
    public abstract class BaseStore : MonoBehaviour
    {
        protected abstract int CostToBuy { get; }
        protected abstract string DisplayText { get; }
        protected abstract List<GameObject> PurchasedItems { get; set; }

        protected AudioSource PurchaseSound { get; private set; }

        private TextMesh TextMesh;

        private void Start()
        {
            PurchaseSound = gameObject.GetComponent<AudioSource>();

            TextMesh = gameObject.GetComponentInChildren<TextMesh>();
            TextMesh.color = Color.clear;
        }

        private void Update()
        {
            if (TextMesh.text != DisplayText)
            {
                TextMesh.text = DisplayText;
            }

            PurchasedItems.RemoveAll(pi => pi == null);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.HasComponent<PlayerLogic>())
            {
                TextMesh.color = Color.yellow;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.HasComponent<PlayerLogic>())
            {
                TextMesh.color = Color.clear;
            }
        }
    }
}
