using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] int hp;
    [SerializeField] int damage;
    [SerializeField] int turnspeed;
    [SerializeField] float speed;
    [SerializeField] float firerate;
    [SerializeField] int stoppingDistance;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] GameObject eyes;
    [SerializeField] GameObject exitPoint;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject patrolPoint1;
    [SerializeField] GameObject patrolPoint2;
    bool shootcd;
    bool playerInRange;
    bool patrolswap;
    GameObject tempPatrolPoint;
    Color originalColor;
    Vector3 playerDirection;
    Vector3 patrolDestination;

    void Start()
    {
        if (patrolPoint1 != null && patrolPoint2 != null)
        {
            patrolDestination = patrolPoint1.transform.position; 
        }
        originalColor = model.material.color;
        agent.speed = speed;
    }
    void Update()
    {
        if (CheckForPlayer())
            MoveNShoot();
        else if(patrolPoint1 != null && patrolPoint2 != null)
            Patrol();
    }
    void MoveNShoot()
    {
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (!shootcd)
            StartCoroutine(Shoot());
        if (agent.remainingDistance <= agent.stoppingDistance)
            SpinLikeAnIdiotUntilYoureFacingThePlayer();
    }
    void Patrol()
    {
        agent.stoppingDistance = 0;
        if (new Vector3(transform.position.x, 0, transform.position.z) !=  new Vector3(patrolDestination.x, 0, patrolDestination.z))
            agent.SetDestination(patrolDestination);
        else
        {
            patrolswap = !patrolswap;
            if (patrolswap)
                patrolDestination = patrolPoint1.transform.position;
            else 
                patrolDestination = patrolPoint2.transform.position;
        }
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
            playerDirection = gameManager.instance.player.transform.position - eyes.transform.position;
            RaycastHit playerRay;
            if (Physics.Raycast(eyes.transform.position, playerDirection, out playerRay))
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, turn, Time.deltaTime * turnspeed);
    }
    public void takeDamage(int amount)
    {
        hp -= amount;
        StartCoroutine(DamageFlash());
        if (hp <= 0)
            Destroy(gameObject);
    }
    IEnumerator Shoot()
    {
        shootcd = true;
        Instantiate(projectile, exitPoint.transform.position, exitPoint.transform.rotation);
        yield return new WaitForSeconds(firerate);
        shootcd = false;
    }
    IEnumerator DamageFlash()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.15f);
        model.material.color = originalColor;
    }
}
