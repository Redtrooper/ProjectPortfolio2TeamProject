using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


public class enemyAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Enemy Stats -----")]
    [SerializeField] protected int enemyHP;
    [SerializeField] protected int enemyTurnSpeed;
    [SerializeField] protected float enemySpeed;
    [SerializeField] protected float enemyFireRate;
    [SerializeField] protected int enemyStoppingDistance;

    [Header("----- Model -----")]
    [SerializeField] protected List<Renderer> enemyModel;
    [SerializeField] protected GameObject enemyEyes;
    [SerializeField] protected GameObject enemyExitPoint;

    [Header("----- Animate -----")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected int animSpeedTrans;

    [Header("----- Spawn Effect -----")]
    [SerializeField] protected GameObject spawnEffect;
    [SerializeField] protected float spawnDelay = 1.0f;
    [SerializeField] protected Transform spawnFXPosition;

    [Header("----- Agent & Projectile -----")]
    [SerializeField] protected NavMeshAgent enemyAgent;
    [SerializeField] protected GameObject enemyProjectile;
    [SerializeField] protected float bulletSpeed;

    [Header("----- Roaming -----")]
    [SerializeField] protected bool doRoam;
    [SerializeField] protected float maxRoamingDistance;
    private bool isRoaming;

    [Header("----- Key -----")]
    [SerializeField] protected GameObject keyModel;
    [SerializeField] protected bool hasKey;

    [Header("----- Checkpoint -----")]
    [SerializeField] protected GameObject checkPointToInstantiate;
    [SerializeField] protected bool instantiateCheckpoint = false;

    // Enemy States
    protected bool isAggro;
    protected bool isShooting;
    private Vector3 originalPosition;
    public bool isDead = false;
    protected bool isDying = false;
    protected bool isSpawning = true;
    protected bool hasSpawnEffectOccurred = false;
    protected List<Color> enemyOriginalColors;
    // Player Data
    protected bool playerInRange;
    protected Vector3 playerDirection;

    // Original Color
    private Color enemyOriginalColor;

    protected virtual void Start()
    {
        if (spawnEffect != null)
        {
            StartCoroutine(SpawnSequence());
        }
        else
        {
            InitializeEnemy();
        }
    }
    protected virtual IEnumerator SpawnSequence()
    {
        yield return StartCoroutine(SpawnEnemy());
    }

    protected virtual void InitializeEnemy()
    {
        enemyOriginalColors = new List<Color>();

        foreach (Renderer renderer in enemyModel)
        {
            enemyOriginalColors.Add(renderer.material.color);
        }

        enemyAgent.speed = enemySpeed;

        if (keyModel != null)
        {
            keyModel.SetActive(hasKey);
        }

        gameManager.instance.enemyReportAlive(this.transform);
        originalPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (!isSpawning)
        {
            if (isAggro)
            {
                EngageTarget();
                UpdateAnimationSpeed();
                isAggro = false;
            }
            else
            {
                if (DetectPlayer())
                {
                    EngageTarget();
                }
                else if (doRoam && !isRoaming)
                {
                    StartCoroutine(Roam());
                }
            }
        }
        else
        {
            // Enemy is still spawning, disable model and functionality
            foreach (Renderer renderer in enemyModel)
            {
                renderer.enabled = false;
            }
            enemyAgent.enabled = false;
        }
    }

    protected virtual IEnumerator SpawnEnemy()
    {
        foreach (Renderer renderer in enemyModel)
        {
            renderer.enabled = false;
        }

        if (spawnEffect != null && !hasSpawnEffectOccurred)
        {
            if (spawnFXPosition != null)
            {
                GameObject effectInstance = Instantiate(spawnEffect, spawnFXPosition.position, Quaternion.identity);
                effectInstance.transform.up = spawnFXPosition.up;
                hasSpawnEffectOccurred = true;
            }
            else
            {
                Debug.LogError("Spawn FX position not assigned for spawn effect!");
            }
        }

        gameManager.instance.enemyReportAlive(this.transform);

        yield return new WaitForSeconds(spawnDelay);

        foreach (Renderer renderer in enemyModel)
        {
            renderer.enabled = true;
        }

        enemyAgent.enabled = true;

        isSpawning = false;
    }




    protected virtual void OnSpawnEffectFinished()
    {
        Debug.Log("Spawn effect finished playing.");
    }

    protected virtual void SpawnEffect()
    {
        if (spawnEffect != null)
        {
            Instantiate(spawnEffect, transform.position, Quaternion.identity);
        }
    }


    protected virtual void UpdateAnimationSpeed()
    {
        if (enemyAgent != null && anim != null)
        {
            float animSpeed = enemyAgent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));
        }
    }

    protected virtual void RotateTowardsPlayer()
    {
        if (gameManager.instance.player != null && !isDying)
        {
            Vector3 playerDirection = gameManager.instance.player.transform.position - transform.position;
            playerDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(playerDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * enemyTurnSpeed);
        }
    }

    protected virtual void EngageTarget()
    {
        if (gameManager.instance.player != null && !isDying)
        {
            enemyAgent.stoppingDistance = enemyStoppingDistance;
            float animSpeed = enemyAgent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));

            enemyAgent.SetDestination(gameManager.instance.player.transform.position);
            StopCoroutine(Roam());

            if (!isShooting)
            {
                if (DetectPlayer())
                {
                    StartCoroutine(Shoot());
                }
            }

            if (enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
            {
                RotateTowardsPlayer();
            }
        }
    }

    protected virtual IEnumerator Roam()
    {
        enemyAgent.stoppingDistance = 0;
        if (enemyAgent.remainingDistance < 0.05f && !isDying)
        {
            isRoaming = true;
            yield return new WaitForSeconds(1.0f);
            Vector3 randomPosition = GetRandomPosition();
            enemyAgent.SetDestination(randomPosition);
            isRoaming = false;
        }
    }

    protected virtual Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-maxRoamingDistance, maxRoamingDistance);
        float randomZ = Random.Range(-maxRoamingDistance, maxRoamingDistance);
        Vector3 randomPosition = originalPosition + new Vector3(randomX, 0f, randomZ);
        return randomPosition;
    }

    protected virtual IEnumerator WaitBeforeNextRoam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(5.0f);
        isRoaming = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StartCoroutine(WaitBeforeNextRoam());
        }
    }

    protected virtual bool DetectPlayer()
    {
        if (playerInRange && !isDying)
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

    protected virtual void RotateTowards()
    {
        Quaternion turn = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, turn, Time.deltaTime * enemyTurnSpeed);
    }

    //-------------------------------------------------
    public virtual bool IsDead
    {
        get
        {
            return enemyHP <= 0;
        }
    }

    public virtual void takeDamage(int amount)
    {
        if (!isAggro)
        {
            isAggro = true;
        }

        enemyHP -= amount;
        StartCoroutine(DamageFlash());
        if (enemyHP <= 0)
        {
            if (!isDying)
            {
                isDying = true;
                if (hasKey)
                {
                    Instantiate(gameManager.instance.keyPickup, transform.position, transform.rotation);
                    if (keyModel != null)
                    {
                        keyModel.SetActive(true);
                    }
                }
                if (instantiateCheckpoint)
                    Instantiate(checkPointToInstantiate, transform.position, new Quaternion(0, 0, 0, 0));
                gameManager.instance.enemyReportDead(this.transform);
                if (IsDead)
                {
                    if (anim)
                        anim.SetTrigger("Death");
                    StartCoroutine(DestroyAfterAnimation());
                }
            }
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    //-------------------------------------------------
    public void pushInDirection(Vector3 dir)
    {
        enemyAgent.velocity += dir / 2;
    }

    protected virtual IEnumerator Shoot()
    {
        if (!isShooting && !isDying)
        {
            isShooting = true;

            if (anim != null)
            {
                anim.SetTrigger("Shoot");
            }

            yield return new WaitForSeconds(enemyFireRate);

            isShooting = false;
        }
    }

    protected virtual void FireProjectile()
    {
        if (enemyProjectile != null && !isDying)
        {
            GameObject player = gameManager.instance.player;
            if (player != null)
            {
                Collider playerCollider = player.GetComponent<Collider>();
                if (playerCollider != null)
                {
                    Vector3 playerColliderPosition = playerCollider.bounds.center;
                    Vector3 directionToPlayer = playerColliderPosition - enemyExitPoint.transform.position;
                    directionToPlayer.Normalize();

                    GameObject bullet = Instantiate(enemyProjectile, enemyExitPoint.transform.position, Quaternion.LookRotation(directionToPlayer));
                    Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();

                    if (bulletRB != null)
                    {
                        bulletRB.velocity = directionToPlayer * bulletSpeed;
                    }
                }
            }
        }
    }


    protected virtual IEnumerator DamageFlash()
    {
        if (!isDying)
        {
           
            List<Color> originalColors = new List<Color>();
            foreach (Renderer renderer in enemyModel)
            {
                originalColors.Add(renderer.material.color);
                renderer.material.color = Color.red; 
            }

            yield return new WaitForSeconds(.15f);

          
            for (int i = 0; i < enemyModel.Count; i++)
            {
                enemyModel[i].material.color = originalColors[i];
            }
        }
    }


}