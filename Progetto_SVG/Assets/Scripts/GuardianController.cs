using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardianController : MonoBehaviour
{
    public EventHandler eventHandler;

    public List<AudioClip> startVoiceLines;
    public List<AudioClip> damageVoiceLinesA;
    public List<AudioClip> damageVoiceLinesB;
    private bool lastDamageListToggle = false;
    public List<AudioClip> laserAttackVoiceLinesA;
    public List<AudioClip> laserAttackVoiceLinesB;
    private bool lastLaserAttackListToggle = false;
    public List<AudioClip> slamAttackVoiceLinesA;
    public List<AudioClip> slamAttackVoiceLinesB;
    private bool lastSlamAttackListToggle = false;
    public List<AudioClip> footStepSounds;
    public AudioSource soundSource;
    public AudioSource attackSoundSource;
    public AudioClip laserSound;
    public AudioClip laserChargeSound;
    public AudioClip slamSound;
    public AudioClip deathSound;
    public AudioSource footStepSource;
    public SkinnedMeshRenderer meshRenderer;

    public CameraLook playerCamera;
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
    public ParticleSystem deathParticles;
    public GameObject slamCollider;
    public LayerMask rayCastLayer;

    private Material[] materials;
    private Animator anim;
    private RaycastHit hit;
    private string lastLaserClipPlayed;
    private string lastSlamClipPlayed;
    private string lastDamageClipPlayed;
    private BoxCollider collisions;

    [SerializeField] private bool hasPlayedStartVoiceLine = false;
    [SerializeField] private float health = 600;
    [SerializeField] private float action = 0f;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool isActivated = false;
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
    [SerializeField] private float timeBetweenAttackVoiceLines = 10f;
    [SerializeField] private float timeSinceLastAttackVoiceLine = 10f;
    [SerializeField] private float timeToDie = 10f;

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
        collisions = GetComponent<BoxCollider>();
        materials = meshRenderer.materials;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        shootBeam = GetComponentInChildren<LineRenderer>();
        shootBeam.enabled = false;
        laserParticles.Stop();
        chargeParticles.Stop();
        slamParticles.Stop();
        deathParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive && isActivated)
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
        if (timeSinceLastAttackVoiceLine < timeBetweenAttackVoiceLines)
        {
            if (timeSinceLastAttackVoiceLine + Time.deltaTime > timeBetweenAttackVoiceLines)
                timeSinceLastAttackVoiceLine = timeBetweenAttackVoiceLines;
            else timeSinceLastAttackVoiceLine += Time.deltaTime;
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
                    speak();
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
            StartCoroutine(playerCamera.shakeCamera(.2f, .25f, .25f, .25f, 0.017f));
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
            playSlamVoiceLine();
            float timeElapsed = 0f;
            anim.SetTrigger("Slam");
            while (timeElapsed < timeToSlam && isAlive)
            {
                isSlamming = true;
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            Instantiate(slamCollider, transform.position + new Vector3(0f, 1f, 0f), transform.rotation);
            slamParticles.Play();
            attackSoundSource.clip = slamSound;
            attackSoundSource.Play();
            StartCoroutine(playerCamera.shakeCamera(2f, .5f, 1f, .25f, 0.017f));
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
            playLaserVoiceLine();
            anim.SetTrigger("StartShooting");
            if (canAim)
                StartCoroutine(aim());
            canAim = false;
            attackSoundSource.clip = laserChargeSound;
            chargeParticles.Play();
            while (timeElapsed < timeToShoot && isAlive)
            {
                if(timeElapsed >= 1.8f && !attackSoundSource.isPlaying)
                    attackSoundSource.Play();
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
        attackSoundSource.clip = laserSound;
        attackSoundSource.Play();
        while (timeElapsed < timeSpentShooting && isAlive) {
            timeElapsed += Time.deltaTime;
            timeAfterLastShot += Time.deltaTime;
            shootBeam.SetPosition(0, shootSource.position);

            if(timeElapsed > timeSpentShooting - timeSpentShooting / 4)
                attackSoundSource.volume = Mathf.Lerp(1f, 0f, timeElapsed / timeSpentShooting);

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
        attackSoundSource.Stop();
        attackSoundSource.volume = 1f;
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

    void playSlamVoiceLine() {
        if (!soundSource.isPlaying && timeSinceLastAttackVoiceLine >= timeBetweenAttackVoiceLines)
        {
            if (slamAttackVoiceLinesA.Count > 1 && !lastSlamAttackListToggle)
            {
                soundSource.clip = slamAttackVoiceLinesA[Random.Range(0, slamAttackVoiceLinesA.Count)];
                slamAttackVoiceLinesB.Add(soundSource.clip);
                slamAttackVoiceLinesA.Remove(soundSource.clip);
            }
            else if (slamAttackVoiceLinesA.Count == 1 && !lastSlamAttackListToggle)
            {
                soundSource.clip = slamAttackVoiceLinesA[0];
                lastSlamAttackListToggle = true;
            }
            else if (slamAttackVoiceLinesB.Count > 1 && lastSlamAttackListToggle)
            {
                soundSource.clip = slamAttackVoiceLinesB[Random.Range(0, slamAttackVoiceLinesB.Count)];
                slamAttackVoiceLinesA.Add(soundSource.clip);
                slamAttackVoiceLinesB.Remove(soundSource.clip);
            }
            else if (slamAttackVoiceLinesB.Count == 1 && lastSlamAttackListToggle)
            {
                soundSource.clip = slamAttackVoiceLinesB[0];
                lastSlamAttackListToggle = false;
            }
            speak();
            timeSinceLastAttackVoiceLine = 0f;
        }
    }

    void playLaserVoiceLine()
    {
        if (!soundSource.isPlaying && timeSinceLastAttackVoiceLine >= timeBetweenAttackVoiceLines)
        {
            if (laserAttackVoiceLinesA.Count > 1 && !lastLaserAttackListToggle)
            {
                soundSource.clip = laserAttackVoiceLinesA[Random.Range(0, laserAttackVoiceLinesA.Count)];
                laserAttackVoiceLinesB.Add(soundSource.clip);
                laserAttackVoiceLinesA.Remove(soundSource.clip);
            }
            else if (laserAttackVoiceLinesA.Count == 1 && !lastLaserAttackListToggle)
            {
                soundSource.clip = laserAttackVoiceLinesA[0];
                lastLaserAttackListToggle = true;
            }
            else if (laserAttackVoiceLinesB.Count > 1 && lastLaserAttackListToggle)
            {
                soundSource.clip = laserAttackVoiceLinesB[Random.Range(0, laserAttackVoiceLinesB.Count)];
                laserAttackVoiceLinesA.Add(soundSource.clip);
                laserAttackVoiceLinesB.Remove(soundSource.clip);
            }
            else if (laserAttackVoiceLinesB.Count == 1 && lastLaserAttackListToggle)
            {
                soundSource.clip = laserAttackVoiceLinesB[0];
                lastLaserAttackListToggle = false;
            }
            speak();
            timeSinceLastAttackVoiceLine = 0f;
        }
    }

    void playDamageVoiceLine()
    {
        if (!soundSource.isPlaying && timeSinceLastDamageVoiceLines >= timeBetweenDamageVoiceLines && !isShooting && !isSlamming)
        {
            if (damageVoiceLinesA.Count > 1 && !lastDamageListToggle)
            {
                soundSource.clip = damageVoiceLinesA[Random.Range(0, damageVoiceLinesA.Count)];
                damageVoiceLinesB.Add(soundSource.clip);
                damageVoiceLinesA.Remove(soundSource.clip);
            }
            else if (damageVoiceLinesA.Count == 1 && !lastDamageListToggle)
            {
                soundSource.clip = damageVoiceLinesA[0];
                lastDamageListToggle = true;
            }
            else if (damageVoiceLinesB.Count > 1 && lastDamageListToggle)
            {
                soundSource.clip = damageVoiceLinesB[Random.Range(0, damageVoiceLinesB.Count)];
                damageVoiceLinesA.Add(soundSource.clip);
                damageVoiceLinesB.Remove(soundSource.clip);
            }
            else if (damageVoiceLinesB.Count == 1 && lastDamageListToggle)
            {
                soundSource.clip = damageVoiceLinesB[0];
                lastDamageListToggle = false;
            }
            speak();
            timeSinceLastDamageVoiceLines = 0f;
        }
    }

    void speak() {
        soundSource.Play();
    }

    IEnumerator die() {
        float timeElapsed = 0f;
        float slider = 0f;
        chargeParticles.Stop();
        attackSoundSource.Stop();
        soundSource.Stop();
        soundSource.clip = deathSound;
        soundSource.Play();
        deathParticles.Play();
        collisions.enabled = false;
        while (timeElapsed < timeToDie) { 
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= timeToDie - timeToDie / 4)
                soundSource.volume = Mathf.Lerp(1f, 0f, timeElapsed / timeToDie);
            if (materials.Length > 0) {
                slider = timeElapsed/timeToDie;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_DissolveAmount", slider);
                }
            }
            yield return null;
        }
        eventHandler.setGuardianKilled();
        Destroy(gameObject);
    }

    public void activateGuardian() {
        isActivated = true;
    }

    public void checkHealth()
    {
        if (health <= 0) {
            isAlive = false;
            StartCoroutine(die());
        } 
        else {
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
            playDamageVoiceLine();
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