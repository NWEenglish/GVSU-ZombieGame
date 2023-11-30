using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.GeneralGameLogic;
using Assets.Scripts.Human;
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
        protected override float CurrentSpeed { get; set; }

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

        private bool IsAttacking;
        private bool IsSprinting;
        private GameObject Player;
        private DateTime NextAudioTime;
        private DateTime LastAttackTime;
        private List<AudioSource> AudioSources;

        [SerializeField] private Sprite IdleSprite;
        [SerializeField] private Sprite AttackSprite;
        private SpriteRenderer SpriteRenderer;

        private void Start()
        {
            this.BaseStart();

            WaveLogic waveLogic = GameObject.Find(ObjectNames.GameLogic).GetComponent<WaveLogic>();

            Player = GameObject.Find(ObjectNames.Player);
            Health = waveLogic.Health;
            AudioSources = gameObject.GetComponents<AudioSource>().ToList();
            SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            IsSprinting = waveLogic.ShouldSprint;
            CurrentSpeed = IsSprinting ? SprintingSpeed : WalkingSpeed;

            foreach (AudioSource source in AudioSources)
            {
                source.mute = ShouldMute;
            }

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

                if (Player != null)
                {
                    if (Health > 0)
                    {
                        Player.GetComponent<PlayerLogic>().Status.AwardPoints(HitPoints);
                    }
                    else
                    {
                        Player.GetComponent<PlayerLogic>().Status.AwardPoints(KillPoints);
                    }
                }
            }
            else if (IsAttacking)
            {
                if (collision.gameObject.HasComponent<IHumanLogic>())
                {
                    collision.gameObject.GetComponent<IHumanLogic>().Hit();
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
