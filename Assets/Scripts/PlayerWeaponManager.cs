using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public List<WeaponController> startingWeapons = new List<WeaponController>();
    public Transform weaponParentSocket;

    [Header("Positions")]
    public Transform defaultWeaponPosition;  // DefaultParentSocket
    public Transform aimingPosition;         // AimParentSocket

    [Header("Aim Settings (FOV Zoom)")]
    public float aimFOV = 30f;
    public float aimSpeed = 10f;
    public Image aimReticle;                 // simple crosshair UI

    [Header("Scope Setup (Sniper Zoom)")]
    public Camera scopeCamera;               // secondary camera rendering to texture
    public RawImage scopeView;               // RawImage showing the scope RenderTexture
    public Image scopeOverlay;               // scope border graphic

    [HideInInspector] public int activeWeaponIndex { get; private set; }

    private WeaponController[] weaponSlots = new WeaponController[5];
    private Camera playerCamera;
    private float defaultFOV;
    private bool isAiming = false;

    void Start()
    {
        // Cámara principal
        playerCamera = Camera.main;
        if (playerCamera == null)
            Debug.LogError("PlayerWeaponManager: No se encontró Camera.main. Asegura la etiqueta MainCamera.");
        else
            defaultFOV = playerCamera.fieldOfView;

        // Verificar referencias
        if (weaponParentSocket == null || defaultWeaponPosition == null || aimingPosition == null)
            Debug.LogError("PlayerWeaponManager: Faltan referencias de sockets en el Inspector.");
        if (aimReticle == null)
            Debug.LogWarning("PlayerWeaponManager: No se asignó AimReticle. No se mostrará crosshair simple.");
        if ((scopeCamera == null) ^ (scopeView == null) ^ (scopeOverlay == null))
            Debug.LogWarning("PlayerWeaponManager: Debes asignar todos los campos de Scope (Camera, RawImage, Overlay) o ninguno.");

        // Desactivar scope al inicio
        if (scopeCamera  != null) scopeCamera.enabled  = false;
        if (scopeView    != null) scopeView.enabled    = false;
        if (scopeOverlay != null) scopeOverlay.enabled = false;

        // Instanciar armas
        activeWeaponIndex = -1;
        foreach (var prefab in startingWeapons)
            AddWeapon(prefab);

        // Auto-equipar primera arma disponible
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != null)
            {
                SwitchWeapon(i);
                break;
            }
        }
    }

    void Update()
    {
        HandleSwitchWeapon();
        HandleAim();
        HandleFire();
    }

    private void HandleSwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0);
        // Añade más teclas (Alpha2, Alpha3...) para más slots si quieres
    }

    private void HandleAim()
    {
        // Detectar right-click down/up
        if (Input.GetMouseButtonDown(1)) isAiming = true;
        if (Input.GetMouseButtonUp(1))   isAiming = false;

        // Mostrar crosshair simple solo si no tienes scopeCamera
        if (aimReticle != null)
            aimReticle.enabled = isAiming && scopeCamera == null;

        // Mover arma suavemente
        Transform targetSocket = isAiming ? aimingPosition : defaultWeaponPosition;
        weaponParentSocket.position = Vector3.Lerp(
            weaponParentSocket.position,
            targetSocket.position,
            Time.deltaTime * aimSpeed
        );
        weaponParentSocket.rotation = Quaternion.Slerp(
            weaponParentSocket.rotation,
            targetSocket.rotation,
            Time.deltaTime * aimSpeed
        );

        // Si tienes cámara de scope asignada, alterna entre cámaras
        if (scopeCamera != null && scopeView != null && scopeOverlay != null)
        {
            scopeCamera.enabled      = isAiming;
            scopeView.enabled        = isAiming;
            scopeOverlay.enabled     = isAiming;

            if (playerCamera != null)
                playerCamera.enabled = !isAiming;
        }
        else if (playerCamera != null)
        {
            // Zoom simple por FOV
            float targetFOV = isAiming ? aimFOV : defaultFOV;
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                targetFOV,
                Time.deltaTime * aimSpeed
            );
        }
    }

    private void HandleFire()
    {
        if (Input.GetMouseButtonDown(0) && activeWeaponIndex >= 0)
        {
            weaponSlots[activeWeaponIndex].TryShoot();
        }
    }

    private void SwitchWeapon(int index)
    {
        // Validar índice
        if (index < 0 || index >= weaponSlots.Length || index == activeWeaponIndex)
            return;

        // Slot vacío → warning y salir
        if (weaponSlots[index] == null)
        {
            Debug.LogWarning($"PlayerWeaponManager: No hay arma en slot {index}.");
            return;
        }

        // Desactivar arma actual
        if (activeWeaponIndex >= 0 && weaponSlots[activeWeaponIndex] != null)
            weaponSlots[activeWeaponIndex].gameObject.SetActive(false);

        // Activar nueva arma
        weaponSlots[index].gameObject.SetActive(true);
        activeWeaponIndex = index;
    }

    private void AddWeapon(WeaponController prefab)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null)
            {
                var clone = Instantiate(prefab, weaponParentSocket);
                clone.owner = gameObject;
                clone.gameObject.SetActive(false);
                weaponSlots[i] = clone;
                return;
            }
        }
    }
}
