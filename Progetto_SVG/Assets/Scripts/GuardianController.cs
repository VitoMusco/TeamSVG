using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardianController : MonoBehaviour
{

    public List<AudioClip> startVoiceLines;
    public List<AudioClip> damageVoiceLines;
    public List<AudioClip> laserAttackVoiceLines;
    public List<AudioClip> slamAttackVoiceLines;
    public List<AudioClip> footStepSounds;
    public AudioSource soundSource;
    public AudioSource footStepSource;

    public NavMeshAgent agent;
    public Transform player;
    public Transform playerPrediction;
    public Transform playerSeeker;
    public LayerMask whatIsGround, whatIsPlayer;
    public Transform shootSource;
    public LineRenderer shootBeam;
    public ParticleSystem laserParticles; 
    public ParticleSystem chargeParticles;
    public ParticleSystem slamParticles;
    public GameObject slamCollider;
    public LayerMask rayCastLayer;

    private Animator anim;
    private RaycastHit hit;

    [SerializeField] private bool hasPlayedStartVoiceLine = false;
    [SerializeField] private float health = 600;
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
    [SerializeField] private float timeBetweenShots = 0.1f;
    [SerializeField] private float timeBetweenFootSteps = 0.71f;
    [SerializeField] private float timeBetweenDamageVoiceLines = 10f;
    [SerializeField] private float timeSinceLastDamageVoiceLines = 10f;
    [SerializeField] private float timeBetweenLaserAttackVoiceLines = 10f;
    [SerializeField] private float timeSinceLastLaserAttackVoiceLines = 10f;
    [SerializeField] private float timeBetweenSlamAttackVoiceLines = 10f;
    [SerializeField] private float timeSinceLastSlamAttackVoiceLines = 10f;

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
            handlePlayerSeeker();
            handleBehaviour();
            handleFootSteps();
            handleAnimations();
            handleVoiceLines();
        }       
    }

    void handleVoiceLines() {
        if (timeSinceLastDamageVoiceLines < timeBetweenDamageVoiceLines) {
            if (timeSinceLastDamageVoiceLines + Time.deltaTime > timeBetweenDamageVoiceLines)
                timeSinceLastDamageVoiceLines = timeBetweenDamageVoiceLines;
            else timeSinceLastDamageVoiceLines += Time.deltaTime;
        }
        if (timeSinceLastLaserAttackVoiceLines < timeBetweenLaserAttackVoiceLines)
        {
            if (timeSinceLastLaserAttackVoiceLines + Time.deltaTime > timeBetweenLaserAttackVoiceLines)
                timeSinceLastLaserAttackVoiceLines = timeBetweenLaserAttackVoiceLines;
            else timeSinceLastLaserAttackVoiceLines += Time.deltaTime;
        }
        if (timeSinceLastSlamAttackVoiceLines < timeBetweenSlamAttackVoiceLines)
        {
            if (timeSinceLastSlamAttackVoiceLines + Time.deltaTime > timeBetweenSlamAttackVoiceLines)
                timeSinceLastSlamAttackVoiceLines = timeBetweenSlamAttackVoiceLines;
            else timeSinceLastSlamAttackVoiceLines += Time.deltaTime;
        }
    }

    void handlePlayerSeeker() {
        playerSeeker.transform.LookAt(player.transform.position);
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        if (Physics.Raycast(playerSeeker.transform.position, playerSeeker.transform.forward, out hit, 42f, rayCastLayer)) {
            if (hit.collider.tag == "Player")
                playerInLaserAttackRange = Physics.CheckSphere(transform.position, laserAttackRange, whatIsPlayer);
            else playerInLaserAttackRange = false;
        }
        playerInSlamAttackRange = Physics.CheckSphere(transform.position, slamAttackRange, whatIsPlayer);
    }

    void handleBehaviour() {
        if (!hasSlammed && !alreadyAttacked)
        {
            if (playerInSightRange && !playerInSlamAttackRange && !isSlamming && !isShooting) {
                chase();
                if (!hasPlayedStartVoiceLine) {
                    soundSource.clip = startVoiceLines[Random.Range(0,startVoiceLines.Count)];
                    soundSource.Play();
                    hasPlayedStartVoiceLine = true;
                }
            };
            if (playerInSlamAttackRange && playerInSightRange)
            {
                if (!isShooting)
                {
                    StartCoroutine(slam());
                }
            }
        }
        if (!hasShot && hasSlammed && !alreadyAttacked) {
            if (playerInSightRange && !playerInLaserAttackRange && !isSlamming && !isShooting ) chase();
            if (playerInLaserAttackRange && playerInSightRange) {
                if (!isSlamming) {
                    StartCoroutine(startShooting());
                }
            }
        }

        if (hasShot && hasSlammed) {
            hasShot = false;
            hasSlammed = false;
        }
    }

    void handleFootSteps()
    {
        if (!isWalking) return;

        timeBetweenFootSteps -= Time.deltaTime;
        if (timeBetweenFootSteps <= 0f)
        {
            footStepSource.clip = footStepSounds[Random.Range(0, footStepSounds.Count)];
            footStepSource.Play();
            timeBetweenFootSteps = 0.71f;
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
            if (!soundSource.isPlaying)
            {
                soundSource.clip = slamAttackVoiceLines[Random.Range(0, slamAttackVoiceLines.Count)];
                soundSource.Play();
            }
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
            if (!soundSource.isPlaying) {
                soundSource.clip = laserAttackVoiceLines[Random.Range(0, laserAttackVoiceLines.Count)];
                soundSource.Play();
            }
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
        float timeElapsed = 0f;
        float timeAfterLastShot = 0f;
        shootBeam.enabled = true;
        laserParticles.Play();
        while (timeElapsed < timeSpentShooting) {
            timeElapsed += Time.deltaTime;
            timeAfterLastShot += Time.deltaTime;
            shootBeam.SetPosition(0, shootSource.position);
            if (Physics.Raycast(shootSource.transform.position, shootSource.transform.forward, out hit, 42f, rayCastLayer))
            {
                shootBeam.SetPosition(1, hit.point);
                if (timeAfterLastShot > timeBetweenShots) {
                    if (hit.collider.tag == "Player")
                        hit.collider.GetComponent<PlayerMovement>().takeDamage(8f);
                    timeAfterLastShot = 0f;
                }
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
            if (!soundSource.isPlaying && timeSinceLastDamageVoiceLines >= timeBetweenDamageVoiceLines) {
                soundSource.clip = damageVoiceLines[Random.Range(0, damageVoiceLines.Count)];
                soundSource.Play();
                timeSinceLastDamageVoiceLines = 0f;
            }
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