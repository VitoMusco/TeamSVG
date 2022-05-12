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

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    public float airTime = 0;
    public float minSurviveFall = 6f;
    public float damageForSeconds = 1f;
    public float playerHeslth = 10;

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
        handleFallDamage();
        handleAnimations();
    }

    //CONTROLLA SE SI E' A TERRA
    void checkIfGrounded() => isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

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
        if (!isCrouched && Input.GetKeyDown("left ctrl"))
        {
            if (isRunning)
                speed = 3f;

            controller.height = 1f;
            groundCheck.position = groundCheck.position + new Vector3(0f, .5f, 0f);
            isCrouched = true;  
        }
        
        if (isCrouched && Input.GetKeyUp("left ctrl"))
        {
            wantsToUncrouch = true;
        }
        if (isCrouched && wantsToUncrouch)
        {
            if(!Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            {
                controller.height = 2f;
                groundCheck.position = groundCheck.position - new Vector3(0f, .5f, 0f);
                isCrouched = false;
                wantsToUncrouch = false;
            }
        }

        //GESTIONE LEVITAZIONE
        if (isCrouched && canLevitate)
        {
            if(!isGrounded)
                velocity.y += -2f * Time.deltaTime;
        }
        else
        {
            if(!isGrounded)
                velocity.y += gravity * gravityMultiplier * Time.deltaTime;
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

    void handleAnimations()
    {
        if (isGrounded)
            action = isWalking ? Mathf.Lerp(action, 1f, 0.25f) : isRunning ? Mathf.Lerp(action, 2f, 0.25f) : Mathf.Lerp(action, 0f, 0.1f);
        else
            action = Mathf.Lerp(action, 0f, 0.1f);
        anim.SetFloat("Blend", action);
    }

    void jump() 
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        lockRight = transform.right;
        lockForward = transform.forward;
        lockSpeed = speed;
    }
}
