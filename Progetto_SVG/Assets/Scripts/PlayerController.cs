using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //LEVELTYPE 1=MAZE 2=PARKOUR 3=COMBAT
    public int levelType = 0;

    //CAMERA
    private Camera playerCamera;

    public float mouseSensitivity = 1f;
    public float swaySmooth = 1f;
    public float swayMultiplier = 1f;
    public bool shake = false;
    public Transform playerArms;
    Quaternion swayRotationX;
    Quaternion swayRotationY;
    Quaternion targetRotation;

    float yRotation = 0f;

    //INPUTS
    public PlayerInput inputs;

    private InputAction move;
    private InputAction look;

    //DEATH
    [SerializeField] private float fallAngle = 90f;
    [SerializeField] private float fallTime = 0.15f;

    //UI CROSSHAIR CHANGE
    public Sprite dotCrosshair, upArrow, downArrow, confirmCrosshair;
    public Image crosshair;

    //PLAYER
    public bool developerMode = false;
    public bool hasRespawned = false;
    public Transform predictedMovement;
    public Transform shootSource;
    public Transform playerSpawnPosition;
    public CompassBehaviour compass;
    public MeshRenderer levitationBraceletRendererL, levitationBraceletRendererR;
    public MeshRenderer compassRenderer;
    public MeshRenderer compassNorthRenderer;
    public MeshRenderer attackBraceletRenderer;
    public MeshRenderer compassBraceletRenderer;
    public MeshRenderer shieldRenderer;
    public MeshRenderer shieldBraceletRenderer;
    public MeshRenderer decalRenderer;
    public ParticleSystem particleShoot;
    public LayerMask rayCastLayer;
    public AudioSource attackSoundSource;
    public AudioSource shieldSoundSource;
    public AudioSource footStepSource;
    public List<AudioClip> footStepSounds;
    public AudioClip playerAttackSound;
    public AudioClip playerLaserAttackSound;
    public Material laserMaterial;
    public Material electricMaterial;

    public Image healthBar;
    public Image healthBarEnd;
    public Image staminaBar;
    public Image staminaBarEnd;
    public float healthBarEndPos = 0f;
    public float healthBarEndStartPosition;
    public float staminaBarEndPos = 0f;
    public float staminaBarEndStartPosition;

    //SHIELD
    private Material[] shieldMaterials;
    private float shieldHealth = 1f;

    private LineRenderer shootBeam;
    private CharacterController controller;
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

    private float x;
    private float z;
    private float lockX;
    private float lockZ;  

    private Vector3 lockRight;
    private Vector3 lockForward;
    private float lockSpeed;


    public float crouchSpeed = 1.5f;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    private float runBobSpeed = 14f;
    private float runBobAmount = 0.08f;
    private float walkBobSpeed = 10f;
    private float walkBobAmount = 0.05f;
    private float crouchBobSpeed = 6f;
    private float crouchBobAmount = 0.025f;
    private float defaultYpos = 0;
    private float timer;
    private bool isAlive = true;
    private bool isInMenu = false;

    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float magicStamina = 100f;
    [SerializeField] private float maxMagicStamina = 100f;
    [SerializeField] private float timeAfterAnAction = 0f;
    [SerializeField] private float timeToShoot = 0.4f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private bool isLevitating = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isShootAttacking = false;
    [SerializeField] private bool isLaserAttacking = false;
    [SerializeField] private bool isDefending = false;
    [SerializeField] private bool isUsingCompass = false;
    [SerializeField] private bool wantsToRun = false;
    [SerializeField] private bool wantsToStopAttacking = false;
    [SerializeField] private bool wantsToStopDefending = false;
    [SerializeField] private bool wantsToStopUsingCompass = false;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float laserAttackDamage = 2f;
    [SerializeField] private float shootAttackStaminaRemove = 10;
    [SerializeField] private float laserAttackStaminaRemove = 2;
    [SerializeField] private float defenseStaminaAdd = 2;
    [SerializeField] private float runStaminaRemove = 5;
    [SerializeField] private float staminaToRemove = 0f;
    [SerializeField] private float staminaRemovalMultiplier = 1f;
    [SerializeField] private bool wantsToUncrouch = false;
    [SerializeField] private bool canDoubleJump = false;
    [SerializeField] private bool canLevitate = false;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 addedVelocity;

    [SerializeField] private bool hasDoubleJumped = false;
    [SerializeField] private float maxTimeAfterAnAction = 2f;
    [SerializeField] float timeToRespawn = 2f;

    [SerializeField] float timeAfterNotGrounded = 0f;
    [SerializeField] float maxTimeAfterNotGrounded = 0.1f;

    void Awake() {
        //INPUT//
        inputs = new PlayerInput();

        move = inputs.PlayerInputs.Move;
        look = inputs.PlayerInputs.Look;

        move.Enable();
        look.Enable();
        inputs.PlayerInputs.Crouch.performed += context => crouch();
        inputs.PlayerInputs.Crouch.canceled += context => unCrouch();       
        inputs.PlayerInputs.Run.performed += context => run();
        inputs.PlayerInputs.Run.canceled += context => stopRunning();       
        inputs.PlayerInputs.Jump.performed += context => jump();        
        inputs.PlayerInputs.Attack.performed += context => attack();        
        inputs.PlayerInputs.LaserAttack.performed += context => laserAttack();
        inputs.PlayerInputs.LaserAttack.canceled += context => stopLaserAttack();        
        inputs.PlayerInputs.Defend.performed += context => defend();
        inputs.PlayerInputs.Defend.canceled += context => cancelDefense();
        inputs.PlayerInputs.UseCompass.performed += context => useCompass();
        inputs.PlayerInputs.UseCompass.canceled += context => cancelCompass();
        inputs.PlayerInputs.Interact.performed += context => interact();

        if (levelType == 3) inputs.PlayerInputs.Attack.Enable();
        if (levelType == 3) inputs.PlayerInputs.LaserAttack.Enable();
        if (levelType == 1) {
            compass.enable();
            inputs.PlayerInputs.UseCompass.Enable();
        } 
        if (levelType == 3) inputs.PlayerInputs.Defend.Enable();
        inputs.PlayerInputs.Crouch.Enable();
        inputs.PlayerInputs.Run.Enable();
        inputs.PlayerInputs.Jump.Enable();
        inputs.PlayerInputs.Interact.Enable();
        /////////
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (developerMode) {
            health = 1000000;
            magicStamina = 1000000;
        }
        shieldMaterials = shieldRenderer.materials;
        staminaBarEndStartPosition = staminaBarEnd.transform.localPosition.x;
        healthBarEndStartPosition = healthBarEnd.transform.localPosition.x;
        levitationBraceletRendererL.enabled = false;
        levitationBraceletRendererR.enabled = false;
        compassRenderer.enabled = false;
        compassNorthRenderer.enabled = false;
        shieldRenderer.enabled = false;
        compassBraceletRenderer.enabled = false;
        disableAttackBracelet();
        shieldBraceletRenderer.enabled = false;
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

    void OnDestroy() {
        inputs.PlayerInputs.Crouch.Disable();
        inputs.PlayerInputs.Run.Disable();
        inputs.PlayerInputs.Jump.Disable();
        inputs.PlayerInputs.Attack.Disable();
        inputs.PlayerInputs.LaserAttack.Disable();
        if (levelType == 1) inputs.PlayerInputs.UseCompass.Disable();
        if (levelType == 3) inputs.PlayerInputs.Defend.Disable();
        inputs.PlayerInputs.Interact.Disable();
    }

    void Update() {
        if (isAlive) {
            if (!isInMenu){
                handleCameraLook();
                if (levelType == 1) handleCrosshair();
                checkIfGrounded();
                handleInputs();
                handleFootSteps();
                handleMovementPrediction();
                handleAnimations();
            }
            
        }     
    }

    void LateUpdate() {
        if (isAlive) {
            movePlayer();
        }
    }

    public void setFov(float fov) { 
        playerCamera.fieldOfView = fov;
    }

    void handleCameraLook() {
        Vector2 mouseInputs = look.ReadValue<Vector2>();
        float mouseX = mouseInputs.x * mouseSensitivity;
        float mouseY = mouseInputs.y * mouseSensitivity;

        swayRotationX = Quaternion.AngleAxis(-mouseY * swayMultiplier * swaySmooth, Vector3.right);
        swayRotationY = Quaternion.AngleAxis(mouseX * swayMultiplier * swaySmooth, Vector3.up);

        targetRotation = swayRotationX * swayRotationY;

        playerArms.localRotation = Quaternion.Slerp(playerArms.localRotation, targetRotation, swaySmooth * Time.deltaTime);

        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void handleCrosshair() {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 3f, rayCastLayer)) {
            if (hit.collider.CompareTag("ConfirmButton")) crosshair.sprite = confirmCrosshair;
            else if (hit.collider.gameObject.name == "ButtonUp") crosshair.sprite = upArrow;
            else if (hit.collider.gameObject.name == "ButtonDown") crosshair.sprite = downArrow;
            else crosshair.sprite = dotCrosshair;
        }
        else crosshair.sprite = dotCrosshair;
    }

    //CONTROLLA SE SI E' A TERRA
    void checkIfGrounded() {
        if (timeAfterNotGrounded > maxTimeAfterNotGrounded && !controller.isGrounded && !isJumping) {
            isGrounded = false;
        }
        else if (timeAfterNotGrounded < maxTimeAfterNotGrounded && !controller.isGrounded && !isJumping) {
            timeAfterNotGrounded += Time.deltaTime;
        }
        else if (controller.isGrounded)
        {
            if (!isGrounded) {
                footStepSource.clip = footStepSounds[Random.Range(0, footStepSounds.Count)];
                footStepSource.Play();
            } 
            isGrounded = true;
            isJumping = false;
            timeAfterNotGrounded = 0f;
        }
    }

    void movePlayer() {
        verticalVelocity = velocity.y;

        if (isGrounded)
        {
            velocity = transform.right * x + transform.forward * z;
            velocity = velocity * speed;
        }
        else
        {
            velocity = Vector3.Lerp(velocity, (lockRight * lockX + lockForward * lockZ) * lockSpeed, 0.25f);
        }

        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void checkIfMoving(float x, float z) {
        if (Mathf.Abs(x) > 0.1 || Mathf.Abs(z) > 0.1)
            isMoving = true;
        else
            isMoving = false;

        if (isMoving && speed == 3f && !isCrouching && isGrounded)
            isWalking = true;
        else 
            isWalking = false;
    }

    //GESTIONE INPUT
    void handleInputs() {
        //MOVIMENTO

        Vector2 movementInputs = move.ReadValue<Vector2>();
        x = movementInputs.x;
        z = movementInputs.y;

        checkIfMoving(velocity.x, velocity.z);
        handleHeadBob();

        if (wantsToRun && isMoving && isGrounded) {
            isRunning = true;
        }
        if (!isMoving && isRunning || !isGrounded) {
            isRunning = false;
        }
        speed = isCrouching ? crouchSpeed : (!isCrouching && isRunning && magicStamina > 0f) ? runSpeed : (!isGrounded && wantsToRun) ? runSpeed : walkSpeed;

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
                levitationBraceletRendererL.enabled = true;
                levitationBraceletRendererR.enabled = true;
            }
            else stopLevitating();
        }
        else
        {
            stopLevitating();
            handleFallDamage();
            if (!isGrounded)
                velocity.y += gravity * gravityMultiplier * Time.deltaTime;
            if (isGrounded)
                velocity.y = gravity;
        }

        if (hasDoubleJumped && isGrounded) {
            hasDoubleJumped = false;
        }

        if (wantsToStopAttacking) endLaserAttack();
        if (wantsToStopDefending) stopDefending();
        if (wantsToStopUsingCompass) stopUsingCompass();

        if (isDefending && magicStamina <= 0f) {
            stopDefending();
        }

        if (isLaserAttacking && magicStamina <= 0f) {
            stopLaserAttack();
        }
    }

    void stopLevitating() { 
        isLevitating = false;
        levitationBraceletRendererL.enabled = false;
        levitationBraceletRendererR.enabled = false;
    }
    

    public void stopDoingAnything() {
        stopDefending();
        stopRunning();
        endLaserAttack();
    }

    void crouch() {
        if (!isCrouching && canCrouch) {
            StartCoroutine(handleCrouch());
            isCrouching = true;
            wantsToUncrouch = false;
        }
    }

    void unCrouch()
    {
        wantsToUncrouch = true;
    }

    void jump() {
        if (isGrounded || !isGrounded && !hasDoubleJumped && canDoubleJump) {
            if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f)) return;

            if (!isGrounded && !hasDoubleJumped)
            {
                StartCoroutine(handleDoubleJump());
                hasDoubleJumped = true;
            }
            if (!hasDoubleJumped || hasDoubleJumped && velocity.y > gravity + gravity/2) velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            else velocity.y = velocity.y + Mathf.Sqrt(jumpHeight * -2 * gravity);
            lockRight = transform.right;
            lockForward = transform.forward;
            lockX = x;
            lockZ = z;
            lockSpeed = speed;
            isJumping = true;
            isGrounded = false;
            movePlayer();
        }
    }

    void run() {
        if (isGrounded && !isCrouching)
            wantsToRun = true;
    }

    void stopRunning()
    {
        wantsToRun = false;
        isRunning = false;
    }

    void attack()
    {
        if (isInMenu) return;
        if (canShoot && !isDefending && magicStamina > 0f)
        {
            shootBeam.material = laserMaterial;
            isAttacking = true;
            enableAttackBracelet();
            isShootAttacking = true;
            Invoke(nameof(shoot), timeToShoot);
            canShoot = false;
            Invoke(nameof(resetShoot), 1f);
        }
    }

    void enableAttackBracelet() {
        attackBraceletRenderer.enabled = true;
    }

    void laserAttack() {
        if (isInMenu) return;
        if (canShoot && !isDefending && magicStamina > 0f) {
            isLaserAttacking = true;
            enableAttackBracelet();
            StartCoroutine(startLaserAttacking());
        }
    }

    IEnumerator startLaserAttacking() {
        float timeAfterInput = 0f;
        float timeToShoot = 1.05f;
        while (timeAfterInput < timeToShoot && isLaserAttacking) {
            timeAfterInput += Time.deltaTime;
            yield return null;
        }
        if (isLaserAttacking) {
            isAttacking = true;
            shootBeam.material = electricMaterial;
            StartCoroutine(expandShootBeam());
            attackSoundSource.clip = playerLaserAttackSound;
            attackSoundSource.loop = true;
            attackSoundSource.Play();
            shootBeam.enabled = true;
            StartCoroutine(updateLaserAttack());
            StartCoroutine(shakeCamera(0f, .2f, 1f, .25f, 0.007f));
        }
    }

    void interact() {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 3f, rayCastLayer)) {
            if (hit.collider.tag == "Button")
                hit.collider.GetComponent<ButtonBehaviour>().pressButton();
            else if (hit.collider.CompareTag("ConfirmButton"))
                hit.collider.GetComponent<ConfirmButtonBehaviour>().pressButton();
        }
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 5f, rayCastLayer)) {
            if (hit.collider.tag == "Quiz") {
                compass.foundQuiz(hit.transform.gameObject);
                hit.collider.GetComponent<QuizBehaviour>().getGrabbed();
                //getQuizPaper();Provvisorio
            }
        }
    }


    IEnumerator updateLaserAttack() {
        RaycastHit hit;
        float timeElapsed = 0f;
        float timeBetweenTicks = 0.25f;
        while (isLaserAttacking) {
            timeElapsed += Time.deltaTime;
            shootBeam.SetPosition(0, shootSource.transform.position);
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 42f, rayCastLayer))
            {
                shootBeam.SetPosition(1, hit.point);
                Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * hit.distance, Color.green);
                if (hit.collider.tag == "Guardian")
                    if (timeElapsed >= timeBetweenTicks) {
                        hit.collider.GetComponent<GuardianController>().takeDamage(laserAttackDamage);
                        timeElapsed = 0f;
                    }
            }
            else {
                shootBeam.SetPosition(1, playerCamera.transform.position + playerCamera.transform.forward * 100f);
            }
            //particleShoot.transform.position = shootSource.transform.position;
            yield return null;
        }
    }

    void stopLaserAttack() {
        if (!isInMenu) endLaserAttack();
        else wantsToStopAttacking = true;
    }

    void endLaserAttack() {
        if (!isLaserAttacking) return;
        disableAttackBracelet();
        isLaserAttacking = false;
        if (isAttacking) {
            isAttacking = false;
            canShoot = false;
            attackSoundSource.Stop();
            StartCoroutine(shrinkShootBeam());
            Invoke(nameof(resetShoot), 1f);
        }
        wantsToStopAttacking = false;
    }

    void defend()
    {
        if (!isDefending && !isAttacking && magicStamina > 0f)
        {
            shieldRenderer.enabled = true;
            shieldBraceletRenderer.enabled = true;
            decalRenderer.enabled = true;
            isDefending = true;
            shieldSoundSource.Play();
        }
    }

    void useCompass() {
        if (!isUsingCompass) {
            compassRenderer.enabled = true;
            compassBraceletRenderer.enabled = true;
            compassNorthRenderer.enabled = true;
            isUsingCompass = true;
        }
    }

    void cancelCompass() {
        if (!isInMenu) stopUsingCompass();
        else wantsToStopUsingCompass = true;
    }

    void stopUsingCompass() {
        compassRenderer.enabled = false;
        compassBraceletRenderer.enabled = false;
        compassNorthRenderer.enabled = false;
        isUsingCompass = false;
        wantsToStopUsingCompass = false;
    }

    void cancelDefense() {
        if (!isInMenu) stopDefending();
        else wantsToStopDefending = true;
    }

    void stopDefending() {
        shieldSoundSource.Stop();
        shieldRenderer.enabled = false;
        shieldBraceletRenderer.enabled = false;
        decalRenderer.enabled = false;
        isDefending = false;
        wantsToStopDefending = false;
    }

    void handleFootSteps()
    {
        if (!isWalking && !isRunning || !isGrounded || isCrouching) return;

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

    void disableAttackBracelet() {
        attackBraceletRenderer.enabled = false;
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
        Invoke(nameof(disableAttackBracelet), 0.5f);
        RaycastHit hit;
        shootBeam.enabled = true;
        shootBeam.SetPosition(0, shootSource.transform.position);
        attackSoundSource.clip = playerAttackSound;
        attackSoundSource.loop = false;
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
        isShootAttacking = false;
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
        particleShoot.Stop();
        shootBeam.enabled = false;
    }

    void handleAnimations() {

        if (isShootAttacking) anim.SetBool("ShootAttacking", true);
        else anim.SetBool("ShootAttacking", false);
        if (isLaserAttacking) anim.SetBool("LaserAttacking", true);
        else anim.SetBool("LaserAttacking", false);

        if (isUsingCompass)
            anim.SetBool("UsingCompass", true);
        else
            anim.SetBool("UsingCompass", false);

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

    IEnumerator handleDoubleJump() {
        float timeToFinish = 0.36f;
        float timeElapsed = 0;
        anim.SetBool("DoubleJumping", true); //Mettilo in handleAnimations
        while (timeElapsed < timeToFinish) {
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        anim.SetBool("DoubleJumping", false);//Mettilo in handleAnimations
    }

    void handleHealthBar() {
        healthBar.fillAmount = health / maxHealth;
        healthBarEnd.transform.localPosition = new Vector2(healthBarEndStartPosition - (280 - (280 / maxHealth) * health), healthBarEnd.transform.localPosition.y);
    }

    void handleStaminaBar() {
        staminaBar.fillAmount = magicStamina / maxMagicStamina;
        staminaBarEnd.transform.localPosition = new Vector2(staminaBarEndStartPosition - (250 - (250 / maxMagicStamina) * magicStamina), staminaBarEnd.transform.localPosition.y);
    }

    void checkHealth()
    {
        if (health <= 0) {
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
                StartCoroutine(shakeCamera(.2f, .25f, .25f, .25f, 0.017f));
                print("Ho preso " + damageAmount + " danni");
                health -= damageAmount;
                handleHealthBar();
                timeAfterAnAction = 0f;
            }
            else
                staminaToRemove += damageAmount * staminaRemovalMultiplier;
            checkHealth();
        }
    }

    public void kill() {
        if(isAlive) die();
    }

    public bool checkIfAlive() {
        return isAlive;
    }

    public void enterMenu() {
        isInMenu = true;
    }

    public void exitMenu() {
        isInMenu = false;
    }

    void die() {
        StartCoroutine(shakeCamera(1f, .25f, .25f, .25f, 0.017f));
        anim.SetBool("isDead", true);
        if (levelType == 1) compass.reset();
        health = 0f;
        magicStamina = 0f;
        lockRight = new Vector3();
        lockForward = new Vector3();
        lockX = 0f;
        lockZ = 0f;
        handleHealthBar();
        handleStaminaBar();
        if (isDefending) stopDefending();
        if (isLaserAttacking) endLaserAttack();
        isGrounded = true;
        isAlive = false;
        velocity = new Vector3();
        StartCoroutine(fall(fallAngle));
        StartCoroutine(respawn());
    }

    IEnumerator fall(float fallAngle) {
        float timeElapsed = 0f;
        if (Random.Range(-1, 1) < 0) fallAngle = -fallAngle;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, fallAngle));

        while (timeElapsed < fallTime) {
            timeElapsed += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / fallTime);
            yield return null;
        }
    }

    IEnumerator respawn() {
        
        float timeElapsed = 0f;
        
        while (timeElapsed < timeToRespawn) {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed = 0f;
        transform.localRotation = playerSpawnPosition.localRotation;
        transform.localPosition = playerSpawnPosition.localPosition;

        while (timeElapsed < 0.1f)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        health = maxHealth;
        magicStamina = maxMagicStamina;
        healthBar.fillAmount = 1f;
        healthBarEnd.transform.localPosition = new Vector2(healthBarEndStartPosition, healthBarEnd.transform.localPosition.y);
        staminaBar.fillAmount = 1f;
        staminaBarEnd.transform.localPosition = new Vector2(staminaBarEndStartPosition, staminaBarEnd.transform.localPosition.y);
        isAlive = true;
        anim.SetBool("isDead", false);
        StartCoroutine(handleStamina());
        StartCoroutine(updateShieldMaterial());
    }

    IEnumerator handleStamina() {
        float staminaToAdd = 5f;
        float staminaRemovalTime = 0.25f;
        float staminaAddTime = 0.25f;
        float stopTime = 0f;
        float attackTime = 0f;
        float defenseTime = 0f;
        float runTime = 0f;
        float targetStamina = magicStamina;
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

            if (isAttacking && isShootAttacking && attackTime > staminaRemovalTime) {
                staminaToRemove = shootAttackStaminaRemove;
                attackTime = 0f;
            }
            if (isAttacking && isLaserAttacking && attackTime > staminaRemovalTime) {
                staminaToRemove = laserAttackStaminaRemove;
                attackTime = 0f;
            }

            if (isDefending && defenseTime > staminaAddTime) {
                if(targetStamina + defenseStaminaAdd <= maxMagicStamina)
                    targetStamina += defenseStaminaAdd;
                defenseTime = 0f;
            }
                
            if (isRunning && runTime > staminaRemovalTime) {
                staminaToRemove = runStaminaRemove;
                runTime = 0f;
            }
            if (staminaToRemove > 0f)
                timeAfterAnAction = 0f;
            if (timeAfterAnAction >= maxTimeAfterAnAction && stopTime > staminaAddTime && targetStamina < maxMagicStamina) {
                if (targetStamina + staminaToAdd > maxMagicStamina)
                    targetStamina = maxMagicStamina;
                else
                    targetStamina += staminaToAdd;
                stopTime = 0f;
            }
            else {
                if (targetStamina - staminaToRemove < 0f)
                    targetStamina = 0f;
                else
                    targetStamina -= staminaToRemove;
            }
            handleStaminaBar();
            if (magicStamina != targetStamina) {
                if (levelType == 3) StartCoroutine(updateShieldMaterial());
                magicStamina = targetStamina;
            }
            staminaToRemove = 0f;
            yield return null;
        }
    }

    IEnumerator updateShieldMaterial() {
        float timeToUpdate = 0.5f;
        float timeElapsed = 0f;
        float startHealth = shieldHealth;
        while (timeElapsed < timeToUpdate) {
            timeElapsed += Time.deltaTime;
            if (shieldMaterials.Length > 0)
            {
                shieldHealth = Mathf.Lerp(startHealth, magicStamina / maxMagicStamina, timeElapsed/timeToUpdate);
                for (int i = 0; i < shieldMaterials.Length; i++)
                {
                    shieldMaterials[i].SetFloat("_DissolveAmount", shieldHealth);
                }
            }
            yield return null;
        }
        shieldHealth = magicStamina / maxMagicStamina;
    }

    public void setLevitation(bool value)
    {
        canLevitate = value;
    }

    public void setSensitivity(float sensitivity) {
        mouseSensitivity = sensitivity;
    }

    public IEnumerator shakeCamera(float shakingTime, float shakeMultiplier, float rotationMultiplier, float shakeRange, float rotationRange)
    {
        float timeElapsed = 0;
        float timeToGenerateNewPosition = 0.1f;
        float timeSinceLastPosition = 0f;
        Vector3 startPosition = playerCamera.transform.localPosition;
        Quaternion startRotation = playerCamera.transform.localRotation;
        float startRotationY = playerCamera.transform.localRotation.y;
        Vector3 positionToReach = new Vector3(startPosition.x + Random.Range(-shakeRange, shakeRange) * shakeMultiplier, startPosition.y + Random.Range(-shakeRange, shakeRange) * shakeMultiplier, startPosition.z);
        float rotationToReach = Random.Range(-rotationRange, rotationRange) * rotationMultiplier;
        while ((timeElapsed < shakingTime && shakingTime > 0) || (isLaserAttacking && shakingTime == 0))
        {
            timeElapsed += Time.deltaTime;
            if (timeSinceLastPosition < timeToGenerateNewPosition)
            {
                timeSinceLastPosition += Time.deltaTime;
                playerCamera.transform.localPosition = Vector3.Lerp(startPosition, positionToReach, timeSinceLastPosition / timeToGenerateNewPosition);
                playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, 
                    new Quaternion(playerCamera.transform.localRotation.x, playerCamera.transform.localRotation.y, 
                    startRotationY + rotationToReach, playerCamera.transform.localRotation.w), timeSinceLastPosition / timeToGenerateNewPosition);
            }
            if (timeSinceLastPosition >= timeToGenerateNewPosition)
            {
                positionToReach = new Vector3(startPosition.x + Random.Range(-shakeRange, shakeRange) * shakeMultiplier, startPosition.y + Random.Range(-shakeRange, shakeRange) * shakeMultiplier, startPosition.z);
                rotationToReach = Random.Range(-rotationRange, rotationRange) * rotationMultiplier;
                timeSinceLastPosition = 0f;
            }

            yield return null;
        }

        playerCamera.transform.localPosition = startPosition;
        playerCamera.transform.localRotation = startRotation;
    }

    public void addVelocity(Vector3 vel) {
        addedVelocity = vel;
    }
}
