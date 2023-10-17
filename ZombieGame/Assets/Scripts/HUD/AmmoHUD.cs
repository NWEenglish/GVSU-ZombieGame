using TMPro;

namespace Assets.Scripts.HUD
{
    public class AmmoHUD
    {
        private readonly TextMeshProUGUI HUDText;

        public AmmoHUD(TextMeshProUGUI ammoHUD)
        {
            HUDText = ammoHUD;
        }

        public void UpdateHUD(int ammoInClip, int remainingAmmo)
        {
            var text = $"Ammo: {ammoInClip} / {remainingAmmo}";

            if (HUDText.text != text)
            {
                HUDText.text = text;
            }
        }
    }
}
