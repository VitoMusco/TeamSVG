using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndHandler : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(fade());
    }

    IEnumerator fade()
    {
        float timeElapsed = 2f;
        float timeToReachPeak = 2f;
        Color color = Color.black;

        while (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            color.a = timeElapsed / timeToReachPeak;
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0;
        fadeImage.color = color;
        fadeImage.enabled = false;
    }
}
