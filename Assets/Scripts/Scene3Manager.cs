using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene3Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum ControlState { Player, Car }
    public ControlState currentControlState;
    public PlayerController playerController;
    public CarController carController;
    public Camera playerCameraComponent; 
    public Camera carCameraComponent;   
                                                                     
    void Start()
    {
        if (playerController == null || carController == null)
        {
            Debug.LogError("PlayerController o CarController no asignados en Scene3Manager!");
            return;
        }
        SwitchToPlayerControl();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // En Scene3Manager.cs
    public void SwitchToPlayerControl()
    {
        currentControlState = ControlState.Player;

        playerController.enabled = true; // Habilita el componente PlayerControllerv2
        if (playerCameraComponent != null)
        {
            playerCameraComponent.enabled = true; // activate the camera
        }

        carController.enabled = false; // Deshabilita el componente CarController
                                       // Si el carro tiene una cámara, deshabilita su GameObject o el componente Camera.
                                       // Ejemplo: if (carController.GetComponentInChildren<Camera>() != null) carController.GetComponentInChildren<Camera>().gameObject.SetActive(false);
        if (carCameraComponent != null)
        {
            carCameraComponent.enabled = false; // Deactivate the camera
        }
        // Manejo de cámaras Cinemachine (si las usas)
        // if (playerVirtualCamera != null) playerVirtualCamera.Priority = 10;

    }

    public void SwitchToCarControl()
    {
        currentControlState = ControlState.Car;

        playerController.enabled = false; // Deshabilita el componente PlayerControllerv2
        if (playerController.playerCamera != null) playerController.playerCamera.gameObject.SetActive(false);

        carController.enabled = true; // Habilita el componente CarController
                                      // Si el carro tiene una cámara, habilita su GameObject o el componente Camera.
                                      // Ejemplo: if (carController.GetComponentInChildren<Camera>() != null) carController.GetComponentInChildren<Camera>().gameObject.SetActive(true);


    }
}
