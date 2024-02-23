using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpointGate : MonoBehaviour
{
    [Header("Collider & Renderer")]
    [SerializeField] BoxCollider gateCollider;
    [SerializeField] MeshRenderer gateRenderer;

    // Door State
    private bool isGateOpen = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isGateOpen && gameManager.instance.playerScript.getKeyCount() >= 1)
        {
            gateCollider.enabled = false;
            gateRenderer.enabled = false;
            isGateOpen = true;
            gameManager.instance.playerScript.useKey(1);
        }
    }
}
