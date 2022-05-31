using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slamBehaviour : MonoBehaviour
{
    [SerializeField] private float timeToExpand = 0f;
    [SerializeField] private float startSize = 0.15f;
    [SerializeField] private float endSize = 2f;
    private Vector3 desiredScale;

    void Start() {
        transform.localScale = new Vector3(startSize, 1f, startSize);
        desiredScale = new Vector3(endSize, 1f, endSize);
    }

    void Update()
    {
        StartCoroutine(expand());
    }

    private IEnumerator expand() {
        float timeElapsed = 0;
        Vector3 startScale = transform.localScale;
        while (timeElapsed < timeToExpand) {
            timeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, desiredScale, timeElapsed / timeToExpand);
            yield return null;
        }
        transform.localScale = desiredScale;
        Destroy(gameObject);
    }
}
