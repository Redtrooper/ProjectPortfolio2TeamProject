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
    public int weaponAmmoMax;
    public int weaponKnockback;
    public bool weaponTakesAmmo;
    public int weaponRecoilDegrees;

    [Header("----- Weapon Model & Exit Point -----")]
    public GameObject weaponModel;
    public Vector3 weaponExitPointPos;

    [Header("----- Weapon Information -----")]
    public Weapon weaponType;

    [Header("----- Weapon Sounds -----")]
    public AudioClip[] weaponReloadSound;
    [Range(0, 1)] public float weaponReloadSoundVol;
    public AudioClip[] weaponShootSound;
    [Range(0, 1)] public float weaponShootSoundVol;

}

public enum Weapon
{
    Rifle,
    Shotgun
}


