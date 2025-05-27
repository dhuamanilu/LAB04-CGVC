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

    [Header("Aim Settings")]
    public float aimFOV = 30f;
    public float aimSpeed = 10f;

    [Header("UI")]
    public Image aimReticle;

    [HideInInspector] public int activeWeaponIndex { get; private set; }

    private WeaponController[] weaponSlots = new WeaponController[5];
    private Camera playerCamera;
    private float defaultFOV;
    private bool isAiming = false;

    void Start()
    {
        // Obtiene la cámara principal
        playerCamera = Camera.main;
        if (playerCamera == null)
            Debug.LogError("PlayerWeaponManager: Necesitas una cámara etiquetada como MainCamera.");
        else
            defaultFOV = playerCamera.fieldOfView;

        // Verifica que las referencias estén asignadas
        if (weaponParentSocket == null || defaultWeaponPosition == null || aimingPosition == null)
            Debug.LogError("PlayerWeaponManager: Faltan referencias de sockets en el Inspector.");

        if (aimReticle == null)
            Debug.LogWarning("PlayerWeaponManager: No hay AimReticle asignado. La mira no aparecerá.");

        // Inicializa slots e instancia armas
        activeWeaponIndex = -1;
        foreach (var prefab in startingWeapons)
            AddWeapon(prefab);

        // 3. Auto-equipar la primera arma disponible
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
        // Añade más teclas si quieres más slots
    }

    private void HandleAim()
    {
        // Right-click down/up
        if (Input.GetMouseButtonDown(1)) isAiming = true;
        if (Input.GetMouseButtonUp(1))   isAiming = false;

        // Retícula UI
        if (aimReticle != null)
            aimReticle.enabled = isAiming;

        // Transición suave de posición y rotación
        Transform target = isAiming ? aimingPosition : defaultWeaponPosition;
        weaponParentSocket.position = Vector3.Lerp(
            weaponParentSocket.position,
            target.position,
            Time.deltaTime * aimSpeed
        );
        weaponParentSocket.rotation = Quaternion.Slerp(
            weaponParentSocket.rotation,
            target.rotation,
            Time.deltaTime * aimSpeed
        );

        // Transición suave de FOV
        if (playerCamera != null)
        {
            float targetFOV = isAiming ? aimFOV : defaultFOV;
            playerCamera.fieldOfView = Mathf.MoveTowards(
            playerCamera.fieldOfView,
            targetFOV,
            aimSpeed * Time.deltaTime
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
        // Índice inválido o mismo slot → nada que hacer
        if (index < 0 || index >= weaponSlots.Length || index == activeWeaponIndex)
            return;

        // Slot vacío → avisar y salir
        if (weaponSlots[index] == null)
        {
            Debug.LogWarning($"PlayerWeaponManager: No hay ningún arma en el slot {index}.");
            return;
        }

        // Desactiva el arma anterior
        if (activeWeaponIndex >= 0 && weaponSlots[activeWeaponIndex] != null)
            weaponSlots[activeWeaponIndex].gameObject.SetActive(false);

        // Activa la nueva arma
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
