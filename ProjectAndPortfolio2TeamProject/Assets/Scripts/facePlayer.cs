using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class facePlayer : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(gameManager.instance.player.transform);
    }
}
