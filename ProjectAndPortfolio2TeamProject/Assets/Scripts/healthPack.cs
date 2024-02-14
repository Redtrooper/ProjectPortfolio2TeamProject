using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class healthPack : MonoBehaviour
{
    [SerializeField] int healthGain;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        IHeal heal = other.GetComponent<IHeal>();
        if (heal != null)
        {
            heal.heal(healthGain);
            Destroy(gameObject);
        }
    }
}
