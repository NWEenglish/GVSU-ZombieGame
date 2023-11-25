using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.Player;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class FriendlyLogic : BaseNpcLogic
    {
        public override TeamType Team => TeamType.PlayerTeam;

        protected override int Health { get; set; } = 100;
        protected override int HitPoints => 0;
        protected override int KillPoints => 0;

        public GameObject Muzzle;

        private BaseWeapon CurrentWeapon;
        private float NearbyRange => 10f;
        private int TimeBetweenShotMs => 500;
        private bool CanShoot() => true;

        private NpcAim Aim;
        private List<GameObject> NearbyTargets = new List<GameObject>();
        private Rigidbody2D Rigidbody;

        private void Start()
        {
            BaseStart();

            gameObject.GetComponent<CircleCollider2D>().radius = NearbyRange;

            Aim = gameObject.GetComponentInChildren<NpcAim>();
            Rigidbody = gameObject.GetComponent<Rigidbody2D>();

            GameObject bullet = GameObject.Find(ObjectNames.Bullet);
            AudioSource reloadAudio = Muzzle.GetComponent<AudioSource>();
            CurrentWeapon = new RifleWeapon(bullet, Muzzle, reloadAudio, null);
        }

        private void Update()
        {
            CheckIfDead();

            var closestVisibleEnemy = NearbyTargets
                .Where(t => Aim.IsVisible(t))
                .OrderBy(t => Vector2.Distance(gameObject.transform.position, t.transform.position))
                .FirstOrDefault();

            // Aim at visible enemy
            if (closestVisibleEnemy != null)
            {
                RotateTowardsEnemy(closestVisibleEnemy);
                AttemptToShoot();
            }

            // Check if needs reload
            if (CurrentWeapon.RemainingClipAmmo == 0)
            {
                CurrentWeapon.Reload();
            }
        }

        private void AttemptToShoot()
        {
            if (CanShoot())
            {
                float bulletTargetAngle = Rigidbody.rotation;
                CurrentWeapon.Shoot(bulletTargetAngle);
            }
        }

        private void RotateTowardsEnemy(GameObject enemy)
        {
            Vector2 target = new Vector2(enemy.transform.position.x - gameObject.transform.position.x, enemy.transform.position.y - gameObject.transform.position.y);
            Quaternion quaternion = Quaternion.LookRotation(Vector3.forward, target);
            Rigidbody.transform.rotation = Quaternion.RotateTowards(Rigidbody.transform.rotation, quaternion, 100);
            Rigidbody.transform.rotation *= Quaternion.Euler(0f, 0f, 90f);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if enemy
            BaseNpcLogic npc = collision.gameObject.GetComponent<BaseNpcLogic>();
            PlayerLogic player = collision.gameObject.GetComponent<PlayerLogic>();

            GameObject target = null;

            // Collision is another NPC and not is on the same team as us.
            if (npc != null && npc.Team != this.Team)
            {
                target = npc.gameObject;
            }
            // Collision is player and we're not on the player's team.
            else if (player != null && TeamType.PlayerTeam != this.Team)
            {
                target = player.gameObject;
            }

            // Perform action with target.
            if (target != null)
            {
                NearbyTargets.Add(target);

                var audio = gameObject.GetComponent<AudioSource>();
                if (audio.isPlaying == false)
                {
                    gameObject.GetComponent<AudioSource>().TryPlay();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // If it left the range, no longer considered for targeting
            NearbyTargets.RemoveAll(t => t == collision.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.HasComponent<BulletLogic>())
            {
                // Friendly fire?
                //Health -= collision.gameObject.GetComponent<BulletLogic>().Damage;

                GameObject player = GameObject.Find(ObjectNames.Player);
                if (player != null)
                {
                    // Maybe deduct points if hitting ally?
                    //if (Health > 0)
                    //{
                    //    player.GetComponent<PlayerLogic>().Status.AwardPoints(HitPoints);
                    //}
                    //else
                    //{
                    //    player.GetComponent<PlayerLogic>().Status.AwardPoints(KillPoints);
                    //}
                }
            }
        }

        public void Hit()
        {
            Health -= 15;

            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
