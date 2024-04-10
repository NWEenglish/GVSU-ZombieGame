using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.GeneralGameLogic;
using Assets.Scripts.Human;
using Assets.Scripts.Player;
using Assets.Scripts.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NPC
{
    public class HumanLogic : BaseNpcLogic, IHumanLogic
    {
        public override TeamType Team => pTeam;
        private TeamType pTeam = TeamType.PlayerTeam; // Default because of zombie mode

        protected override int Health { get; set; } = 100;
        protected override int HitPoints => 0;
        protected override int KillPoints => 0;
        protected override float CurrentSpeed { get; set; } = 2.75f;

        public GameObject Muzzle;

        private BaseWeapon CurrentWeapon;
        private float NearbyRange => 10f;
        private float FollowRange => 3f;
        private int MinTimeBetweenChatter => 10;
        private int MinTimeBetweenMovementMs => 500;

        private bool CanShoot() => CurrentWeapon.CanShoot() && CurrentWeapon.RemainingClipAmmo > 0 && CurrentWeapon.RemainingTotalAmmo > 0;

        private NpcAim Aim;
        private List<GameObject> NearbyTargets = new List<GameObject>();
        private Rigidbody2D Rigidbody;
        private AudioSource Scream;
        private List<AudioSource> Chatter;
        private DateTime LastChatterTime = DateTime.MinValue;
        private DateTime LastStoppedTime = DateTime.MinValue;
        private bool MustFollowPlayer;
        private bool IsDisabled = false;
        private bool IsEnabled => !IsDisabled;
        private List<Transform> PointsOfInterest;

        public void InitValues(List<Transform> pointsOfInterest, TeamType team)
        {
            PointsOfInterest = pointsOfInterest;
            pTeam = team;

            this.InitValues();
        }

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

            var gameLogic = GameObject.Find(ObjectNames.GameLogic).GetComponent<BaseGameModeLogic>();
            MustFollowPlayer = gameLogic.GameMode == GameModeType.ZombieMode && this.Team == TeamType.PlayerTeam;
        }

        private void Update()
        {
            if (IsEnabled)
            {
                CheckIfDead();

                var closestVisibleEnemy = NearbyTargets
                    .Where(t => Aim.IsVisible(t))
                    .OrderBy(t => Vector2.Distance(gameObject.transform.position, t.transform.position))
                    .FirstOrDefault();

                // Aim at visible enemy
                if (closestVisibleEnemy != null)
                {
                    LastStoppedTime = DateTime.Now;

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
        }

        private void FixedUpdate()
        {
            if (IsEnabled)
            {
                if (Target != null && DateTime.Now.Subtract(LastStoppedTime).TotalMilliseconds > MinTimeBetweenMovementMs)
                {
                    Agent.SetDestination(Target.position);
                    if (Agent.remainingDistance > Agent.stoppingDistance)
                    {
                        transform.rotation = Quaternion.LookRotation(Vector3.forward, Agent.velocity.normalized);
                        transform.rotation *= Quaternion.Euler(0f, 0f, 90);
                    }
                    else
                    {
                        LastStoppedTime = DateTime.Now;
                    }
                }
                else
                {
                    SetTarget();
                }
            }
        }

        private void SetTarget()
        {
            // Follow player
            if (Target == null && MustFollowPlayer)
            {
                var player = GameObject.Find(ObjectNames.Player);
                if (player != null)
                {
                    Target = player.transform;
                }
            }
            // Go to Points-of-Interest
            else if (PointsOfInterest?.Any() ?? false)
            {
                Transform newTarget = null;

                do
                {
                    int randomValue = (int)(Random.value * 100);
                    newTarget = PointsOfInterest.ElementAt(randomValue % PointsOfInterest.Count);
                }
                while (newTarget == Target);

                Target = newTarget;
            }
        }

        private void AttemptCombatChatter()
        {
            if (Team == TeamType.PlayerTeam && !Scream.isPlaying && !Chatter.Any(audio => audio.isPlaying))
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
                CurrentWeapon.Shoot(bulletTargetAngle, Team);
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

            AttemptScream();
            SetCurrentColor(isShooting: false);

            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void AttemptScream()
        {
            // Chance to scream
            if (Team == TeamType.PlayerTeam && !Scream.isPlaying && (int)((Random.value * 100) % 5) == 0)
            {
                // Stop playing the other audio
                foreach (var audioSource in Chatter)
                {
                    audioSource.Pause();
                }

                Scream.TryPlay();
            }
        }

        // Unity color reference: https://docs.unity3d.com/ScriptReference/Color.html
        private void SetCurrentColor(bool isShooting)
        {
            Color color;

            // Friendlies are "normal" color
            if (Team == TeamType.PlayerTeam)
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

                color = new Color(1, green, blue);
            }
            else
            {
                // Per Unity: Color.Black RGBA is (0, 0, 0, 1)
                color = new Color(0, 0, 0);
            }

            gameObject.GetComponent<SpriteRenderer>().color = color;
        }

        protected override void TakeHit(int damage)
        {
            Health -= damage;
            AttemptScream();
        }

        public void Disable()
        {
            IsDisabled = true;
            Agent.isStopped = true;
        }
    }
}
