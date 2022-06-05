using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform predictedMovement;
    public Transform shootSource;

    private LineRenderer shootBeam;
    private CharacterController controller;
    private Camera playerCamera;
    private Animator anim;

    [SerializeField] private float predictedVelocityMultiplier = 3f;
    [SerializeField] private float speed = 3f;
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

    [SerializeField] private bool hasDoubleJumped = false;

    void Awake() {
        shootBeam = GetComponentInChildren<LineRenderer>();
        shootBeam.enabled = false;
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        defaultYpos = playerCamera.transform.localPosition.y;
        anim = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update() {
        if (isAlive) {
            checkIfGrounded();
            handleInputs();
            handleMovementPrediction();
            handleAnimations();
        }     
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

    //GESTIONE MOVIMENTO
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
        if (Input.GetKeyUp("left shift"))
            speed = 3f;

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

        //GESTIONE CROUCH  
        if (Input.GetKeyDown("left ctrl") && !isCrouching && canCrouch) {
            StartCoroutine(handleCrouch());
            isCrouching = true;
        }
        if (Input.GetKeyUp("left ctrl")) {
            wantsToUncrouch = true;
        }
        if (Input.GetKeyDown("left ctrl") && isCrouching && wantsToUncrouch)
            wantsToUncrouch = false;
        if (isCrouching && wantsToUncrouch && canCrouch) {
            if (!Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f)) {
                StartCoroutine(handleCrouch());
                isCrouching = false;
                wantsToUncrouch = false;
            }
        }
        

        //GESTIONE LEVITAZIONE
        if (isCrouching && canLevitate) {
            if (!isGrounded) {
                velocity.y += -2f * Time.deltaTime;
                isLevitating = true;
            }
            else isLevitating = false;
        }
        else {
            isLevitating = false;
            handleFallDamage();
            if(!isGrounded)
                velocity.y += gravity * gravityMultiplier * Time.deltaTime;
            else if(velocity.y < gravity)
                    velocity.y = gravity;
        }

        controller.Move(velocity * Time.deltaTime);

        //GESTIONE ATTACCO
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            //ATTACCA
            if (canShoot) {
                isAttacking = true;
                Invoke(nameof(shoot), timeToShoot);
                canShoot = false;
                Invoke(nameof(resetShoot), 1f);
            }
        }
        if (Input.GetMouseButtonUp(0))
            isAttacking = false;

        //GESTIONE DIFESA
        if (Input.GetMouseButtonDown(1))
            isDefending = true;
        if (Input.GetMouseButtonUp(1))
            isDefending = false;
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
            print("mi sono fatto male");
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
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 42f))
        {
            shootBeam.SetPosition(1, hit.point);
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * hit.distance, Color.green);
            if (hit.collider.tag == "Guardian")
                hit.collider.GetComponent<GuardianController>().takeDamage(10f);
        }
        else {
            shootBeam.SetPosition(1, playerCamera.transform.position + playerCamera.transform.forward * 100f);
        }
        Invoke(nameof(removeBeam), 1f);
    }

    void removeBeam() {
        shootBeam.enabled = false;
    }

    void handleAnimations() {
        if (isAttacking)
            anim.SetTrigger("Attacking");
        else
            anim.ResetTrigger("Attacking");
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
    public void checkHealth()
    {
        if (health <= 0)
            isAlive = false;
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
        }
        else
        {
            print("sono morto!");
            Update();
        }
    }
}
