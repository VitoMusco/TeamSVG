using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardianController : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    private Animator anim;

    [SerializeField] private float action = 0f;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private float timeToSlam;
    [SerializeField] private float timeToShoot;
    [SerializeField] private bool hasShot = false;
    [SerializeField] private bool hasSlammed = false;
    [SerializeField] private bool isShooting = false;
    [SerializeField] private bool isSlamming = false;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, laserAttackRange, slamAttackRange;
    public bool playerInSightRange, playerInLaserAttackRange, playerInSlamAttackRange;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInLaserAttackRange = Physics.CheckSphere(transform.position, laserAttackRange, whatIsPlayer);
        playerInSlamAttackRange = Physics.CheckSphere(transform.position, slamAttackRange, whatIsPlayer);

        handleBehaviour();
        handleAnimations();
    }

    void handleBehaviour() {
        if (!hasShot) {
            if (playerInSightRange && !playerInLaserAttackRange && !isSlamming && !isShooting) chase();
            if (playerInLaserAttackRange && playerInSightRange) {
                if (!isSlamming) {
                    StartCoroutine(shoot());
                }
            }
        }
        if (!hasSlammed && hasShot) {
            if (playerInSightRange && !playerInSlamAttackRange && !isSlamming && !isShooting) chase();
            if (playerInSlamAttackRange && playerInSightRange) {
                if (!isShooting) {
                    StartCoroutine(slam());
                }
            }
        }
        if (hasShot && hasSlammed) {
            hasShot = false;
            hasSlammed = false;
        }
    }

    private void chase() {
        agent.SetDestination(player.position);
        isWalking = true;
    }
    private void attack() {

        agent.SetDestination(transform.position);
        var playerPosition = player.position;
        playerPosition.y = 0;
        transform.LookAt(playerPosition);
    
        if (!alreadyAttacked) {
            isWalking = false;
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack() {
        alreadyAttacked = false;
    }

    private void handleAnimations() {
        if (!isWalking)
            action = Mathf.Lerp(action, 1f, 0.25f);
        if (isWalking)
            action = Mathf.Lerp(action, 2f, 0.25f);
        anim.SetFloat("Blend", action);
    }

    IEnumerator slam() {
        agent.SetDestination(transform.position);
        aim();       

        isWalking = false;
        float timeElapsed = 0f;
        anim.SetTrigger("Slam");
        while (timeElapsed < timeToSlam) {
            isSlamming = true;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //Slam
        anim.ResetTrigger("Slam");
        isSlamming = false;
        hasSlammed = true;
    }

    IEnumerator shoot() {
        agent.SetDestination(transform.position);
        aim();

        isWalking = false;
        float timeElapsed = 0f;
        anim.SetTrigger("StartShooting");
        while (timeElapsed < timeToShoot)
        {
            isShooting = true;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isShooting = false;
        anim.ResetTrigger("StartShooting");
        //Sparo
        anim.SetTrigger("StopShooting");
        hasShot = true;
    }

    void aim() {
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, laserAttackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, slamAttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
