using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpointGate : MonoBehaviour
{
    [SerializeField] BoxCollider doorCollider;
    [SerializeField] MeshRenderer doorRenderer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerScript.getKeyCount() >= 1)
        {
            doorCollider.enabled = false;
            doorRenderer.enabled = false;
            gameManager.instance.playerScript.useKey(1);
        }
    }
}
