using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class weaponStats : ScriptableObject
{
    public GameObject weaponProjectile;
    public float weaponFireRate;
    public float weaponReloadTime;
    public int weaponAmmoCurr;
    public int weaponAmmoMax;

    public GameObject weaponModel;
}
