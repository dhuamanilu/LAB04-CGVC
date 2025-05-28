using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public Transform weaponMuzzle;
    public GameObject flashEffect;

    [Header("Info")]
    public string weaponName;
    public Sprite icon;

    [Header("Audio")]
    public AudioClip shootSfx;

    [Header("General")]
    public LayerMask hittableLayers;
    public GameObject bulletHolePrefab;

    [Header("Shoot Parameters")]
    public float fireRange = 200f;
    public float recoilForce = 4f;
    public float fireRate = 0.6f;
    public int maxAmmo = 8;

    [Header("Reload Parameters")]
    public float reloadTime = 1.5f;
    public int currentAmmo { get; private set; }

    // Lógica interna
    private float lastTimeShoot = Mathf.NegativeInfinity;
    private AudioSource audioSource;
    [HideInInspector] public GameObject owner { get; set; }

    private void Awake()
    {
        currentAmmo = maxAmmo;

        // Asegura AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            TryShoot();

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Reload());

        // Suavizado de retorno tras recoil
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 5f);
    }

    public bool TryShoot()
    {
        if (Time.time >= lastTimeShoot + fireRate && currentAmmo > 0)
        {
            HandleShoot();
            currentAmmo--;
            return true;
        }
        return false;
    }

    private void HandleShoot()
    {
        // 1) Comprueba muzzle
        if (weaponMuzzle == null)
        {
            Debug.LogError("WeaponController: falta asignar 'weaponMuzzle' en el Inspector.");
            return;
        }

        // 2) Reproduce sonido
        if (shootSfx != null)
            audioSource.PlayOneShot(shootSfx);

        // 3) Instancia flash
        if (flashEffect != null)
        {
            GameObject flashClone = Instantiate(
                flashEffect,
                weaponMuzzle.position,
                Quaternion.LookRotation(weaponMuzzle.forward),
                transform
            );
            Destroy(flashClone, 1f);
        }
        else
        {
            Debug.LogWarning("WeaponController: no se asignó 'flashEffect'.");
        }

        // 4) Recoil
        AddRecoil();

        // 5) Comprueba owner y cámara
        if (owner == null)
        {
            Debug.LogError("WeaponController: 'owner' es null. ¿Olvidaste setearlo al instanciar el arma?");
            return;
        }
        PlayerController pc = owner.GetComponent<PlayerController>();
        if (pc == null || pc.playerCamera == null)
        {
            Debug.LogError("WeaponController: no se encontró PlayerController o playerCamera en el owner.");
            return;
        }

        // 6) Raycast e impactos
        RaycastHit[] hits = Physics.RaycastAll(
            pc.playerCamera.transform.position,
            pc.playerCamera.transform.forward,
            fireRange,
            hittableLayers
        );
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != owner)
            {
                GameObject hole = Instantiate(
                    bulletHolePrefab,
                    hit.point + hit.normal * 0.001f,
                    Quaternion.LookRotation(hit.normal)
                );
                Destroy(hole, 4f);
            }
        }

        // 7) Actualiza tiempo de disparo
        lastTimeShoot = Time.time;
    }

    private void AddRecoil()
    {
        transform.Rotate(-recoilForce, 0f, 0f);
        transform.position -= transform.forward * (recoilForce / 50f);
    }

    private IEnumerator Reload()
    {
        Debug.Log("Recargando...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        Debug.Log("Recargada");
    }
}