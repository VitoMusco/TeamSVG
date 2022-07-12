using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehaviour : MonoBehaviour
{
    public PlayerController player;

    private AudioSource soundSource;
    [SerializeField] private float timeBeforeDeath = 15f;
    private float timeAfterActivation = 0f;
    private float timeAfterLastAttack = 0f;
    [SerializeField] private float timeBetweenAttacks = 3f;
    [SerializeField] private float timeAfterSpawning = 0f;
    [SerializeField] private float timeToActivate = 3f;
    private NavMeshAgent agent;
    private Animator anim;
    private bool isActivated = false;
    private bool playerInAttackRange = false;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isDissolving = false;
    [SerializeField] private bool isSpawning = false;
    [SerializeField] private LayerMask whatIsPlayer;

    void Awake() {
        soundSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isActivated && isSpawning) {
            timeAfterSpawning += Time.deltaTime;
            if (timeAfterSpawning >= timeToActivate) {
                isActivated = true;
            }
        }

        if (isActivated)
        {
            timeAfterActivation += Time.deltaTime;
            timeAfterLastAttack += Time.deltaTime;
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (playerInAttackRange)
            {
                agent.SetDestination(transform.position);
                isWalking = false;
                if (timeAfterLastAttack >= timeBetweenAttacks) {
                    isAttacking = true;
                    timeAfterLastAttack = 0f;
                    Invoke(nameof(attack), 1f);
                }
            }
            else {
                agent.SetDestination(player.transform.position);
                isAttacking = false;
                isWalking = true;
            }
            if (timeAfterActivation >= timeBeforeDeath - 0.8f) {
                isDissolving = true;
                isActivated = false;
            }
            handleAnimations();
        }

        if (!isActivated && timeAfterActivation >= timeBeforeDeath) Destroy(gameObject);
    }

    void handleAnimations() {
        if (isWalking) anim.SetBool("Walking", true);
        else anim.SetBool("Walking", false);
        if (isAttacking) anim.SetBool("Attacking", true);
        else anim.SetBool("Attacking", false);
        if (isDissolving) anim.SetTrigger("Dissolving");
    }

    void attack() {
        player.takeDamage(10f);
        isAttacking = false;
    }

    public void activate() {
        soundSource.Play();
        isSpawning = true;
        agent.enabled = true;
        anim.enabled = true;
    }
}
