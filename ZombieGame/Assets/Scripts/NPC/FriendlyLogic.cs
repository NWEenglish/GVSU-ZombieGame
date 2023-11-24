using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.NPC
{
    public class FriendlyLogic : BaseNpcLogic
    {
        protected override int Health { get; set; } = 100;
        protected override int HitPoints => 0;
        protected override int KillPoints => 0;
        protected override TeamType Team => TeamType.PlayerTeam;

        private void Start()
        {
            BaseStart();
        }

        private void Update()
        {
            CheckIfDead();
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
