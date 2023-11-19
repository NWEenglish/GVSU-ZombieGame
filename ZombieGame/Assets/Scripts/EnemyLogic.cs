using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Extensions;
using Assets.Scripts.GeneralGameLogic;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class EnemyLogic : MonoBehaviour
    {
        [SerializeField] Transform target;
        NavMeshAgent agent;

        private const int HitPoints = 10;
        private const int KillPoints = 60;
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

        private int Health = 1;
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
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;

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
            if (target != null)
            {
                agent.SetDestination(target.position);
                transform.rotation = Quaternion.LookRotation(Vector3.forward, agent.velocity.normalized);
                transform.rotation *= Quaternion.Euler(0f, 0f, 90);
            }
        }

        private void Update()
        {
            if (Health <= 0)
            {
                Destroy(gameObject);
            }

            if (DateTime.Now > NextAudioTime && AudioSources.Any(a => !a.isPlaying))
            {
                PlayAudio();
            }

            if (DateTime.Now > LastAttackTime.AddSeconds(MinTimeBetweenAttacks))
            {
                if (target != null && Vector2.Distance(target.position, gameObject.transform.position) <= AttackRange)
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
            else if (collision.gameObject.HasComponent<PlayerLogic>() && IsAttacking)
            {
                collision.gameObject.GetComponent<PlayerLogic>().Hit();
                AttackHitState();
            }
        }

        private void NormalState()
        {
            agent.speed = CurrentSpeed;
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

            agent.speed = AttackSpeed;
            LastAttackTime = DateTime.Now;
            IsAttacking = true;
        }

        private void AttackHitState()
        {
            agent.speed = PlayerHitSpeed;
            LastAttackTime = DateTime.Now;
            IsAttacking = false;
        }

        private void PlayAudio()
        {
            if (target != null && Vector2.Distance(target.position, gameObject.transform.position) <= AudioRange)
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
