using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class itemStats : ScriptableObject
{
    [Header("----- Player Stat Modifiers -----")]
    public float healthMultiplier = 1;
    public float speedMultiplier = 1;
    public int extraJumps;
    public float jumpForceMultiplier = 1;
    public float healthRegenerationMultiplier = 1;
    public bool lifeSteal;
    public float lifeStealMultiplier = 1;
    public float maxStaminaMultiplier = 1;
    public float staminaRecoveryRateMultiplier = 1;
    public float critChanceMultiplier = 1;


    [Header("----- Weapon Modifiers -----")]
    public float damageMultiplier = 1;
    public float bulletSpeedMultiplier = 1;
    public float fireRateMultiplier = 1;
    public float weaponRecoilMultiplier = 1;
}
