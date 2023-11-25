using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.GeneralGameLogic;
using Assets.Scripts.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NPC
{
    public class EnemyLogic : BaseNpcLogic
    {
        public override TeamType Team => TeamType.ZombieTeam;

        protected override int Health { get; set; }
        protected override int HitPoints => 10;
        protected override int KillPoints => 60;

        private const float AudioRange = 10f;
        private const float AttackRange = 1.5f;
        private const float WalkingSpeed = 2.50f;
        private const float SprintingSpeed = 3.75f;
        private const float AttackSpeed = 1.5f;
        private const float PlayerHitSpeed = 0.25f;
        private const double MaxTimeBetweenAudio = 30;
        private const double MinTimeBetweenAudio = 5;
        private const double MinTimeBetweenAttacks = 2;
        private const double LengthTimeOfAttack = 1;

        private float CurrentSpeed;
        private bool IsAttacking;
        private bool IsSprinting;
        private DateTime NextAudioTime;
        private DateTime LastAttackTime;
        private List<AudioSource> AudioSources;

        private Sprite IdleSprite;
        private Sprite AttackSprite;
        private SpriteRenderer SpriteRenderer;

        private void Start()
        {
            this.BaseStart();

            Health = GameObject.Find(ObjectNames.GameLogic).GetComponent<WaveLogic>().Health;
            AudioSources = gameObject.GetComponents<AudioSource>().ToList();
            IdleSprite = GameObject.Find(ObjectNames.ZombieIdle).GetComponent<SpriteRenderer>().sprite;
            AttackSprite = GameObject.Find(ObjectNames.ZombieAttack).GetComponent<SpriteRenderer>().sprite;
            SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            IsSprinting = GameObject.Find(ObjectNames.GameLogic).GetComponent<WaveLogic>().ShouldSprint;
            CurrentSpeed = IsSprinting ? SprintingSpeed : WalkingSpeed;

            SetNextAudioPlayTime();
        }

        private void FixedUpdate()
        {
            if (Target != null)
            {
                Agent.SetDestination(Target.position);
                transform.rotation = Quaternion.LookRotation(Vector3.forward, Agent.velocity.normalized);
                transform.rotation *= Quaternion.Euler(0f, 0f, 90);
            }
        }

        // Think we can move this to BaseNPC once Friendly has audio/attacking concepts
        private void Update()
        {
            UpdateClosestTarget();
            CheckIfDead();

            if (DateTime.Now > NextAudioTime && AudioSources.Any(a => !a.isPlaying))
            {
                PlayAudio();
            }

            if (DateTime.Now > LastAttackTime.AddSeconds(MinTimeBetweenAttacks))
            {
                if (Target != null && Vector2.Distance(Target.position, gameObject.transform.position) <= AttackRange)
                {
                    AttackState();
                }
            }

            if (DateTime.Now > LastAttackTime.AddSeconds(LengthTimeOfAttack))
            {
                NormalState();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.HasComponent<BulletLogic>())
            {
                Health -= collision.gameObject.GetComponent<BulletLogic>().Damage;

                GameObject player = GameObject.Find(ObjectNames.Player);
                if (player != null)
                {
                    if (Health > 0)
                    {
                        player.GetComponent<PlayerLogic>().Status.AwardPoints(HitPoints);
                    }
                    else
                    {
                        player.GetComponent<PlayerLogic>().Status.AwardPoints(KillPoints);
                    }
                }
            }
            else if (IsAttacking)
            {
                if (collision.gameObject.HasComponent<PlayerLogic>())
                {
                    collision.gameObject.GetComponent<PlayerLogic>().Hit();
                    AttackHitState();
                }
                else if (collision.gameObject.HasComponent<FriendlyLogic>())
                {
                    collision.gameObject.GetComponent<FriendlyLogic>().Hit();
                    AttackHitState();
                }
            }
        }

        private void NormalState()
        {
            Agent.speed = CurrentSpeed;
            IsAttacking = false;
            SpriteRenderer.sprite = IdleSprite;
            Destroy(gameObject.GetComponent<PolygonCollider2D>());
            gameObject.AddComponent<PolygonCollider2D>();
        }

        private void AttackState()
        {
            SpriteRenderer.sprite = AttackSprite;

            Destroy(gameObject.GetComponent<PolygonCollider2D>());
            gameObject.AddComponent<PolygonCollider2D>();

            Agent.speed = AttackSpeed;
            LastAttackTime = DateTime.Now;
            IsAttacking = true;
        }

        private void AttackHitState()
        {
            Agent.speed = PlayerHitSpeed;
            LastAttackTime = DateTime.Now;
            IsAttacking = false;
        }

        private void PlayAudio()
        {
            if (Target != null && Vector2.Distance(Target.position, gameObject.transform.position) <= AudioRange)
            {
                int randomValue = (int)((Random.value * 100) % AudioSources.Count);
                AudioSources[randomValue].TryPlay();
            }
            SetNextAudioPlayTime();
        }

        private void SetNextAudioPlayTime()
        {
            int randomValue = (int)((Random.value * 100) % MaxTimeBetweenAudio);

            if (randomValue < MinTimeBetweenAudio)
            {
                randomValue += (int)MinTimeBetweenAudio;
            }

            NextAudioTime = DateTime.Now.AddSeconds(randomValue);
        }
    }
}
