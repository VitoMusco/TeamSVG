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
    [SerializeField] private float isAttacking = 0f;
    [SerializeField] private float timeToSlam;
    [SerializeField] private float timeToShoot;
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
        //if (playerInSightRange && !playerInLaserAttackRange) chase();
        //if (playerInSlamAttackRange && playerInSightRange) attack(2f);
        //else if (playerInLaserAttackRange && playerInSightRange) attack(3f);

        //handleAnimations();
    }

    void handleBehaviour() { 
        
    }

    /*private void chase() {
        if (isAttacking != 0f) return;

        agent.SetDestination(player.position);
        isWalking = true;
        isAttacking = 0f;
    }*/
    /*private void attack(float type) {

        agent.SetDestination(transform.position);
        var playerPosition = player.position;
        playerPosition.y = 0;
        transform.LookAt(playerPosition);

        if (!alreadyAttacked) {
            //ATTACCA!
            isWalking = false;
            isAttacking = type;
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack() {
        alreadyAttacked = false;
    }*/

    /*private void handleAnimations() {
        if (!isWalking)
            action = Mathf.Lerp(action, 0f, 0.25f);
            anim.SetFloat("Blend", action);
        if (isWalking) {
            action = Mathf.Lerp(action, 1f, 0.25f);
            anim.SetFloat("Blend", action);
        }
        else if(isAttacking != 0f) {
            anim.SetFloat("Blend", isAttacking);
            if (isAttacking == 2f && !isSlamming && !isShooting) {
                StartCoroutine(slam());
            }
            if (isAttacking == 3f && !isSlamming && !isShooting)
                StartCoroutine(shoot());
        }
    }*/

    IEnumerator slam() {
        float timeElapsed = 0f;
        anim.SetTrigger("Slam");
        while (timeElapsed < timeToSlam) {
            isSlamming = true;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        anim.ResetTrigger("Slam");
        isSlamming = false;
        isAttacking = 0f;
    }

    IEnumerator shoot() {
        float timeElapsed = 0f;
        anim.SetTrigger("StartShooting");
        while (timeElapsed < timeToShoot)
        {
            isShooting = true;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        timeElapsed = 0f;
        anim.ResetTrigger("StartShooting");
        //Sparo
        anim.SetTrigger("StopShooting");
        anim.ResetTrigger("StartShooting");
        isShooting = false;
        isAttacking = 0f;
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
