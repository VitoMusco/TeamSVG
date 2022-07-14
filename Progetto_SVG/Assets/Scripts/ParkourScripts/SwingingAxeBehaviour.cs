using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingAxeBehaviour : MonoBehaviour
{
    [SerializeField, Range(0f, 360f)] private float angle = 90f;
    [SerializeField, Range(0f, 5f)] private float speed = 2f;
    [SerializeField, Range(0f, 10f)] private float startTime = 0f;
    Quaternion start, end;

    void Update()
    {
        rotateAxe();
    }

    void Awake() {
        start = axeRotation(angle);
        end = axeRotation(-angle);
    }

    void resetTimer() {
        startTime = 0f;
    }

    Quaternion axeRotation(float angle) {
        Quaternion axeRotation = transform.rotation;
        float angleZ = axeRotation.z + angle;

        if (angleZ > 180) angleZ -= 360;
        else if (angleZ < -180) angleZ += 360;

        axeRotation.eulerAngles = new Vector3(axeRotation.eulerAngles.x, axeRotation.eulerAngles.y, angleZ);
        return axeRotation;
    }

    void rotateAxe() {
        startTime += Time.deltaTime;
        transform.rotation = Quaternion.Lerp(start, end, (Mathf.Sin(startTime * speed + Mathf.PI / 2) + 1f) / 2f);
    }
}
