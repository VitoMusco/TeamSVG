using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSRoomEventHandler : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private AudioClip firstSpawnClip;
    [SerializeField] private string firstSpawnClipSubtitles;

    void Awake() {
        if (!GlobalEvents.SpawnedForTheFirstTime) {
            player.playVoiceLine(firstSpawnClip, firstSpawnClipSubtitles);
            GlobalEvents.SpawnedForTheFirstTime = true;
        }
    }
}
