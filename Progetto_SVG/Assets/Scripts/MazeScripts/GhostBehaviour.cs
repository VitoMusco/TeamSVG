using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehaviour : MonoBehaviour
{
    public PlayerController player;
    public AudioClip spawn, slap, death;

    private AudioSource soundSource;
    [SerializeField] private float timeBeforeDeath = 15f;
    [SerializeField] private float timeAfterActivation = 0f;
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
        timeBeforeDeath += death.length;
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
                isSpawning = false;
            }
        }
        
        if (isActivated || isDissolving) timeAfterActivation += Time.deltaTime;
        if (isActivated)
        {
            timeAfterLastAttack += Time.deltaTime;
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (playerInAttackRange)
            {
                agent.SetDestination(transform.position);
                isWalking = false;
                if (timeAfterLastAttack >= timeBetweenAttacks) {
                    isAttacking = true;
                    timeAfterLastAttack = 0f;
                    soundSource.clip = slap;
                    soundSource.Play();
                    Invoke(nameof(attack), 1f);
                }
            }
            else {
                agent.SetDestination(player.transform.position);
                isAttacking = false;
                isWalking = true;
            }
            if (timeAfterActivation >= timeBeforeDeath - death.length) {
                soundSource.clip = death;
                soundSource.Play();
                isDissolving = true;
                isActivated = false;
            }
            handleAnimations();
        }

        if (!isActivated && timeAfterActivation >= timeBeforeDeath) {
            kill();
        }
    }

    void handleAnimations() {
        if (isWalking) anim.SetBool("Walking", true);
        else anim.SetBool("Walking", false);
        if (isAttacking) anim.SetBool("Attacking", true);
        else anim.SetBool("Attacking", false);
        if (isDissolving) anim.SetTrigger("Dissolving");
    }

    void attack() {
        if (isActivated) {
            if (!GlobalEvents.FirstGhostSlap) GlobalEvents.FirstGhostSlap = true;
            player.takeDamage(10f);
            isAttacking = false;
        }
    }

    public void kill() {
        if (!GlobalEvents.FirstGhostDeath) GlobalEvents.FirstGhostDeath = true;
        transform.position = new Vector3(0f, 0f, 0f);
        timeAfterSpawning = 0f;
        timeAfterActivation = 0f;
        isDissolving = false;
        isWalking = false;
        isAttacking = false;
        isActivated = false;
        isSpawning = false;
        agent.enabled = false;
        anim.enabled = false;
        gameObject.SetActive(false);
    }

    public void activate() {
        soundSource.clip = spawn;
        soundSource.Play();
        isSpawning = true;
        agent.enabled = true;
        anim.enabled = true;
    }

    public bool isActive() {
        if (isActivated || isSpawning) return true;
        else return false;
    }
}
