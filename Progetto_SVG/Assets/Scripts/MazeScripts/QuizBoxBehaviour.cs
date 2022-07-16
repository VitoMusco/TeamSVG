using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizBoxBehaviour : MonoBehaviour
{
    public int id;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public int currentPosition = -1; //-1 = Bloccato, 0= Sbloccato
    public int solutionPosition = 1;
    public DoorUnlockerBehaviour quizHandler;
    public int maxPosition = 5;
    public int minPosition = 1;
    private MeshRenderer meshRenderer;
    private Material[] materials;
    [SerializeField] private int answerMaterialPosition = 1;
    [SerializeField] private float timeAfterChangingAnswer = 0;
    [SerializeField] private float timeToWaitBeforeGlowing = 3f;
    [SerializeField] private bool isGlowing = false;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
        startPosition = transform.position + new Vector3(0f, 0.8f, 0f);
        endPosition = startPosition + new Vector3(0f, 1.6f, 0f);
    }

    void Update() {
        handleGlow();
    }

    void handleGlow() {
        timeAfterChangingAnswer += Time.deltaTime;
        if (isGlowing) return;
        if (timeAfterChangingAnswer > timeToWaitBeforeGlowing && currentPosition <= maxPosition && currentPosition >= minPosition) {
            StartCoroutine(glow());
        }
    }

    IEnumerator glow() {
        isGlowing = true;
        while (isGlowing) {
            materials[answerMaterialPosition].SetFloat("_FresnelAmount", 1.25f * Mathf.Sin(timeAfterChangingAnswer * Mathf.PI) + 1.25f);
            yield return null;
        }
    }

    public void moveUp() {
        if (currentPosition == -1) return;
        else if (currentPosition >= maxPosition) {
            transform.position = startPosition;
            currentPosition = minPosition;
        }
        else {
            currentPosition++;
            transform.position += new Vector3(0f, 0.4f, 0f);
        }
        resetTimeAfterChangingAnswer();
    }

    public void moveDown() {
        if (currentPosition == -1) return;
        if (currentPosition <= minPosition) {
            transform.position = endPosition;
            currentPosition = maxPosition;
        }
        else {
            currentPosition--;
            transform.position -= new Vector3(0f, 0.4f, 0f);
        }
        resetTimeAfterChangingAnswer();
    }

    void resetTimeAfterChangingAnswer() {
        isGlowing = false;
        materials[answerMaterialPosition].SetFloat("_FresnelAmount", 2.5f);
        timeAfterChangingAnswer = 0f;
    }

    public bool getCorrectAnswer() {
        if (currentPosition == solutionPosition) return true;
        else return false;
    }

    public void unlock() {
        currentPosition++;
        transform.position += new Vector3(0f, 0.4f, 0f);
    }

    public void lockQuiz() {
        currentPosition = -1;
        transform.position = startPosition - new Vector3(0f, 1.2f, 0f);
    }

    public void reset() {
        currentPosition = -1;
        transform.position = startPosition - new Vector3(0f, 0.8f, 0f);
    }

    public void confirmAnswer() {
        if (currentPosition < 1) return;
        if (currentPosition == solutionPosition) quizHandler.solvedQuiz(id);
        else quizHandler.enteredWrongAnswer();
    }
}
