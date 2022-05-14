using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    private Camera playerCamera;
    private Animator anim;

    public float speed = 3f;
    public float gravity = -9.81f;
    public float gravityMultiplier = 2f;
    public float jumpHeight = 3f;


    public float airTime = 0;

    [SerializeField] private float action = 0;
    [SerializeField] Vector3 velocity;
    [SerializeField] private bool wantsToUncrouch = false;
    public bool canDoubleJump;
    public bool canLevitate = false;
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
    [SerializeField] private bool isCrouched = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isRunning = false;

    void Awake() 
    {
        playerCamera = GetComponentInChildren<Camera>();
        defaultYpos = playerCamera.transform.localPosition.y;
        anim = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        checkIfGrounded();
        handleInputs();
        handleAnimations();
    }

    //CONTROLLA SE SI E' A TERRA
    void checkIfGrounded() => isGrounded = controller.isGrounded;

    void checkIfMoving(float x, float z) 
    {
        if (Mathf.Abs(x) > 0.1 || Mathf.Abs(z) > 0.1)
            isMoving = true;
        else
            isMoving = false;

        if (isMoving && speed == 3f && !isCrouched && isGrounded)
            isWalking = true;
        else 
            isWalking = false;

        if (isMoving && speed == 6f && !isCrouched && isGrounded)
            isRunning = true;
        else
            isRunning = false;
    }

    //GESTIONE MOVIMENTO
    void handleInputs()
    {
        //MOVIMENTO
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move;
        if (isGrounded)
        {
            move = transform.right * x + transform.forward * z;
            move = move * speed;
        }
        else
        {
            move = lockRight * x + lockForward * z;
            move = move * lockSpeed;
        }

        checkIfMoving(move.x, move.z);
        handleHeadBob();
        
        controller.Move(move * Time.deltaTime);


        if (!isCrouched && Input.GetKeyDown("left shift"))
            speed = 6f;   
        if (Input.GetKeyUp("left shift"))
            speed = 3f;

        //GESTIONE SALTO E DOPPIO SALTO
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            canDoubleJump = true;
            jump();
        }
        else if (Input.GetButtonDown("Jump") && canDoubleJump)
        {
            jump();
            canDoubleJump = false;
        }

        //GESTIONE CROUCH E LEVITAZIONE     
        if (Input.GetKeyDown("left ctrl") && !isCrouched)
        {
            StartCoroutine(handleCrouch());
            isCrouched = true;
        }
        if (Input.GetKeyUp("left ctrl"))
        {
            wantsToUncrouch = true;
        }
        if (isCrouched && wantsToUncrouch)
        {
            if (!Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            {
                StartCoroutine(handleCrouch());
                isCrouched = false;
                wantsToUncrouch = false;
            }
        }
        if (!isCrouched && wantsToUncrouch) 
        {
            wantsToUncrouch = false;
        }

        //GESTIONE LEVITAZIONE
        if (isCrouched && canLevitate)
        {
            if(!isGrounded)
                velocity.y += -2f * Time.deltaTime;
        }
        else
        {
            handleFallDamage();
            if(!isGrounded)
                velocity.y += gravity * gravityMultiplier * Time.deltaTime;
            else
                if(velocity.y < gravity)
                    velocity.y = gravity;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void handleFallDamage() 
    {
        if (isGrounded && velocity.y < -30)
        {     
            print("mi sono fatto male");
        }
    }

    void handleHeadBob()
    {
        if (!isGrounded) return;

        if (isMoving) {
            timer += Time.deltaTime * (isCrouched ? crouchBobSpeed : isWalking ? walkBobSpeed : runBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYpos + Mathf.Sin(timer) * (isCrouched ? crouchBobAmount : isWalking ? walkBobAmount : runBobAmount),
                playerCamera.transform.localPosition.z
                );
        }
    }

    IEnumerator handleCrouch() 
    {
        if (isRunning)
            speed = 3f;

        float currentHeight = controller.height;
        float targetHeight = isCrouched ? 2f : 1.25f;
        float timeElapsed = 0;
        float timeToCrouch = 0.1f;

        Vector3 currentCenter = controller.center;
        Vector3 targetCenter = isCrouched ? new Vector3(0f, 0f, 0f) : new Vector3(0f, 0.25f, 0f);

        while (timeElapsed < timeToCrouch)
        {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        controller.height = targetHeight;
        controller.center = targetCenter;
    }

    void handleAnimations()
    {
        if (isGrounded)
            action = isCrouched ? Mathf.Lerp(action, 1, 0.25f) : isWalking ? Mathf.Lerp(action, 3f, 0.25f) : isRunning ? Mathf.Lerp(action, 4f, 0.25f) : Mathf.Lerp(action, 2f, 0.1f);
        else
            action = Mathf.Lerp(action, 5f, 0.1f);
        anim.SetFloat("Blend", action);
    }

    void jump() 
    {
        if (isCrouched && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f)) return;
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        lockRight = transform.right;
        lockForward = transform.forward;
        lockSpeed = speed;
    }
}
