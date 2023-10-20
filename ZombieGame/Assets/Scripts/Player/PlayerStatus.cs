using System;
using Assets.Scripts.Stores;
using Assets.Scripts.Weapons;

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

        public void TakeHit()
        {
            Health -= 34;
            LastHealthChange = DateTime.Now;
        }

        public void AwardPoints(int points)
        {
            Points += points;
        }

        public void HandleAmmoPurchase(Weapon weapon, BaseStore store)
        {
            int points = Points;
            weapon.PurchaseAmmo(ref points, store);
            Points = points;
        }

        public Weapon HandleWeaponPurchase(BaseStore store)
        {
            int points = Points;
            Weapon weapon = store.PurchaseWeapon(ref points);
            Points = points;

            return weapon;
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

                LastHealthChange = System.DateTime.Now;
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
    }
}
