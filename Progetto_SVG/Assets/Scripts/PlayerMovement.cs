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

    [SerializeField] Vector3 velocity;
    [SerializeField] private bool isGrounded;
    private bool isCrouched = false;
    [SerializeField] private bool wantsToUncrouch = false;
    public bool canDoubleJump;
    public bool canLevitate = false;
    private Vector3 lockRight;
    private Vector3 lockForward;

    /*
    private float walkBobSpeed = 14f;
    private float walkBobAmount = 0.05f;
    private float crouchBobSpeed = 8f;
    private float crouchBobAmount = 0.025f;
    private float defaultYpos = 0;
    private float timer;
    */

    void Awake() 
    {
        playerCamera = GetComponentInChildren<Camera>();
        //defaultYpos = playerCamera.transform.localPosition.y;
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
    void checkIfGrounded()
    { 
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    //GESTIONE MOVIMENTO
    void handleInputs()
    {
        //MOVIMENTO
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move;
        if (isGrounded)
            move = transform.right * x + transform.forward * z;
        else
            move = lockRight * x + lockForward * z;

        
        controller.Move(move * speed * Time.deltaTime);

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
        /*FALL DAMAGE DA RIVEDERE
        if (isGrounded)
        {
            if(airTime > minSurviveFall)
            {
                //playerHealth -= damageForSeconds * airTime;
                print("mi sono fatto male");
            }
            
            airTime = 0;
        }
        */
    }

    void handleAnimations()
    {
        anim.SetFloat("Blend", 0f);
    }

    void jump() 
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        lockRight = transform.right;
        lockForward = transform.forward;
    }
}
