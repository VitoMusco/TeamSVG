using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController controller;
    private Camera playerCamera;
    private Animator anim;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float jumpHeight = 3f;

    [SerializeField] private float action = 0;
    [SerializeField] Vector3 velocity;
    [SerializeField] private bool wantsToUncrouch = false;
    [SerializeField] private bool canDoubleJump;
    [SerializeField] private bool canLevitate = false;

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


    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool isLevitating = false;
    [SerializeField] private bool hasDoubleJumped = false;

    void Awake() {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        defaultYpos = playerCamera.transform.localPosition.y;
        anim = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update() {
        checkIfGrounded();
        handleInputs();
        handleAnimations();
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
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move;
        if (isGrounded) {
            move = transform.right * x + transform.forward * z;
            move = move * speed;
        }
        else {
            move = lockRight * x + lockForward * z;
            move = move * lockSpeed;
        }

        checkIfMoving(move.x, move.z);
        handleHeadBob();
        
        controller.Move(move * Time.deltaTime);

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

    void handleAnimations() {
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
}
