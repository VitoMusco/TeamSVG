using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampDenialBlockBehaviour : MonoBehaviour
{
    private PlayerMovement player;
    public ParticleSystem particles;
    public MeshRenderer vfxRenderer;

    [SerializeField] private bool playerInCollider = false;
    [SerializeField] private bool isExpanded = false;
    [SerializeField] private bool isDealingDamage = false;
    [SerializeField] private float timeToExpand = 3f;
    [SerializeField] private float timeToShrink = 3f;
    [SerializeField] private float timeBeforeActivation = 2f;
    [SerializeField] private float timer = 0f;
    [SerializeField] private float damagePerTick = 0f;
    [SerializeField] private float tickTime = 0.5f;

    void OnTriggerEnter(Collider collision) {
        print("pino");
        if (collision.gameObject.tag == "Player") {
            playerInCollider = true;
            vfxRenderer.enabled = true;
        } 
    }

    void OnTriggerExit(Collider collision)
    {
        print("pino2");
        if (collision.gameObject.tag == "Player") {
            playerInCollider = false;
            if(isExpanded)
                vfxRenderer.enabled = false;
        }
                   
    }

    void OnTriggerStay(Collider collision) {
        if (playerInCollider && timer >= timeBeforeActivation && isExpanded && !isDealingDamage)
        {
            isDealingDamage = true;
            player = collision.GetComponent<PlayerMovement>();
            particles.Play();
            StartCoroutine(dealDamage());
        }
        
    }

    void Awake() {
        vfxRenderer.enabled = false;
        particles.Stop();
    }

    void Update()
    {
        handleTimer();
        if (!isDealingDamage) {
            particles.Stop();
        }
    }

    void handleTimer() {
        if (playerInCollider && timer < timeBeforeActivation) {
            if (timer + Time.deltaTime >= timeBeforeActivation)
                timer = timeBeforeActivation;
            else
                timer += Time.deltaTime;
        }
        else {
            if (timer - Time.deltaTime <= 0f)
                timer = 0f;
            else
                timer -= Time.deltaTime;
        }
        if (playerInCollider && timer >= timeBeforeActivation)
            StartCoroutine(expand());
        if (!playerInCollider && timer == 0f && isExpanded == true)
            StartCoroutine(shrink());
    }

    IEnumerator dealDamage() {
        float timeElapsed = 0f;
        while (playerInCollider) {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= tickTime) {
                player.takeDamage(damagePerTick);
                timeElapsed = 0f;
            }
            yield return null;
        }
    }

    IEnumerator expand() {
        float timeElapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 desiredScale = new Vector3(2f, 1f, 2f);
        while (timeElapsed < timeToExpand) {
            timeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, desiredScale, timeElapsed / timeToExpand);
            yield return null;
        }
        transform.localScale = desiredScale;
        isExpanded = true;
    }
    
    IEnumerator shrink() {
        float timeElapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 desiredScale = new Vector3(1f, 1f, 1f);
        while (timeElapsed < timeToShrink) {
            timeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, desiredScale, timeElapsed / timeToShrink);
            yield return null;
        }
        transform.localScale = desiredScale;
        isExpanded = false;
        isDealingDamage = false;
        vfxRenderer.enabled = false;
    }
}
