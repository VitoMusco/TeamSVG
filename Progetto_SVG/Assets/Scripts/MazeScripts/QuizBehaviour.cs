using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizBehaviour : MonoBehaviour
{
    public int id;
    public Transform ghostSpawner;
    public Transform quizPaper;
    public GhostBehaviour ghostPrefab;
    public DoorUnlockerBehaviour quizHandler;
    public ParticleSystem particles;
    public ParticleSystem explosionParticles;

    private AudioSource soundSource;
    private bool isGrabbed = false;
    private float timeBetweenMovements = 3f;
    private float timeSinceLastMovement;
    private float maxVerticalMovement = .2f;
    private float newPos;
    private Vector3 startPosition;
    private Vector3 startScale;
    private Vector3 targetPosition;

    void Awake() {
        soundSource = GetComponent<AudioSource>();
        startPosition = quizPaper.localPosition;
        startScale = transform.localScale;
        newPos = maxVerticalMovement;
        timeSinceLastMovement = timeBetweenMovements;
    }

    void FixedUpdate() {
        if(!isGrabbed)
            rotateAndMoveQuizPaper();
    }

    void rotateAndMoveQuizPaper() {
        quizPaper.Rotate(new Vector3(0f, -1f, 0f), Space.World);
        timeSinceLastMovement += Time.deltaTime;
        if (timeSinceLastMovement >= timeBetweenMovements)
        {
            newPos = -newPos;
            targetPosition = new Vector3(quizPaper.localPosition.x, newPos, quizPaper.localPosition.y);
            startPosition = quizPaper.localPosition;
            timeSinceLastMovement = 0f;
        }
        else {
            timeSinceLastMovement += Time.deltaTime;
            quizPaper.localPosition = Vector3.Lerp(startPosition, targetPosition, Mathf.Sin(timeSinceLastMovement / timeBetweenMovements * Mathf.PI));
        }
    }

    void destroy() {
        if (!GlobalEvents.FirstGhostSpawn) GlobalEvents.FirstGhostSpawn = true;
        ghostPrefab.gameObject.SetActive(true);
        ghostPrefab.transform.position = ghostSpawner.position;
        ghostPrefab.transform.rotation = ghostSpawner.rotation;
        ghostPrefab.activate();
        gameObject.SetActive(false);
    }

    IEnumerator ascendQuizPaper() {
        float timeElapsed = 0f;
        float timeToAscend = 1.3f;
        Vector3 ascendPosition = startPosition + new Vector3(0f, 2f, 0f);
        Vector3 targetScale = new Vector3(0f, 0f, 0f);
        while (timeElapsed < timeToAscend) {
            timeElapsed += Time.deltaTime;
            
            quizPaper.localPosition = Vector3.Lerp(startPosition, ascendPosition, timeElapsed / timeToAscend);
            quizPaper.localScale = Vector3.Lerp(startScale, targetScale, timeElapsed / timeToAscend);
            yield return null;
        }
    }

    public void getGrabbed() {
        soundSource.Play();
        isGrabbed = true;
        particles.Stop();
        StartCoroutine(ascendQuizPaper());
        quizHandler.foundQuiz(id);
        explosionParticles.Play();
        Invoke(nameof(destroy), 1.4f);
    }

    public void reset() {
        gameObject.SetActive(true);
        isGrabbed = false;
        particles.Play();        
        quizPaper.localPosition = startPosition;
        quizPaper.localScale = startScale;
    }
}
