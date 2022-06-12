using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    public float swaySmooth = 1f;
    public float swayMultiplier = 1f;
    public bool shake = false;
    public Transform playerBody;
    public Transform playerArms;
    Quaternion swayRotationX;
    Quaternion swayRotationY;
    Quaternion targetRotation;

    float yRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        swayRotationX = Quaternion.AngleAxis(-mouseY * swayMultiplier * swaySmooth, Vector3.right);
        swayRotationY = Quaternion.AngleAxis(mouseX * swayMultiplier * swaySmooth, Vector3.up);

        targetRotation = swayRotationX * swayRotationY;

        playerArms.localRotation = Quaternion.Slerp(playerArms.localRotation, targetRotation, swaySmooth * Time.deltaTime);

        mouseX = mouseX * mouseSensitivity * Time.deltaTime;
        mouseY = mouseY * mouseSensitivity * Time.deltaTime;

        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public IEnumerator shakeCamera(float shakingTime, float shakeMultiplier, float rotationMultiplier, float shakeRange, float rotationRange) {
        float timeElapsed = 0;
        float timeToGenerateNewPosition = 0.1f;
        float timeSinceLastPosition = 0f;
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;
        float startRotationY = transform.localRotation.y;
        Vector3 positionToReach = new Vector3(startPosition.x + Random.Range(-shakeRange, shakeRange) * shakeMultiplier, startPosition.y + Random.Range(-shakeRange, shakeRange) * shakeMultiplier, startPosition.z);
        float rotationToReach = Random.Range(-rotationRange, rotationRange) * rotationMultiplier;
        while (timeElapsed < shakingTime) {
            timeElapsed += Time.deltaTime;
            if (timeSinceLastPosition < timeToGenerateNewPosition) {
                timeSinceLastPosition += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(startPosition, positionToReach, timeSinceLastPosition / timeToGenerateNewPosition);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, new Quaternion(transform.localRotation.x, transform.localRotation.y, startRotationY + rotationToReach, transform.localRotation.w), timeSinceLastPosition / timeToGenerateNewPosition);
            }
            if (timeSinceLastPosition >= timeToGenerateNewPosition) {
                positionToReach = new Vector3(startPosition.x + Random.Range(-shakeRange, shakeRange) * shakeMultiplier, startPosition.y + Random.Range(-shakeRange, shakeRange) * shakeMultiplier, startPosition.z);
                rotationToReach = Random.Range(-rotationRange, rotationRange) * rotationMultiplier;
                timeSinceLastPosition = 0f;
            }

            yield return null;
        }

        transform.localPosition = startPosition;
        transform.localRotation = startRotation;
    }
}
