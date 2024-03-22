using System.Collections;
using UnityEngine;

public class PatternBoss : enemyAI
{
    [Header("----- Boss Settings -----")]
    [SerializeField] private int specialAttackCooldown;
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
        if (specialAttackProjectile != null && specialAttackExitPoints.Length > 0)
        {
           
            int randomIndex = Random.Range(0, specialAttackExitPoints.Length);
            Transform selectedExitPoint = specialAttackExitPoints[randomIndex];

            Instantiate(specialAttackProjectile, selectedExitPoint.position, selectedExitPoint.rotation);

            yield return new WaitForSeconds(specialAttackFireRate);
        }
    }

    private IEnumerator SpecialAttackCooldown()
    {
        yield return new WaitForSeconds(specialAttackCooldown);
        canUseSpecialAttack = true;
    }
}
