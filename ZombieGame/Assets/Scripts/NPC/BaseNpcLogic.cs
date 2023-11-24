using System.Linq;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPC
{
    public abstract class BaseNpcLogic : MonoBehaviour
    {
        [SerializeField]
        protected Transform Target;
        protected NavMeshAgent Agent;

        protected abstract int Health { get; set; }

        protected abstract int HitPoints { get; }
        protected abstract int KillPoints { get; }
        protected abstract TeamType Team { get; }

        protected void BaseStart()
        {
            Agent = GetComponent<NavMeshAgent>();
            Agent.updateRotation = false;
            Agent.updateUpAxis = false;
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
                targets.Add(GameObject.FindFirstObjectByType<PlayerLogic>().transform);
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
    }
}
