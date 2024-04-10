using System;
using Assets.Scripts.Stores.SupportStores;
using Assets.Scripts.Stores.WeaponStores;
using Assets.Scripts.Weapons;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.Player
{
    public class PlayerStatus
    {
        public bool IsPlayerDead => Health <= 0;
        public int MaxHealth => 100;

        public int Health { get; private set; }
        public int Energy { get; private set; }
        public int Points { get; private set; }

        private DateTime LastHealthChange = DateTime.Now;
        private DateTime LastEnergyChange = DateTime.Now;

        private const int MaxEnergy = 1000;
        private const int HealthRegenTime = 200;
        private const int HealthAmountRegen = 1;
        private readonly Logger _logger = Logger.GetLogger();

        public PlayerStatus()
        {
            Health = MaxHealth;
            Energy = MaxEnergy;
            Points = 0;
        }

        public void Update(bool isMoving, bool isSprinting)
        {
            RegenHealth();
            EnergyLogic(isMoving, isSprinting);
        }

        public void TakeZombieHit()
        {
            Health -= 34;
            LastHealthChange = DateTime.Now;
            _logger.LogDebug($"Player took a hit from a zombie. | PlayerHealth: {Health}");
        }

        public void TakeBulletHit(int damage)
        {
            Health -= damage;
            LastHealthChange = DateTime.Now;
            _logger.LogDebug($"Player took a hit from a bullet. | PlayerHealth: {Health}");
        }

        public void AwardPoints(int points)
        {
            Points += points;
            _logger.LogDebug($"Player has earned points. | EarnedPoints: {points} | TotalPoints: {Points}");
        }

        public void HandleAmmoPurchase(BaseWeapon weapon, BaseWeaponStore store)
        {
            int points = Points;
            weapon.PurchaseAmmo(ref points, store);
            Points = points;
        }

        public BaseWeapon HandleWeaponPurchase(BaseWeaponStore store)
        {
            int points = Points;
            BaseWeapon weapon = store.PurchaseWeapon(ref points);
            Points = points;

            return weapon;
        }

        public void HandleSupportPurchase(BaseSupportStore store)
        {
            int points = Points;
            store.PurchaseSupport(ref points, store.gameObject.transform.position);
            Points = points;
        }

        private void RegenHealth()
        {
            if (Health != MaxHealth && DateTime.Now > LastHealthChange.AddMilliseconds(HealthRegenTime))
            {
                Health += HealthAmountRegen;

                if (Health > MaxHealth)
                {
                    Health = MaxHealth;
                }

                LastHealthChange = DateTime.Now;
            }
        }

        private void EnergyLogic(bool isMoving, bool isSprinting)
        {
            if (LastEnergyChange < DateTime.Now.AddMilliseconds(1))
            {
                if (isSprinting && isMoving)
                {
                    Energy -= 10;
                }
                else if (isMoving)
                {
                    Energy++;
                }
                else
                {
                    Energy += 3;
                }

                if (Energy < 0)
                {
                    Energy = 0;
                }
                else if (Energy > MaxEnergy)
                {
                    Energy = MaxEnergy;
                }

                LastEnergyChange = DateTime.Now;
            }
        }

        public void Reset()
        {
            Health = MaxHealth;
            Energy = MaxEnergy;
        }
    }
}
