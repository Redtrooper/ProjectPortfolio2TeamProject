using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Enemy Stats -----")]
    [SerializeField] int enemyHP;
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

    [Header("----- Animate -----")]
    [SerializeField] Animator anim;
    [SerializeField] int animSpeedTrans;

    [Header("----- Roaming -----")]
    [SerializeField] bool doRoam;
    [SerializeField] float maxRoamingDistance;
    private bool isRoaming;


    [Header("----- Key -----")]
    [SerializeField] GameObject keyModel;
    [SerializeField] bool hasKey;

    // Enemy States
    private bool isAggro; // this will make it so they go aggro when shot out of range - john
    private bool isShooting;
    private Vector3 originalPosition;
    private bool hasAnimator => anim != null;

    // Player Data
    private bool playerInRange;
    private Vector3 playerDirection;

    // Patrol


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

        gameManager.instance.enemyReportAlive(this.transform);
        originalPosition = transform.position;

    }
    void Update()
    {
        float animSpeed = enemyAgent.velocity.normalized.magnitude;

        if (hasAnimator)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));
        }

        if (isAggro)
        {
            MoveNShoot();
            isAggro = false;
        }
        else
        {

            if (CheckForPlayer())
            {
                MoveNShoot();
            }
            else if(doRoam && !isRoaming)
            {
                StartCoroutine(Roam());
            }
        }
    }
    void MoveNShoot()
    {
        enemyAgent.stoppingDistance = enemyStoppingDistance;
        enemyAgent.SetDestination(gameManager.instance.player.transform.position);
        StopCoroutine(Roam());

        if (!isShooting)
        {
            if (hasAnimator)
            {
                anim.SetTrigger("Shoot");
            }

            if (CheckForPlayer())
            {
                StartCoroutine(Shoot()); 
            }
        }

        if (enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
            SpinLikeAnIdiotUntilYoureFacingThePlayer();
    }

    IEnumerator Roam()
    {
        enemyAgent.stoppingDistance = 0;
        if (enemyAgent.remainingDistance < 0.05f)
        {
            isRoaming = true;
            yield return new WaitForSeconds(1.0f); // this is how long they will stay at their destination b4 going to a new one
            Vector3 randomPosition = GetRandomPosition();
            enemyAgent.SetDestination(randomPosition);
            isRoaming = false; 
        }
    }

    Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-maxRoamingDistance, maxRoamingDistance);
        float randomZ = Random.Range(-maxRoamingDistance, maxRoamingDistance);
        Vector3 randomPosition = originalPosition + new Vector3(randomX, 0f, randomZ);
        return randomPosition;
    }

    IEnumerator WaitBeforeNextRoam()
    {
        Debug.Log("Waiting");
        isRoaming = true;
        yield return new WaitForSeconds(5.0f);
        isRoaming = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            playerInRange = false;
            StartCoroutine(WaitBeforeNextRoam());
        }
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
        Quaternion turn = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));
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
            gameManager.instance.enemyReportDead(this.transform);
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

        yield return new WaitForSeconds(enemyFireRate);


        if (hasAnimator)
        {
            anim.ResetTrigger("Shoot");
        }
        else
        {
            createBullet();
        }

        isShooting = false;
    }

    public void createBullet()
    {
        Instantiate(enemyProjectile, enemyExitPoint.transform.position, enemyExitPoint.transform.rotation); 
    }

    IEnumerator DamageFlash()
    {
        enemyModel.material.color = Color.red;
        yield return new WaitForSeconds(.15f);
        enemyModel.material.color = enemyOriginalColor;
    }
}