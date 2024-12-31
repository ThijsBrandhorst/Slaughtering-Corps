using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSUIManager : MonoBehaviour {
    [Header("UI References")]
    public Image crosshairImage;
    public TMP_Text ammoText;

    [Header("Reload Bar UI")]
    [Tooltip("The parent GameObject of the reload bar (so we can hide/show it).")]
    public GameObject reloadBarRoot;

    [Tooltip("The Image component used as a fill bar.")]
    public Image reloadFillImage;

    [Header("Game References")]
    public WeaponController weapon;
    public Camera mainCamera;

    [Header("Crosshair Settings")]
    public Color defaultCrosshairColor = Color.white;
    public Color enemyCrosshairColor = Color.red;
    public float crosshairCheckRange = 100f;

    private void Update() {
        UpdateAmmoUI();
        UpdateCrosshairColor();
        UpdateReloadBar();
    }

    // ----------------------------------------------------------------
    // Ammo UI
    // ----------------------------------------------------------------
    private void UpdateAmmoUI() {
        if (!weapon || !ammoText)
            return;
        ammoText.text = $"{weapon.CurrentAmmo} / {weapon.MaxAmmo}";
    }

    // ----------------------------------------------------------------
    // Crosshair Color
    // ----------------------------------------------------------------
    private void UpdateCrosshairColor() {
        if (!crosshairImage || !mainCamera)
            return;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, crosshairCheckRange)) {
            if (hit.collider.CompareTag("Enemy"))
                crosshairImage.color = enemyCrosshairColor;
            else
                crosshairImage.color = defaultCrosshairColor;
        } else {
            crosshairImage.color = defaultCrosshairColor;
        }
    }

    // ----------------------------------------------------------------
    // Reload Bar
    // ----------------------------------------------------------------
    private void UpdateReloadBar() {
        if (!weapon || !reloadBarRoot || !reloadFillImage)
            return;

        bool isReloading = weapon.isReloading; 
        reloadBarRoot.SetActive(isReloading);

        if (isReloading) {
            float progress = weapon.GetReloadProgress();
            reloadFillImage.fillAmount = progress;
        }
    }
}
