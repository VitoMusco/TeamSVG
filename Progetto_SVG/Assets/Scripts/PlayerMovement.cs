using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public bool developerMode = false;
    public bool hasRespawned = false;
    public Transform predictedMovement;
    public Transform shootSource;
    public Transform playerSpawnPosition;
    public MeshRenderer shieldRenderer;
    public MeshRenderer decalRenderer;
    public ParticleSystem particleShoot;
    public LayerMask rayCastLayer;
    public AudioSource attackSoundSource;
    public AudioSource shieldSoundSource;
    public AudioSource footStepSource;
    public List<AudioClip> footStepSounds;

    public Image healthBar;
    public Image healthBarEnd;
    public Image staminaBar;
    public Image staminaBarEnd;
    public float healthBarEndPos = 0f;
    public float healthBarEndStartPosition;
    public float staminaBarEndPos = 0f;
    public float staminaBarEndStartPosition;

    private LineRenderer shootBeam;
    private CharacterController controller;
    private Camera playerCamera;
    private Animator anim;

    [SerializeField] private float predictedVelocityMultiplier = 3f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float timeAfterFootSteps = 0f;
    [SerializeField] private float timeBetweenFootSteps;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float action = 0;
    [SerializeField] private float verticalVelocity;

    [SerializeField] Vector3 velocity;
    [SerializeField] private bool wantsToUncrouch = false;
    [SerializeField] private bool canDoubleJump;
    [SerializeField] private bool canLevitate = false;

    private float x;
    private float z;
    private float lockX;
    private float lockZ;  

    private Vector3 lockRight;
    private Vector3 lockForward;
    private float lockSpeed;


    private float runBobSpeed = 14f;
    private float runBobAmount = 0.08f;
    private float walkBobSpeed = 10f;
    private float walkBobAmount = 0.05f;
    private float crouchBobSpeed = 6f;
    private float crouchBobAmount = 0.025f;
    private float defaultYpos = 0;
    private float timer;
    private bool isAlive = true;

    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float magicStamina = 100f;
    [SerializeField] private float maxMagicStamina = 100f;
    [SerializeField] private float timeAfterAnAction = 0f;
    [SerializeField] private float timeToShoot = 0.4f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool isLevitating = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isDefending = false;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackStaminaRemove = 10;
    [SerializeField] private float defenseStaminaAdd = 2;
    [SerializeField] private float runStaminaRemove = 5;
    [SerializeField] private float staminaToRemove = 0f;
    [SerializeField] private float staminaRemovalMultiplier = 1f;

    [SerializeField] private bool hasDoubleJumped = false;
    [SerializeField] private float maxTimeAfterAnAction = 2f;

    void Awake() {
        if (developerMode) {
            health = 1000000;
            magicStamina = 1000000;
        }
        staminaBarEndStartPosition = staminaBarEnd.transform.localPosition.x;
        healthBarEndStartPosition = healthBarEnd.transform.localPosition.x;
        shieldRenderer.enabled = false;
        decalRenderer.enabled = false;
        shootBeam = GetComponentInChildren<LineRenderer>();
        shootBeam.enabled = false;
        shootBeam.widthMultiplier = 0f;
        particleShoot.Stop();
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        defaultYpos = playerCamera.transform.localPosition.y;
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(handleStamina());
    }
    // Update is called once per frame
    void Update() {
        if (isAlive) {
            checkIfGrounded();
            handleInputs();
            handleFootSteps();
            handleMovementPrediction();
            handleAnimations(); 
        }     
    }

    void LateUpdate() {
        if (!isAlive && !hasRespawned) {
            isAlive = true;
            hasRespawned = true;
        }
        else if (isAlive)
            controller.Move(velocity * Time.deltaTime);

    }

    //CONTROLLA SE SI E' A TERRA
    void checkIfGrounded() => isGrounded = controller.isGrounded;

    void checkIfMoving(float x, float z) {
        if (Mathf.Abs(x) > 0.1 || Mathf.Abs(z) > 0.1)
            isMoving = true;
        else
            isMoving = false;

        if (isMoving && speed == 3f && !isCrouching && isGrounded)
            isWalking = true;
        else 
            isWalking = false;

        if (isMoving && speed == 6f && !isCrouching && isGrounded)
            isRunning = true;
        else
            isRunning = false;
    }

    //GESTIONE INPUT
    void handleInputs() {
        //MOVIMENTO
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        verticalVelocity = velocity.y;

        if (isGrounded) {
            velocity = transform.right * x + transform.forward * z;
            velocity = velocity * speed;
        }
        else {
            velocity = lockRight * lockX + lockForward * lockZ;
            velocity = velocity * lockSpeed;
        }

        velocity.y = verticalVelocity;
        checkIfMoving(velocity.x, velocity.z);
        handleHeadBob();

        if (!isCrouching && Input.GetKeyDown("left shift"))
            speed = 6f;
        if (Input.GetKeyUp("left shift") || magicStamina == 0f)
            speed = 3f;

        //GESTIONE CROUCH  
        if (Input.GetKeyDown("left ctrl") && !isCrouching && canCrouch)
        {
            StartCoroutine(handleCrouch());
            isCrouching = true;
        }
        if (Input.GetKeyUp("left ctrl"))
        {
            wantsToUncrouch = true;
        }
        if (Input.GetKeyDown("left ctrl") && isCrouching && wantsToUncrouch)
            wantsToUncrouch = false;
        if (isCrouching && wantsToUncrouch && canCrouch)
        {
            if (!Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            {
                StartCoroutine(handleCrouch());
                isCrouching = false;
                wantsToUncrouch = false;
            }
        }

        //GESTIONE LEVITAZIONE
        if (isCrouching && canLevitate)
        {
            if (!isGrounded)
            {
                velocity.y += -2f * Time.deltaTime;
                isLevitating = true;
            }
            else isLevitating = false;
        }
        else
        {
            isLevitating = false;
            handleFallDamage();
            if (!isGrounded)
                velocity.y += gravity * gravityMultiplier * Time.deltaTime;
            if (velocity.y > gravity && isGrounded)
                velocity.y = gravity;
        }

        //GESTIONE SALTO E DOPPIO SALTO
        if (Input.GetButtonDown("Jump") && isGrounded) {
            jump();
            canDoubleJump = true;
        }
        else if (Input.GetButtonDown("Jump") && canDoubleJump) {
            StartCoroutine(handleDoubleJump());
            hasDoubleJumped = true;
            jump();
            canDoubleJump = false;
        }
        if (hasDoubleJumped && isGrounded) {
            hasDoubleJumped = false;
        }

        //GESTIONE ATTACCO
        if (Input.GetMouseButtonDown(0) && canShoot && !isDefending && magicStamina > 0f)
        {
            //ATTACCA
            if (canShoot) {
                isAttacking = true;
                Invoke(nameof(shoot), timeToShoot);
                canShoot = false;
                Invoke(nameof(resetShoot), 1f);
            }
        }

        //GESTIONE DIFESA
        if (Input.GetMouseButtonDown(1) && !isAttacking && magicStamina > 0f) {
            shieldRenderer.enabled = true;
            decalRenderer.enabled = true;
            isDefending = true;
            shieldSoundSource.Play();
        }
        if (Input.GetMouseButtonUp(1) || magicStamina == 0f) {
            shieldSoundSource.Stop();
            shieldRenderer.enabled = false;
            decalRenderer.enabled = false;
            isDefending = false;
        }
    }

    void handleFootSteps()
    {
        if (!isWalking && !isRunning) return;

        timeBetweenFootSteps = isWalking ? 0.5f : 0.3f;
        timeAfterFootSteps += Time.deltaTime;
        if (timeAfterFootSteps >= timeBetweenFootSteps)
        {
            footStepSource.clip = footStepSounds[Random.Range(0, footStepSounds.Count)];
            footStepSource.Play();
            //StartCoroutine(playerCamera.shakeCamera(.2f, .25f, .25f, .25f, 0.017f));
            timeAfterFootSteps = 0f;
        }
    }

    void handleMovementPrediction() {
        Vector3 XZVelocity = velocity;
        XZVelocity.y = 0;
        predictedMovement.position = Vector3.Lerp(predictedMovement.position, transform.position + XZVelocity * predictedVelocityMultiplier, 0.25f);
    }

    void resetShoot() {
        canShoot = true;
    }

    void handleFallDamage() {
        if (isGrounded && velocity.y < -30) {
            health = 0;
            checkHealth();
        }
    }

    void handleHeadBob() {
        if (!isGrounded) return;

        if (isMoving) {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isWalking ? walkBobSpeed : runBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYpos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isWalking ? walkBobAmount : runBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    IEnumerator handleCrouch() {

        bool applyGravity = false;
        float currentHeight = controller.height;
        float targetHeight = isCrouching ? 2f : 1.25f;
        float timeElapsed = 0;
        float timeToCrouch = 0.1f;

        Vector3 currentCenter = controller.center;
        Vector3 targetCenter = isCrouching ? new Vector3(0f, 0f, 0f) : new Vector3(0f, 0.25f, 0f);
        
        canCrouch = false;

        if (isRunning)
            speed = 3f;
        if (isGrounded)
            applyGravity = true;

        while (timeElapsed < timeToCrouch) {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;

            if (applyGravity)
            {
                velocity.y = gravity * gravityMultiplier;
                controller.Move(velocity * Time.deltaTime);
            }
            yield return null;
        }

        controller.height = targetHeight;
        controller.center = targetCenter;
        canCrouch = true;
    }

    void shoot() {
        RaycastHit hit;
        shootBeam.enabled = true;
        shootBeam.SetPosition(0, shootSource.transform.position);
        attackSoundSource.Play();
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 42f, rayCastLayer))
        {
            shootBeam.SetPosition(1, hit.point);
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * hit.distance, Color.green);
            if (hit.collider.tag == "Guardian")
                hit.collider.GetComponent<GuardianController>().takeDamage(attackDamage);
        }
        else {
            shootBeam.SetPosition(1, playerCamera.transform.position + playerCamera.transform.forward * 100f);
        }
        particleShoot.transform.position = shootSource.transform.position;
        particleShoot.Play();
        StartCoroutine(expandShootBeam());
        StartCoroutine(shrinkShootBeam());
        isAttacking = false;
        //Invoke(nameof(removeBeam), 1f);
    }

    IEnumerator expandShootBeam() {
        float timeToExpand = 0.25f;
        float timeElapsed = 0f;
        while (timeElapsed < timeToExpand) {
            timeElapsed += Time.deltaTime;
            shootBeam.widthMultiplier = Mathf.Lerp(shootBeam.widthMultiplier, 2f, timeElapsed / timeToExpand);
            yield return null;
        }
        shootBeam.widthMultiplier = 1f;
    }

    IEnumerator shrinkShootBeam() {
        float timeToShrink = 0.25f;
        float timeElapsed = 0f;
        while (timeElapsed < timeToShrink)
        {
            timeElapsed += Time.deltaTime;
            shootBeam.widthMultiplier = Mathf.Lerp(shootBeam.widthMultiplier, 0f, timeElapsed / timeToShrink);
            yield return null;
        }
        shootBeam.widthMultiplier = 0f;
        shootBeam.enabled = false;
    }

    void handleAnimations() {
        if (isAttacking) {
            anim.SetInteger("Attacking", Random.Range(1, 3));
        }
        else
            anim.SetInteger("Attacking", 0);

        if (isDefending)
            anim.SetBool("Defending", true);
        else
            anim.SetBool("Defending", false);
        
        if (isGrounded)
            action = isCrouching ? Mathf.Lerp(action, 2, 0.25f) : isWalking ? Mathf.Lerp(action, 4f, 0.25f) : 
                isRunning ? Mathf.Lerp(action, 5f, 0.25f) : Mathf.Lerp(action, 3f, 0.1f);
        else
            action = isLevitating ? Mathf.Lerp(action, 1, 0.25f) : Mathf.Lerp(action, 6f, 0.1f);

        anim.SetFloat("Blend", action);
    }

    void jump() {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f)) return;
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);       
        lockRight = transform.right;
        lockForward = transform.forward;
        lockX = x;
        lockZ = z;
        lockSpeed = speed;
    }

    IEnumerator handleDoubleJump() {
        float timeToFinish = 0.36f;
        float timeElapsed = 0;
        anim.SetBool("DoubleJumping", true);
        while (timeElapsed < timeToFinish) {
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        anim.SetBool("DoubleJumping", false);
    }
    void checkHealth()
    {
        if (health <= 0) {
            isAlive = false;
            die();
        }
        else {
            print("Salute rimanente: " + health);
        }
    }

    public void takeDamage(float damageAmount)
    {
        if (isAlive)
        {
            if (!isDefending) {
                print("Ho preso " + damageAmount + " danni");
                health -= damageAmount;
                healthBar.fillAmount = health / maxHealth;
                healthBarEnd.transform.localPosition = new Vector2(healthBarEndStartPosition - (280 - (280 / maxHealth) * health), healthBarEnd.transform.localPosition.y);
                timeAfterAnAction = 0f;
            }
            else
                staminaToRemove += damageAmount * staminaRemovalMultiplier;
            checkHealth();
        }
    }

    void die() {
        transform.localPosition = playerSpawnPosition.localPosition;
        transform.localRotation = playerSpawnPosition.localRotation;
        health = maxHealth;
        magicStamina = maxMagicStamina;
        velocity = new Vector3();
        //isAlive = true;
        print("sono morto!");
    }

    IEnumerator handleStamina() {
        float staminaToAdd = 5f;
        float staminaRemovalTime = 0.25f;
        float staminaAddTime = 0.25f;
        float stopTime = 0f;
        float attackTime = 0f;
        float defenseTime = 0f;
        float runTime = 0f;
        
        while (isAlive) {           
            if (timeAfterAnAction < maxTimeAfterAnAction)
                timeAfterAnAction += Time.deltaTime;
            if(attackTime < maxTimeAfterAnAction)
                attackTime += Time.deltaTime;
            if(defenseTime < maxTimeAfterAnAction)
                defenseTime += Time.deltaTime;
            if(runTime < maxTimeAfterAnAction)
                runTime += Time.deltaTime;
            if(stopTime < maxTimeAfterAnAction)
                stopTime += Time.deltaTime;

            if (isAttacking && attackTime > staminaRemovalTime) {
                staminaToRemove = attackStaminaRemove;
                attackTime = 0f;
            }
                
            if (isDefending && defenseTime > staminaAddTime) {
                if(magicStamina + defenseStaminaAdd <= maxMagicStamina)
                    magicStamina += defenseStaminaAdd;
                defenseTime = 0f;
            }
                
            if (isRunning && runTime > staminaRemovalTime) {
                staminaToRemove = runStaminaRemove;
                runTime = 0f;
            }
            if (staminaToRemove > 0f)
                timeAfterAnAction = 0f;
            if (timeAfterAnAction >= maxTimeAfterAnAction && stopTime > staminaAddTime && magicStamina < maxMagicStamina) {
                if (magicStamina + staminaToAdd > maxMagicStamina)
                    magicStamina = maxMagicStamina;
                else
                    magicStamina += staminaToAdd;
                stopTime = 0f;
            }
            else {
                if (magicStamina - staminaToRemove < 0f)
                    magicStamina = 0f;
                else
                    magicStamina -= staminaToRemove;
            }
            staminaBar.fillAmount = magicStamina / maxMagicStamina;
            staminaBarEnd.transform.localPosition = new Vector2(staminaBarEndStartPosition - (250 - (250/maxMagicStamina) * magicStamina), staminaBarEnd.transform.localPosition.y);
            staminaToRemove = 0f;
            yield return null;
        }
    }
}
