using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public Transform gate;
    public Transform gateCollider;
    public GuardianController guardian;
    public PlayerController player;
    public MeshRenderer portal;
    public BoxCollider portalCollider;
    public ParticleSystem portalParticles;

    [SerializeField] private BoxCollider gateCrossDetector;
    [SerializeField] private bool hasCrossedGate = false;
    [SerializeField] private bool guardianHasBeenKilled = false;
    [SerializeField] private float timeToMoveGate = 3f;

    void Awake() {
        gateCrossDetector = GetComponent<BoxCollider>();
    }

    void Update() {
        if (!player.checkIfAlive() && !guardianHasBeenKilled) {
            gate.position = new Vector3(gate.position.x, -2.6f, gate.position.z);
            hasCrossedGate = false;
            guardian.resetGuardian();
        }
    }

    void OnTriggerEnter(Collider collidedObject) {
        if (collidedObject.gameObject.tag == "Player" && !hasCrossedGate) {
            guardian.activateGuardian();
            StartCoroutine(operateGate());
        }
    }

    IEnumerator operateGate() {
        float timeElapsed = 0f;
        float desiredYPosition;
        if (!hasCrossedGate) {
            desiredYPosition = 1.4f;
        }
        else desiredYPosition = -2.6f;
        gateCollider.position = new Vector3(gateCollider.position.x, desiredYPosition, gateCollider.position.z);
        Vector3 startPosition = gate.position;
        Vector3 desiredPosition = new Vector3(startPosition.x, desiredYPosition, startPosition.z);

        while (timeElapsed < timeToMoveGate) {
            timeElapsed += Time.deltaTime;
            gate.position = Vector3.Lerp(startPosition, desiredPosition, timeElapsed / timeToMoveGate);
            yield return null;
        }
        if(!hasCrossedGate)
            hasCrossedGate = true;
        gate.position = desiredPosition;
    }

    public void setGuardianKilled() {
        guardianHasBeenKilled = true;
        portal.enabled = true;
        portalCollider.enabled = true;
        portalParticles.Play();
        StartCoroutine(operateGate());
    }

}
