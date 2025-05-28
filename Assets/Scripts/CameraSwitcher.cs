using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera targetCamera; // Cámara a activar al pasar
    public CinemachineVirtualCamera otherCamera;  // Cámara a desactivar

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo entró: " + other.name);
        if (other.CompareTag("Player") || other.CompareTag("Car")) 
        {
            Debug.Log("¡Colisión detectada! Cambiando de cámara.");
            targetCamera.Priority = 11;  // Activamos esta
            otherCamera.Priority = 10;   // Desactivamos la otra
        }
    }
}
