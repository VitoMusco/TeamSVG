using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardianController : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;
    public Transform playerPrediction;
    public LayerMask whatIsGround, whatIsPlayer;
    public Transform shootSource;
    public LineRenderer shootBeam;
    public ParticleSystem laserParticles; 
    public ParticleSystem chargeParticles;
    public ParticleSystem slamParticles;
    public GameObject slamCollider;

    private Animator anim;
    private float health = 600;

    [SerializeField] private float action = 0f;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isAlive = true;
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
    public float timeAfterShooting, timeAfterSlamming;
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
        chargeParticles.Stop();
        slamParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInLaserAttackRange = Physics.CheckSphere(transform.position, laserAttackRange, whatIsPlayer);
            playerInSlamAttackRange = Physics.CheckSphere(transform.position, slamAttackRange, whatIsPlayer);

            handleBehaviour();
            handleAnimations();
        }       
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

            float timeElapsed = 0f;
            anim.SetTrigger("Slam");
            while (timeElapsed < timeToSlam)
            {
                isSlamming = true;
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            Instantiate(slamCollider, transform.position + new Vector3(0f, 1f, 0f), transform.rotation);
            slamParticles.Play();
            anim.ResetTrigger("Slam");
            isSlamming = false;
            hasSlammed = true;
            canAim = true;
            Invoke(nameof(ResetAttack), timeAfterSlamming);
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
            chargeParticles.Play();
            while (timeElapsed < timeToShoot)
            {
                isShooting = true;
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            StartCoroutine(shoot());
            hasShot = true;
            canAim = true;
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
            {
                shootBeam.SetPosition(1, hit.point);
                if (hit.collider.tag == "Player")
                    hit.collider.GetComponent<PlayerMovement>().takeDamage(8f);
            }             
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
        Invoke(nameof(ResetAttack), timeAfterShooting);
    }

    IEnumerator aim() {
        float timeElapsed = 0f;
        Quaternion whereToLook = new Quaternion();
        Quaternion startRotation = transform.rotation;
        Vector3 lookPos;
        Vector3 predictedLocation = playerPrediction.position;
        while (timeElapsed < timeToAim) {         
            lookPos = predictedLocation - transform.position;
            //if (lookPos.x < 3 || lookPos.z < 3)
            lookPos.y = 0;
            whereToLook = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(startRotation, whereToLook, timeElapsed / timeToAim);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.rotation = whereToLook;
    }

    public void checkHealth()
    {
        if (health <= 0)
            isAlive = false;
        else
        {
            print("Salute rimanente: " + health);
        }
    }

    public void takeDamage(float amount)
    {
        if (isAlive)
        {
            print("Ho preso " + amount + " danni");
            health -= amount;
            checkHealth();
        }
        else
        {
            print("sono morto!");
            Update();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, laserAttackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, slamAttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}