using System.Collections;
using UnityEngine;

public class PatternBoss : enemyAI
{
    [Header("----- Boss Settings -----")]
    [SerializeField] private float specialAttackCooldown;
    [SerializeField] private float specialAttackFireRate;
    [SerializeField] private GameObject specialAttackProjectile;
    [SerializeField] private Transform[] specialAttackExitPoints;
    private bool canUseSpecialAttack = true;

    protected override IEnumerator Shoot()
    {
        if (canUseSpecialAttack)
        {
            canUseSpecialAttack = false;
            StartCoroutine(SpecialAttackCooldown());
            yield return StartCoroutine(SpecialAttack());
        }
        else
        {
            yield return base.Shoot();
        }
    }

    private IEnumerator SpecialAttack()
    {
        if (specialAttackProjectile != null)
        {
            foreach (Transform exitPoint in specialAttackExitPoints)
            {
                Instantiate(specialAttackProjectile, exitPoint.position, exitPoint.rotation);
                yield return new WaitForSeconds(specialAttackFireRate);
            }
        }
    }

    private IEnumerator SpecialAttackCooldown()
    {
        yield return new WaitForSeconds(specialAttackCooldown);
        canUseSpecialAttack = true;
    }
}
