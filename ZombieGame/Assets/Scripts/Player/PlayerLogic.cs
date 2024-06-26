using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.GeneralGameLogic;
using Assets.Scripts.Helpers;
using Assets.Scripts.HUD;
using Assets.Scripts.Human;
using Assets.Scripts.Stores;
using Assets.Scripts.Stores.SupportStores;
using Assets.Scripts.Stores.WeaponStores;
using Assets.Scripts.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.Player
{
    public class PlayerLogic : MonoBehaviour, IHumanLogic
    {
        public PlayerStatus Status { get; private set; }

        private Rigidbody2D Body;
        private BaseWeaponStore CurrentWeaponStore;
        private BaseSupportStore CurrentSupportStore;
        private BaseWeapon CurrentWeapon;
        private List<BaseWeapon> Weapons;
        private AmmoHUD AmmoHUD;
        private HealthHUD HealthHUD;
        private PointsHUD PointsHUD;
        private GameOverHUD GameOverScreen;
        private BaseGameModeLogic GameModeLogic;

        private bool IsDisabled = false;
        private bool IsEnabled => !IsDisabled;

        private const float PlayerWalkingSpeed = 5f;
        private const float PlayerSprintSpeed = PlayerWalkingSpeed * 2;
        private const float PlayerReloadSpeed = PlayerWalkingSpeed / 2;
        private const double HealthBlinkTime = 1;
        private readonly Logger _logger = Logger.GetLogger();

        private float PlayerSpeed => GetPlayerSpeed();
        private float HorizontalSpeed => Input.GetAxisRaw("Horizontal") * PlayerSpeed;
        private float VerticalSpeed => Input.GetAxisRaw("Vertical") * PlayerSpeed;

        private bool IsMoving => Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        private bool IsSprinting => Input.GetKey(KeyCode.LeftShift);
        private bool IsReloading => Input.GetKeyDown(KeyCode.R);
        private bool IsPurchasing => Input.GetKeyDown(KeyCode.B);

        private void Start()
        {
            Status = new PlayerStatus();
            Body = gameObject.GetComponent<Rigidbody2D>();

            GameModeLogic = GameObject.Find(ObjectNames.GameLogic).GetComponent<BaseGameModeLogic>();

            AmmoHUD = new AmmoHUD(GameObject.Find(ObjectNames.Ammo_HUD).GetComponent<TextMeshProUGUI>());

            var healthBlink = new BlinkHelper(GameObject.Find(ObjectNames.Health_Indicator_HUD).GetComponent<Image>(), Color.red, HealthBlinkTime);
            HealthHUD = new HealthHUD(GameObject.Find(ObjectNames.Health_Panel_HUD).GetComponent<Image>(), healthBlink, Status.MaxHealth);

            Weapons = new List<BaseWeapon>()
            {
                GameObject.Find(ObjectNames.Pistol).GetComponent<BasePlayer>().Weapon
            };

            GameOverScreen = new GameOverHUD()
            {
                GameOverTitle = GameObject.Find(ObjectNames.GameOver_Title).GetComponent<TextMeshProUGUI>(),
                GameOverSubtext = GameObject.Find(ObjectNames.GameOver_Subtext).GetComponent<TextMeshProUGUI>()
            };

            ZombieModeSetup();
            NonZombieModeSetup();

            CurrentWeapon = Weapons.First();
            Equip();
        }

        private void ZombieModeSetup()
        {
            if (GameModeLogic.GameMode == GameModeType.ZombieMode)
            {
                PointsHUD = new PointsHUD(GameObject.Find(ObjectNames.Points_HUD).GetComponent<TextMeshProUGUI>());
            }
        }

        private void NonZombieModeSetup()
        {
            if (GameModeLogic.GameMode == GameModeType.NonZombieMode)
            {
                Weapons.Add(GameObject.Find(ObjectNames.Rifle).GetComponent<BasePlayer>().Weapon);
                Weapons.Reverse();
            }
        }

        private void FixedUpdate()
        {
            if (IsEnabled)
            {
                Move();
                Rotate();
            }
        }

        private void Update()
        {
            if (IsEnabled)
            {
                PurchaseWeapons();
                PurchaseSupport();
                SwitchWeapons();
                ShootUpdate();
                ReloadLogic();

                Status.Update(IsMoving, IsSprinting);
                AmmoHUD.UpdateHUD(CurrentWeapon.RemainingClipAmmo, CurrentWeapon.RemainingTotalAmmo);
                HealthHUD.UpdateHUD(Status.Health);

                if (PointsHUD != null)
                {
                    PointsHUD.UpdateHUD(Status.Points);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.HasComponent<BaseStore>())
            {
                if (collision.gameObject.HasComponent<BaseWeaponStore>())
                {
                    CurrentWeaponStore = collision.GetComponent<BaseWeaponStore>();
                }
                else
                {
                    CurrentSupportStore = collision.GetComponent<BaseSupportStore>();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.HasComponent<BaseStore>())
            {
                if (collision.gameObject.HasComponent<BaseWeaponStore>())
                {
                    CurrentWeaponStore = null;
                }
                else
                {
                    CurrentSupportStore = null;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out BulletLogic bulletLogic))
            {
                Status.TakeBulletHit(bulletLogic.Damage);
                HandleDamage();
            }
        }

        // Mainly used for if a zombie attacks the player
        public void Hit()
        {
            Status.TakeZombieHit();
            HandleDamage();
        }

        private void HandleDamage()
        {
            if (Status.Health <= 0)
            {
                _logger.LogDebug($"Player has died. | PlayerHealth: {Status.Health}");

                if (GameModeLogic.GameMode == GameModeType.ZombieMode)
                {
                    Destroy(gameObject);
                    GameOverScreen.ShowZombiesGameOver(GameObject.Find(ObjectNames.GameLogic).GetComponent<WaveLogic>().Wave);
                }
                else
                {
                    _logger.LogDebug("Player is dead, but is allowed to respawning.");
                }
            }

            HealthHUD.DisplayHit();
        }

        private void SwitchWeapons()
        {
            if (Input.GetKeyDown(KeyCode.E) && !CurrentWeapon.IsReloading())
            {
                CurrentWeapon = Weapons[(Weapons.IndexOf(CurrentWeapon) + 1) % Weapons.Count];
                Equip();
            }
        }

        private void PurchaseWeapons()
        {
            if (IsPurchasing && CurrentWeaponStore != null)
            {
                _logger.LogDebug($"Player is making a purchase. | CurrentStoreName: {CurrentWeaponStore.name}");
                if (Weapons.Any(w => w.Type == CurrentWeaponStore.Type))
                {
                    _logger.LogDebug($"Player is attempting to purchase ammo.");
                    Status.HandleAmmoPurchase(CurrentWeapon, CurrentWeaponStore);
                }
                else
                {
                    _logger.LogDebug($"Player is attempting to purchase a weapon.");
                    var newWeapon = Status.HandleWeaponPurchase(CurrentWeaponStore);

                    if (newWeapon != null)
                    {
                        if (Weapons.Count == 2)
                        {
                            _logger.LogDebug($"Player has too many weapons. Replacing the current weapon. | CurrentWeapon: {CurrentWeapon.Type}");
                            Weapons.Remove(CurrentWeapon);
                            Weapons.Add(newWeapon);
                        }
                        else
                        {
                            Weapons.Add(newWeapon);
                        }

                        CurrentWeapon = newWeapon;
                        Equip();
                    }
                }
            }
        }

        private void PurchaseSupport()
        {
            if (IsPurchasing && CurrentSupportStore != null)
            {
                _logger.LogDebug($"Playing is making a purchase. | CurrentStoreName: {CurrentSupportStore.name}");
                Status.HandleSupportPurchase(CurrentSupportStore);
            }
        }

        private float GetPlayerSpeed()
        {
            float speed;

            if (CurrentWeapon.IsReloading())
            {
                speed = PlayerReloadSpeed;
            }
            else if (IsSprinting && Status.Energy > 0)
            {
                speed = PlayerSprintSpeed;
            }
            else
            {
                speed = PlayerWalkingSpeed;
            }

            return speed;
        }

        private void ReloadLogic()
        {
            if (IsReloading)
            {
                CurrentWeapon.Reload();
            }
        }

        private void ShootUpdate()
        {
            if (Input.GetMouseButton(0) && CurrentWeapon.FireType == FireType.FullyAutomatic)
            {
                float bulletTargetAngle = Body.rotation;
                CurrentWeapon.Shoot(bulletTargetAngle, TeamType.PlayerTeam);
            }
            else if (Input.GetMouseButtonDown(0) && CurrentWeapon.FireType == FireType.SemiAutomatic)
            {
                float bulletTargetAngle = Body.rotation;
                CurrentWeapon.Shoot(bulletTargetAngle, TeamType.PlayerTeam);
            }
        }

        private void Move()
        {
            Vector2 speed = new Vector2(HorizontalSpeed, VerticalSpeed).normalized * PlayerSpeed;
            Body.velocity = speed;
            Body.angularVelocity = 0f;
        }

        // Borrowed from TankGame
        private void Rotate()
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 wsp = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 target = new Vector2(mousePosition.x - wsp.x, mousePosition.y - wsp.y);

            Quaternion quaternion = Quaternion.LookRotation(Vector3.forward, target);
            Body.transform.rotation = Quaternion.RotateTowards(Body.transform.rotation, quaternion, 100);
            Body.transform.rotation *= Quaternion.Euler(0f, 0f, 90f);
        }

        private void Equip()
        {
            CurrentWeapon.Equip(gameObject);
            Destroy(gameObject.GetComponent<PolygonCollider2D>());
            gameObject.AddComponent<PolygonCollider2D>();
        }

        public void Disable()
        {
            IsDisabled = true;
            Body.velocity = new Vector2(0, 0);
        }

        public void Enable()
        {
            IsDisabled = false;
        }

        public void ResetPlayer()
        {
            Status.Reset();

            foreach (BaseWeapon weapon in Weapons)
            {
                weapon.Reset();
            }
        }
    }
}