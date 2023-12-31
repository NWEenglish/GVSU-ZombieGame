using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.Human;
using Assets.Scripts.Player;
using Assets.Scripts.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NPC
{
    // While right now it's FriendlyLogic, this will be turned into HumanLogic, which is intended to have sub-children for various friendly and hostile NPCs.
    public class FriendlyLogic : BaseNpcLogic, IHumanLogic
    {
        public override TeamType Team => TeamType.PlayerTeam;

        protected override int Health { get; set; } = 100;
        protected override int HitPoints => 0;
        protected override int KillPoints => 0;
        protected override float CurrentSpeed { get; set; } = 2.75f;

        public GameObject Muzzle;

        private BaseWeapon CurrentWeapon;
        private float NearbyRange => 10f;
        private float FollowRange => 3f;
        private int MinTimeBetweenChatter => 10;
        private int MinTimeBetweenMovementAfterSpotMs => 500;

        private bool CanShoot() => CurrentWeapon.CanShoot() && CurrentWeapon.RemainingClipAmmo > 0 && CurrentWeapon.RemainingTotalAmmo > 0;

        private NpcAim Aim;
        private List<GameObject> NearbyTargets = new List<GameObject>();
        private Rigidbody2D Rigidbody;
        private AudioSource Scream;
        private List<AudioSource> Chatter;
        private DateTime LastChatterTime = DateTime.MinValue;
        private DateTime LastEnemySpotTime = DateTime.MinValue;

        private void Start()
        {
            BaseStart();

            Agent.stoppingDistance = FollowRange;

            gameObject.GetComponent<CircleCollider2D>().radius = NearbyRange;

            Aim = new NpcAim(gameObject.transform);
            Rigidbody = gameObject.GetComponent<Rigidbody2D>();

            GameObject bullet = GameObject.Find(ObjectNames.Bullet);
            AudioSource reloadAudio = Muzzle.GetComponent<AudioSource>();
            CurrentWeapon = new RifleWeapon(bullet, Muzzle, reloadAudio, null);
            CurrentWeapon.EnableUnlimitedAmmo(gameObject);

            Scream = gameObject.GetComponents<AudioSource>().First(audio => audio.clip.name.Contains("scream"));
            Chatter = gameObject.GetComponents<AudioSource>().Where(audio => !audio.clip.name.Contains("scream")).ToList();

            Scream.mute = ShouldMute;
            foreach (var audio in Chatter)
            {
                audio.mute = ShouldMute;
            }
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
                LastEnemySpotTime = DateTime.Now;

                SetCurrentColor(isShooting: true);
                RotateTowardsGameObject(closestVisibleEnemy);
                AttemptToShoot();
                AttemptCombatChatter();
            }
            else
            {
                SetCurrentColor(isShooting: false);
            }

            // Check if needs reload
            if (CurrentWeapon.RemainingClipAmmo == 0)
            {
                CurrentWeapon.Reload();
            }
        }

        private void FixedUpdate()
        {
            if (Target != null && DateTime.Now.Subtract(LastEnemySpotTime).Milliseconds > MinTimeBetweenMovementAfterSpotMs)
            {
                Agent.SetDestination(Target.position);
                if (Agent.remainingDistance > Agent.stoppingDistance)
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, Agent.velocity.normalized);
                    transform.rotation *= Quaternion.Euler(0f, 0f, 90);
                }
            }
        }

        private void AttemptCombatChatter()
        {
            if (!Scream.isPlaying && !Chatter.Any(audio => audio.isPlaying))
            {
                // Time out completed and now checking for chance for bots to have chatter.
                if (DateTime.Now.Subtract(LastChatterTime).TotalSeconds > MinTimeBetweenChatter
                    && (int)((Random.value * 100) % 100) == 0)
                {
                    AudioSource audio = Chatter.ElementAt((int)((Random.value * 100) % Chatter.Count()));
                    audio.TryPlay();
                    LastChatterTime = DateTime.Now;
                }
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

        private void RotateTowardsGameObject(GameObject other)
        {
            Vector2 target = new Vector2(other.transform.position.x - gameObject.transform.position.x, other.transform.position.y - gameObject.transform.position.y);
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
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // If it left the range, no longer considered for targeting
            NearbyTargets.RemoveAll(t => t == collision.gameObject);
        }

        public void Hit()
        {
            Health -= 15;

            // Chance to scream when hit
            if (!Scream.isPlaying && (int)((Random.value * 100) % 5) == 0)
            {
                // Stop playing the other audio
                foreach (var audioSource in Chatter)
                {
                    audioSource.Pause();
                }

                Scream.TryPlay();
            }

            SetCurrentColor(isShooting: false);

            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void SetCurrentColor(bool isShooting)
        {
            // Per Unity: Color.Yellow RGBA is (1, 0.92, 0.016, 1)
            float greenForYellowMix = 0.92f;
            float blueForYellowMix = 0.016f;

            float colorRatio = Health / 100f;
            float green = colorRatio;
            float blue = colorRatio;

            if (isShooting)
            {
                green *= greenForYellowMix;
                blue *= blueForYellowMix;
            }

            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, green, blue);
        }
    }
}
