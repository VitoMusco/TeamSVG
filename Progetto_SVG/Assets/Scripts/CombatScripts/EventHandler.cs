using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public Transform gate;
    public Transform gateCollider;
    public GuardianController guardian;
    public PlayerController player;
    public PortalBehaviour portal;
    public ParticleSystem portalParticles;

    [SerializeField] private BoxCollider gateCrossDetector;
    [SerializeField] private bool hasCrossedGate = false;
    [SerializeField] private bool guardianHasBeenKilled = false;
    [SerializeField] private float timeToMoveGate = 3f;

    [SerializeField] private AudioClip firstSpawnClip;
    [SerializeField] private string firstSpawnClipSubtitles;

    [SerializeField] private AudioClip guardianAttackClip;
    [SerializeField] private string guardianAttackClipSubtitles;

    [SerializeField] private AudioClip halfGuardianLifeClip;
    [SerializeField] private string halfGuardianLifeClipSubtitles;

    [SerializeField] private AudioClip guardianDeathClip;
    [SerializeField] private string guardianDeathClipSubtitles;

    [SerializeField] private AudioClip playerAttackClip;
    [SerializeField] private string playerAttackClipSubtitles;

    [SerializeField] private AudioClip playerDefenseClip;
    [SerializeField] private string playerDefenseClipSubtitles;

    void Awake() {
        Invoke(nameof(playStartVoiceLine), 3f);
        gateCrossDetector = GetComponent<BoxCollider>();
    }

    void playStartVoiceLine() {
        if (!GlobalEvents.SpawnedInCombat)
        {
            GlobalEvents.SpawnedInCombatPlayed = player.playVoiceLine(firstSpawnClip, firstSpawnClipSubtitles);
        }
    }

    void Update() {
        handleVoiceLines();
        if (!player.checkIfAlive() && !guardianHasBeenKilled && !GlobalEvents.GuardianDeath) {
            gate.position = new Vector3(gate.position.x, -2.6f, gate.position.z);
            gateCollider.position = new Vector3(gateCollider.position.x, -2.6f, gateCollider.position.z);
            hasCrossedGate = false;
            guardian.resetGuardian();
        }
    }

    void OnTriggerEnter(Collider collidedObject) {
        if (collidedObject.gameObject.tag == "Player" && !hasCrossedGate) {
            guardian.activateGuardian();
            if(!GlobalEvents.CrossedGate) GlobalEvents.CrossedGate = true;
            StartCoroutine(operateGate());
        }
    }

    IEnumerator operateGate() {
        if (!GlobalEvents.GuardianDeath) {
            float timeElapsed = 0f;
            float desiredYPosition;
            if (!hasCrossedGate)
            {
                desiredYPosition = 1.4f;
            }
            else desiredYPosition = -2.6f;
            gateCollider.position = new Vector3(gateCollider.position.x, desiredYPosition, gateCollider.position.z);
            Vector3 startPosition = gate.position;
            Vector3 desiredPosition = new Vector3(startPosition.x, desiredYPosition, startPosition.z);

            while (timeElapsed < timeToMoveGate)
            {
                timeElapsed += Time.deltaTime;
                gate.position = Vector3.Lerp(startPosition, desiredPosition, timeElapsed / timeToMoveGate);
                yield return null;
            }
            if (!hasCrossedGate)
                hasCrossedGate = true;
            gate.position = desiredPosition;
        }
    }

    public void setGuardianKilled() {
        guardianHasBeenKilled = true;
        portal.enable();
        StartCoroutine(operateGate());
    }

    void handleVoiceLines() {
        if (GlobalEvents.FirstGuardianAttack && !GlobalEvents.FirstGuardianAttackPlayed)
        {
            GlobalEvents.FirstGuardianAttackPlayed = player.playVoiceLine(guardianAttackClip, guardianAttackClipSubtitles);
        }
        if (GlobalEvents.HalfGuardianLife && !GlobalEvents.HalfGuardianLifePlayed)
        {
            GlobalEvents.HalfGuardianLifePlayed = player.playVoiceLine(halfGuardianLifeClip, halfGuardianLifeClipSubtitles);
        }
        if (GlobalEvents.GuardianDeath && !GlobalEvents.GuardianDeathPlayed)
        {
            GlobalEvents.GuardianDeathPlayed = player.playVoiceLine(guardianDeathClip, guardianDeathClipSubtitles);
        }
        if (GlobalEvents.FirstPlayerAttack && !GlobalEvents.FirstPlayerAttackPlayed)
        {
            GlobalEvents.FirstPlayerAttackPlayed = player.playVoiceLine(playerAttackClip, playerAttackClipSubtitles);
        }
        if (GlobalEvents.FirstPlayerDefense && !GlobalEvents.FirstPlayerDefensePlayed)
        {
            GlobalEvents.FirstPlayerDefensePlayed = player.playVoiceLine(playerDefenseClip, playerDefenseClipSubtitles);
        }
    }

}
