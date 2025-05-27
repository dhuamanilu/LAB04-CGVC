using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene3Manager : MonoBehaviour
{
    // Start is called before the first frame update
    /*public enum ControlState { Player, Car }
    public ControlState currentControlState;
    public PlayerController playerController;
    public CarController carController;
    public Cinemachine.CinemachineVirtualCamera playerVirtualCamera; // Asigna tu VCam del jugador aquí
    public Cinemachine.CinemachineVirtualCamera carVirtualCamera;   // Asigna tu VCam del coche aquí
                                                                    // Puedes añadir más cámaras si es necesario (ej. interior del coche)
    void Start()
    {
        SwitchToPlayerControl();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SwitchToPlayerControl()
    {
        currentControlState = ControlState.Player;

        playerController.enabled = true;
        carController.enabled = false; // Desactiva el script del coche

        // Activar cámara del jugador y desactivar la del coche
        // En Cinemachine, esto se hace cambiando la prioridad o activando/desactivando GameObjects de VCam
        playerVirtualCamera.Priority = 10;
        carVirtualCamera.Priority = 5; // Menor prioridad para la cámara inactiva

       
        playerController.gameObject.SetActive(true);
        // carController.gameObject.SetActive(false); // ¡Cuidado con esto si el coche debe seguir existiendo!
        
    }

    public void SwitchToCarControl()
    {
        currentControlState = ControlState.Car;

        playerController.enabled = false; // Desactiva el script del jugador
        carController.enabled = true;

        // Activar cámara del coche y desactivar la del jugador
        carVirtualCamera.Priority = 10;
        playerVirtualCamera.Priority = 5;

        // Podrías mover/desactivar al personaje para que "entre" al coche
        // playerController.gameObject.SetActive(false);
    }*/
}
