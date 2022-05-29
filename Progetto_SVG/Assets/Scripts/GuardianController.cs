using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardianController : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Transform shootSource;
    public LineRenderer shootBeam;
    public ParticleSystem laserParticles;

    private Animator anim;

    [SerializeField] private float action = 0f;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private float timeToSlam;
    [SerializeField] private float timeToShoot;
    [SerializeField] private bool hasShot = false;
    [SerializeField] private bool hasSlammed = false;
    [SerializeField] private bool isShooting = false;
    [SerializeField] private bool isSlamming = false;
    [SerializeField] private bool canAim = true;
    [SerializeField] private float timeToAim = 1f;
    [SerializeField] private float timeSpentShooting = 3f;

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
        shootBeam = GetComponentInChildren<LineRenderer>();
        shootBeam.enabled = false;
        laserParticles.Stop();
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
        if (!hasShot && !alreadyAttacked) {
            if (playerInSightRange && !playerInLaserAttackRange && !isSlamming && !isShooting) chase();
            if (playerInLaserAttackRange && playerInSightRange) {
                if (!isSlamming) {
                    StartCoroutine(startShooting());
                }
            }
        }
        if (!hasSlammed && hasShot && !alreadyAttacked) {
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
    /*
    private void attack() {

        agent.SetDestination(transform.position);
        var playerPosition = player.position;
        playerPosition.y = 0;
        transform.LookAt(playerPosition);
    
        
    }*/
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
        if (canAim)
            StartCoroutine(aim());
        canAim = false;

        if (!alreadyAttacked)
        {
            isWalking = false;
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);

            float timeElapsed = 0f;
            anim.SetTrigger("Slam");
            while (timeElapsed < timeToSlam)
            {
                isSlamming = true;
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            //Slam
            anim.ResetTrigger("Slam");
            isSlamming = false;
            hasSlammed = true;
            canAim = true;
        }
    }

    IEnumerator startShooting() {
        float timeElapsed = 0f;
        agent.SetDestination(transform.position);
        if (!alreadyAttacked) {
            isWalking = false;
            alreadyAttacked = true;

            anim.SetTrigger("StartShooting");
            if (canAim)
                StartCoroutine(aim());
            canAim = false;
            while (timeElapsed < timeToShoot)
            {
                isShooting = true;
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            StartCoroutine(shoot());
            hasShot = true;
            canAim = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    IEnumerator shoot() {
        RaycastHit hit;       
        float timeElapsed = 0f;
        shootBeam.enabled = true;
        laserParticles.Play();
        while (timeElapsed < timeSpentShooting) {
            timeElapsed += Time.deltaTime;
            shootBeam.SetPosition(0, shootSource.position);
            if (Physics.Raycast(shootSource.transform.position, shootSource.transform.forward, out hit, 42f))
                shootBeam.SetPosition(1, hit.point);
            else {
                shootBeam.SetPosition(1, shootSource.transform.position + shootSource.transform.forward * 100f);
            }
            Debug.DrawRay(shootSource.transform.position, shootSource.transform.forward * hit.distance, Color.green);
            yield return null;
        }
        shootBeam.enabled = false;
        laserParticles.Stop();
        isShooting = false;
        anim.ResetTrigger("StartShooting");
        anim.SetTrigger("StopShooting");
    }

    IEnumerator aim() {
        float timeElapsed = 0f;
        Quaternion whereToLook = new Quaternion();
        while (timeElapsed < timeToAim) {
            whereToLook = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, whereToLook, timeElapsed / timeToAim);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.rotation = whereToLook;
    }

    public void takeDamage(float amount) {
        print("Ho preso " + amount + " danni");
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
