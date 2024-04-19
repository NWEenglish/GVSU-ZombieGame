using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.GeneralGameLogic;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPC
{
    public abstract class BaseNpcLogic : MonoBehaviour
    {
        public abstract TeamType Team { get; }

        [SerializeField] protected Transform Target;
        protected NavMeshAgent Agent;
        protected BaseGameModeLogic GameModeLogic;
        protected bool ShouldMute = true;
        protected bool ShouldMove = false;

        protected abstract int Health { get; set; }
        protected abstract int HitPoints { get; }
        protected abstract int KillPoints { get; }
        protected abstract float CurrentSpeed { get; set; }

        protected abstract void TakeHit(int damage);

        public void InitValues()
        {
            ShouldMute = false;
            ShouldMove = true;
        }

        protected void BaseStart()
        {
            Agent = GetComponent<NavMeshAgent>();
            Agent.updateRotation = false;
            Agent.updateUpAxis = false;
            Agent.speed = ShouldMove ? CurrentSpeed : 0f;

            GameModeLogic = GameObject.Find(ObjectNames.GameLogic).GetComponent<BaseGameModeLogic>();
        }

        protected void UpdateClosestTarget()
        {
            var targets = GameObject.FindObjectsOfType<BaseNpcLogic>()
                .Where(npc => npc.Team != this.Team
                    && npc.name.EndsWith("(Clone)")) // To ensure not chasing after the "Spawnable" objects
                .Select(npc => npc.transform)
                .ToList();

            // Add the player to the list if NPC is not on the same team.
            if (Team != TeamType.PlayerTeam)
            {
                var player = GameObject.FindFirstObjectByType<PlayerLogic>();
                if (player != null)
                {
                    targets.Add(player.transform);
                }
            }

            Target = targets
                .OrderBy(t => Vector2.Distance(transform.position, t.position))
                .FirstOrDefault();
        }

        protected void CheckIfDead()
        {
            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.HasComponent<BulletLogic>())
            {
                TakeHit(collision.gameObject.GetComponent<BulletLogic>().Damage);
            }
        }
    }
}
