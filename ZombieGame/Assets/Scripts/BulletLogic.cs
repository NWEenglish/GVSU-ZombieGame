using Assets.Scripts.Constants.Names;
using Assets.Scripts.Extensions;
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

            // Ignore bullet collision with player
            PolygonCollider2D playerCollider = GameObject.Find(ObjectNames.Player)?.GetComponent<PolygonCollider2D>();
            PolygonCollider2D bulletCollider = gameObject.GetComponent<PolygonCollider2D>();
            Physics2D.IgnoreCollision(playerCollider, bulletCollider);

            // Ignore collision with ally?
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
