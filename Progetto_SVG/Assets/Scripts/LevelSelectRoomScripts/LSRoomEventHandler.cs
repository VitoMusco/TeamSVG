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
        if (!GlobalEvents.SpawnedForTheFirstTime) {
            GlobalEvents.SpawnedForTheFirstTime = player.playVoiceLine(firstSpawnClip, firstSpawnClipSubtitles);
        }

        if (LevelHandler.getLevelToPlay() == 2) GlobalEvents.CompletedFirstLevel = true;
        if (LevelHandler.getLevelToPlay() == 3) GlobalEvents.CompletedSecondLevel = true;

        if (GlobalEvents.CompletedFirstLevel && !GlobalEvents.CompletedFirstLevelPlayed) {
            GlobalEvents.CompletedFirstLevelPlayed = player.playVoiceLine(firstLevelCompletion, firstLevelCompletionSubtitles);
        }

        if (GlobalEvents.CompletedSecondLevel && !GlobalEvents.CompletedSecondLevelPlayed)
        {
            GlobalEvents.CompletedSecondLevelPlayed = player.playVoiceLine(secondLevelCompletion, secondLevelCompletionSubtitles);
        }
    }
}
