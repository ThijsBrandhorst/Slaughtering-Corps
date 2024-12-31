using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;     

[RequireComponent(typeof(AudioSource))]
public class WeaponController : MonoBehaviour {
    // --------------------------------------------------------------------
    //                           References
    // --------------------------------------------------------------------
    [Header("References")]
    [Tooltip("Transform at the muzzle for accurate bullet direction.")]
    public Transform raycastOrigin;

    [Tooltip("Main camera to determine shooting direction.")]
    public Camera fpsCamera;

    private AudioSource audioSource;
    private PlayerControls playerControls; 

    // --------------------------------------------------------------------
    //                           Gun Stats
    // --------------------------------------------------------------------
    [Header("Gun Stats")]
    [Tooltip("Time between shots.")]
    public float fireRate = 0.1f;

    [Tooltip("Max distance of the hitscan ray.")]
    public float maxRange = 100f;

    [Tooltip("Maximum ammo in the magazine.")]
    public int maxAmmo = 30;

    [Tooltip("Time in seconds to complete a reload.")]
    public float reloadTime = 2f;

    private int currentAmmo;
    private float nextFireTime = 0f;
    public bool isReloading = false;
    private float reloadStartTime = 0f;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    // --------------------------------------------------------------------
    //                         ADS (Aim Down Sights)
    // --------------------------------------------------------------------
    //[Header("ADS Settings")]
    //Tooltip("Position & rotation when aiming down sights.")]
    //public Transform adsPosition;

    //[Tooltip("Position & rotation for normal hip-fire.")]
    // Transform defaultPosition;

    //[Tooltip("How fast the weapon lerps between Default and ADS positions.")]
    //public float adsSpeed = 5f;

    //private bool isADS = false; 

    // --------------------------------------------------------------------
    //                          Audio Clips
    // --------------------------------------------------------------------
    [Header("Audio Clips")]
    public AudioClip shootSound;
    public AudioClip reloadSound;

    // ====================================================================
    //                           Unity Methods
    // ====================================================================
    private void Awake() {
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();

        playerControls = new PlayerControls();

        playerControls.Player.Fire.performed += ctx => OnFire(ctx);
        playerControls.Player.Reload.performed += ctx => OnReload(ctx);
    }

    private void OnEnable() {
        if (playerControls != null)
            playerControls.Enable();
    }

    private void OnDisable() {
        if (playerControls != null)
            playerControls.Disable();
    }

    // ====================================================================
    //                    Input Action Callback Methods
    // ====================================================================
    private void OnFire(InputAction.CallbackContext context) {
        if (context.performed) {
            AttemptShoot();
        }
    }

    private void OnReload(InputAction.CallbackContext context) {
        if (context.performed) {
            StartReload();
        }
    }

    // ====================================================================
    //                         Weapon Logic Methods
    // ====================================================================
    private void AttemptShoot() {
        if (Time.timeScale == 0) return;

        if (Time.time < nextFireTime || isReloading)
            return;

        if (currentAmmo <= 0)
            return;

        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        Ray ray = new Ray(fpsCamera.transform.position, fpsCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRange)) {
            Debug.Log("Shot hit: " + hit.collider.name);
            //TO-DO: add damage
        }

        PlaySound(shootSound);
    }

    private void StartReload() {
        if (Time.timeScale == 0) return;

        if (isReloading || currentAmmo == maxAmmo)
            return;

        isReloading = true;
        reloadStartTime = Time.time;
        PlaySound(reloadSound);
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine() {
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    // ====================================================================
    //                           Utility Methods
    // ====================================================================
    private void PlaySound(AudioClip clip) {
        if (!clip || !audioSource)
            return;

        audioSource.PlayOneShot(clip);
    }

    public float GetReloadProgress() {
        if (!isReloading)
            return 0f;

        float elapsed = Time.time - reloadStartTime;
        return Mathf.Clamp01(elapsed / reloadTime);
    }
}
