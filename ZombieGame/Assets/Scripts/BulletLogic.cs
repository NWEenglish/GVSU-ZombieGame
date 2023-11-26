using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.NPC;
using UnityEngine;

namespace Assets.Scripts
{
    public class BulletLogic : MonoBehaviour
    {
        public int Damage { get; private set; }

        private bool HasCollided = false;
        private AudioSource Audio;

        public void InitValues(int damage)
        {
            Damage = damage;
        }

        private void Start()
        {
            Audio = gameObject.GetComponent<AudioSource>();
            Audio.TryPlay();

            PolygonCollider2D bulletCollider = gameObject.GetComponent<PolygonCollider2D>();

            // Ignore bullet collision with player
            PolygonCollider2D playerCollider = GameObject.Find(ObjectNames.Player)?.GetComponent<PolygonCollider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }

            // Ignore collision with allies
            List<PolygonCollider2D> allyColliders = GameObject.FindGameObjectsWithTag(TagNames.NPC)
                .Select(npc => npc.GetComponent<BaseNpcLogic>())
                .Where(npc => npc.Team == TeamType.PlayerTeam)
                .Select(npc => npc.GetComponent<PolygonCollider2D>())
                .ToList();

            foreach (var allyCollider in allyColliders)
            {
                Physics2D.IgnoreCollision(allyCollider, bulletCollider);
            }
        }

        private void Update()
        {
            if (HasCollided && !Audio.isPlaying)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            RemoveBullet();
        }

        private void RemoveBullet()
        {
            Destroy(gameObject.GetComponent<PolygonCollider2D>());
            Destroy(gameObject.GetComponent<Rigidbody2D>());
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            HasCollided = true;
        }
    }
}
