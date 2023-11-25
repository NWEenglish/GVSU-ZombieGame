using System.Linq;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class FriendlyLogic : BaseNpcLogic
    {
        public override TeamType Team => TeamType.PlayerTeam;

        protected override int Health { get; set; } = 100;
        protected override int HitPoints => 0;
        protected override int KillPoints => 0;

        private void Start()
        {
            BaseStart();
        }

        private void Update()
        {
            CheckIfDead();
        }

        public void OnTriggerStay2D(Collider2D collision)
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
            var fovSprite = gameObject.GetComponentsInChildren<SpriteRenderer>().Where(sr => sr.gameObject.name.Contains("FOV")).FirstOrDefault();

            if (target != null)
            {

                fovSprite.color = new Color(Color.red.r, Color.red.g, Color.red.b, .5f);
            }
            else
            {
                fovSprite.color = new Color(Color.green.r, Color.green.g, Color.green.b, .5f);
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
