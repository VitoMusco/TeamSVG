using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slamBehaviour : MonoBehaviour
{
    [SerializeField] private float timeToExpand = 0f;
    [SerializeField] private float startSize = 0.15f;
    [SerializeField] private float endSize = 2f;
    [SerializeField] private float slamDamage = 30f;
    [SerializeField] private Vector3 desiredScale;
    [SerializeField] private AudioSource soundSource;

    void Start() {
        transform.localScale = new Vector3(startSize, 1f, startSize);
        desiredScale = new Vector3(endSize, 1f, endSize);
    }

    void Awake() {
        soundSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        StartCoroutine(expand());
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            collider.gameObject.GetComponent<PlayerController>().takeDamage(slamDamage);
        }
    }

    private IEnumerator expand() {
        float timeElapsed = 0;
        Vector3 startScale = transform.localScale;
        while (timeElapsed < timeToExpand) {
            timeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, desiredScale, timeElapsed / timeToExpand);
            if(timeElapsed >= timeToExpand - timeToExpand / 4)
                soundSource.volume = Mathf.Lerp(1f, 0f, timeElapsed / timeToExpand);
            yield return null;
        }
        transform.localScale = desiredScale;
        Destroy(gameObject);
    }
}
