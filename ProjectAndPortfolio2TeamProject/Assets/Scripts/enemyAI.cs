using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Enemy Stats -----")]
    [SerializeField] int enemyHP;
    [SerializeField] int enemyDamage;
    [SerializeField] int enemyTurnSpeed;
    [SerializeField] float enemySpeed;
    [SerializeField] float enemyFireRate;
    [SerializeField] int enemyStoppingDistance;

    [Header("----- Model -----")]
    [SerializeField] Renderer enemyModel;
    [SerializeField] GameObject enemyEyes;
    [SerializeField] GameObject enemyExitPoint;

    [Header("----- Agent & Projectile -----")]
    [SerializeField] NavMeshAgent enemyAgent;
    [SerializeField] GameObject enemyProjectile;

    [Header("----- Roam Settings -----")]
    [SerializeField] bool isRoaming;
    [SerializeField] float roamingRange;
    [SerializeField] float roamPauseTime;

    [Header("----- Animate -----")]
    [SerializeField] Animator anim;
    [SerializeField] int animSpeedTrans;

    [Header("----- Key -----")]
    [SerializeField] GameObject keyModel;
    [SerializeField] bool hasKey;

    // Enemy States
    private bool isAggro; // this will make it so they go aggro when shot out of range - john
    private bool isShooting;
    bool isRoamingCoroutineRunning;
    private bool hasAnimator => anim != null;

    // Player Data
    private bool playerInRange;
    private Vector3 playerDirection;

    // Original Color
    private Color enemyOriginalColor;

    void Start()
    {

        enemyOriginalColor = enemyModel.material.color;
        enemyAgent.speed = enemySpeed;

        if (keyModel != null)
        {
            keyModel.SetActive(hasKey);
        }

    }
    void Update()
    {
        float animSpeed = enemyAgent.velocity.normalized.magnitude;

        if (hasAnimator)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));
        }



        if (isRoaming)
        {
            Roam();
        }
        else if (isAggro)
        {
            MoveNShoot();
        }
        else if (CheckForPlayer())
        {
            isAggro = true;
            MoveNShoot();
        }
    }
    void MoveNShoot()
    {
        enemyAgent.stoppingDistance = enemyStoppingDistance;
        enemyAgent.SetDestination(gameManager.instance.player.transform.position);

         if (!isShooting)
    {
        StartCoroutine(Shoot());
    }

        if (enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
        {
            SpinLikeAnIdiotUntilYoureFacingThePlayer();
        }

    }
    void Roam()
    {
        if (isRoaming && !isRoamingCoroutineRunning)
        {
            StartCoroutine(RoamCoroutine());
        }
    }

    IEnumerator RoamCoroutine()
    {
        isRoamingCoroutineRunning = true;

        if (!enemyAgent.hasPath || enemyAgent.remainingDistance < 1.0f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
            randomDirection += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 1))
            {
                enemyAgent.SetDestination(hit.position);
            }
        }

        yield return new WaitForSeconds(roamPauseTime);

        isRoamingCoroutineRunning = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
    bool CheckForPlayer()
    {
        if (playerInRange)
        {
            playerDirection = gameManager.instance.player.transform.position - enemyEyes.transform.position;
            RaycastHit playerRay;
            if (Physics.Raycast(enemyEyes.transform.position, playerDirection, out playerRay))
            {
                if (playerRay.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }
    void SpinLikeAnIdiotUntilYoureFacingThePlayer()
    {
        //turnspeed should be either really high or really low for comedic effect
        Quaternion turn = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.rotation.y, playerDirection.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, turn, Time.deltaTime * enemyTurnSpeed);

    }
    public void takeDamage(int amount)
    {
        if (!isAggro)
        {
            isAggro = true;
        }


        enemyHP -= amount;
        StartCoroutine(DamageFlash());
        if (enemyHP <= 0)
        {
            if (hasKey)
            {
                Instantiate(gameManager.instance.keyPickup, transform.position, transform.rotation);
                if (keyModel != null)
                {
                    keyModel.SetActive(true);
                }
            }
            Destroy(gameObject);
        }
    }
    public void pushInDirection(Vector3 dir)
    {
        enemyAgent.velocity += dir / 2;
    }
    IEnumerator Shoot()
    {
        isShooting = true;
        if (hasAnimator)
        {
            anim.SetTrigger("Shoot");
        }
        Instantiate(enemyProjectile, enemyExitPoint.transform.position, enemyExitPoint.transform.rotation);
        yield return new WaitForSeconds(enemyFireRate);
        isShooting = false;
    }
    IEnumerator DamageFlash()
    {
        enemyModel.material.color = Color.red;
        yield return new WaitForSeconds(.15f);
        enemyModel.material.color = enemyOriginalColor;
    }
}
