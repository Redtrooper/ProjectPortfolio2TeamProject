using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class weaponStats : ScriptableObject
{
    [Header("----- Weapon Projectile -----")]
    public GameObject weaponProjectile;

    [Header("----- Weapon Stats -----")]
    public float weaponFireRate;
    public float weaponReloadTime;
    public int weaponAmmoCurr;
    public int weaponAmmoMax;
    public int weaponKnockback;
    public bool weaponTakesAmmo;

    [Header("----- Weapon Model & Exit Point -----")]
    public GameObject weaponModel;
    public Vector3 weaponExitPointPos;
}
