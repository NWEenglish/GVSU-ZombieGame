using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.GeneralGameLogic;
using Assets.Scripts.Helpers;
using Assets.Scripts.HUD;
using Assets.Scripts.Stores;
using Assets.Scripts.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerLogic : MonoBehaviour
    {
        public PlayerStatus Status { get; private set; }

        private Rigidbody2D Body;
        private BaseStore CurrentStore;
        private BaseWeapon CurrentWeapon;
        private List<BaseWeapon> Weapons;
        private AmmoHUD AmmoHUD;
        private HealthHUD HealthHUD;
        private PointsHUD PointsHUD;
        private GameOverHUD GameOverScreen;

        private const float PlayerWalkingSpeed = 5f;
        private const float PlayerSprintSpeed = PlayerWalkingSpeed * 2;
        private const float PlayerReloadSpeed = PlayerWalkingSpeed / 2;
        private const double HealthBlinkTime = 1;

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

            AmmoHUD = new AmmoHUD(GameObject.Find(ObjectNames.Ammo_HUD).GetComponent<TextMeshProUGUI>());
            PointsHUD = new PointsHUD(GameObject.Find(ObjectNames.Points_HUD).GetComponent<TextMeshProUGUI>());

            var healthBlink = new BlinkHelper(GameObject.Find(ObjectNames.Health_Indicator_HUD).GetComponent<Image>(), Color.red, HealthBlinkTime);
            HealthHUD = new HealthHUD(GameObject.Find(ObjectNames.Health_Panel_HUD).GetComponent<Image>(), healthBlink, Status.MaxHealth);

            Weapons = new List<BaseWeapon>()
            {
                GameObject.Find(ObjectNames.Pistol).GetComponent<PlayerPistol>().Weapon
            };

            GameOverScreen = new GameOverHUD()
            {
                GameOverTitle = GameObject.Find(ObjectNames.GameOver_Title).GetComponent<TextMeshProUGUI>(),
                GameOverWave = GameObject.Find(ObjectNames.GameOver_Subtext).GetComponent<TextMeshProUGUI>()
            };

            CurrentWeapon = Weapons.First();
            Equip();
        }

        private void FixedUpdate()
        {
            Move();
            Rotate();
        }

        private void Update()
        {
            PurchaseWeapons();
            SwitchWeapons();
            ShootUpdate();
            ReloadLogic();

            Status.Update(IsMoving, IsSprinting);
            AmmoHUD.UpdateHUD(CurrentWeapon.RemainingClipAmmo, CurrentWeapon.RemainingTotalAmmo);
            HealthHUD.UpdateHUD(Status.Health);
            PointsHUD.UpdateHUD(Status.Points);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name.Contains(ObjectNames.Store))
            {
                CurrentStore = collision.GetComponent<BaseStore>();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.name.Contains(ObjectNames.Store))
            {
                CurrentStore = null;
            }
        }

        public void Hit()
        {
            Status.TakeHit();

            if (Status.Health <= 0)
            {
                Destroy(gameObject);
                GameOverScreen.ShowGameOver(GameObject.Find(ObjectNames.GameLogic).GetComponent<WaveLogic>().Wave);
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
            if (IsPurchasing && CurrentStore != null)
            {
                if (Weapons.Any(w => w.Type == CurrentStore.Type))
                {
                    Status.HandleAmmoPurchase(CurrentWeapon, CurrentStore);
                }
                else
                {
                    var newWeapon = Status.HandleWeaponPurchase(CurrentStore);

                    if (newWeapon != null)
                    {
                        if (Weapons.Count == 2)
                        {
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
                CurrentWeapon.Shoot(bulletTargetAngle);
            }
            else if (Input.GetMouseButtonDown(0) && CurrentWeapon.FireType == FireType.SemiAutomatic)
            {
                float bulletTargetAngle = Body.rotation;
                CurrentWeapon.Shoot(bulletTargetAngle);
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
    }
}