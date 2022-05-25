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
    [SerializeField] private bool isAttacking = false;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

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
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange && !playerInAttackRange) Chase();
        if (playerInAttackRange && playerInSightRange) Attack();

        handleAnimations();
    }

    private void Chase() {
        agent.SetDestination(player.position);
        isWalking = true;
        isAttacking = false;
    }
    private void Attack() {
        agent.SetDestination(transform.position);
        var playerPosition = player.position;
        playerPosition.y = 0;
        transform.LookAt(playerPosition);

        if (!alreadyAttacked) {
            //ATTACCA!
            isWalking = false;
            isAttacking = true;
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack() {
        alreadyAttacked = false;
    }

    private void handleAnimations() {
        action = isWalking ? Mathf.Lerp(action, 1f, 0.25f) : isAttacking ? Mathf.Lerp(action, 2f, 0.25f) : Mathf.Lerp(action, 0f, 0.25f);
        anim.SetFloat("Blend", action);
    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
