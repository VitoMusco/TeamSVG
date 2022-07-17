using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSRoomEventHandler : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private AudioClip firstSpawnClip;
    [SerializeField] private string firstSpawnClipSubtitles;

    [SerializeField] private AudioClip firstLevelCompletion;
    [SerializeField] private string firstLevelCompletionSubtitles;

    [SerializeField] private AudioClip secondLevelCompletion;
    [SerializeField] private string secondLevelCompletionSubtitles;

    void Awake() {
        if (GlobalEvents.SpawnedForTheFirstTime) {
            GlobalEvents.SpawnedForTheFirstTime = player.playVoiceLine(firstSpawnClip, firstSpawnClipSubtitles);
        }

        if (GlobalEvents.CompletedFirstLevel) {
            GlobalEvents.CompletedFirstLevel = player.playVoiceLine(firstLevelCompletion, firstLevelCompletionSubtitles);
        }

        if (GlobalEvents.CompletedSecondLevel)
        {
            GlobalEvents.CompletedFirstLevel = player.playVoiceLine(secondLevelCompletion, secondLevelCompletionSubtitles);
        }
    }
}
