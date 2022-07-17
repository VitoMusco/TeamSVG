using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourEventHandler : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    [SerializeField] private AudioClip firstParkourSpawnClip;
    [SerializeField] private string firstParkourSpawnClipSubtitles;

    [SerializeField] private AudioClip cannonShotClip;
    [SerializeField] private string cannonShotClipSubtitles;

    [SerializeField] private AudioClip platformRideClip;
    [SerializeField] private string platformRideClipSubtitles;

    [SerializeField] private AudioClip halfParkourCompletionClip;
    [SerializeField] private string halfParkourCompletionClipSubtitles;

    [SerializeField] private AudioClip levitationClip;
    [SerializeField] private string levitationClipSubtitles;

    [SerializeField] private AudioClip rotatingAxesClip;
    [SerializeField] private string rotatingAxesClipSubtitles;

    void Awake() {
        if (!GlobalEvents.SpawnedInParkour)
        {
            GlobalEvents.SpawnedInParkourPlayed = player.playVoiceLine(firstParkourSpawnClip, firstParkourSpawnClipSubtitles);
        }
    }

    void Update()
    {
        handleVoiceLines();
    }

    void handleVoiceLines() {
        if (GlobalEvents.FirstCannonShot && !GlobalEvents.FirstCannonShotPlayed)
        {
            GlobalEvents.FirstCannonShotPlayed = player.playVoiceLine(cannonShotClip, cannonShotClipSubtitles);
        }
        if (GlobalEvents.FirstPlatformRide && !GlobalEvents.FirstPlatformRidePlayed)
        {
            GlobalEvents.FirstPlatformRidePlayed = player.playVoiceLine(platformRideClip, platformRideClipSubtitles);
        }
        if (GlobalEvents.HalfParkourCompletion && !GlobalEvents.HalfParkourCompletionPlayed)
        {
            GlobalEvents.HalfParkourCompletionPlayed = player.playVoiceLine(halfParkourCompletionClip, halfParkourCompletionClipSubtitles);
        }
        if (GlobalEvents.FirstLevitation && !GlobalEvents.FirstLevitationPlayed)
        {
            GlobalEvents.FirstLevitationPlayed = player.playVoiceLine(levitationClip, levitationClipSubtitles);
        }
        if (GlobalEvents.SeenRotatingAxes && !GlobalEvents.SeenRotatingAxesPlayed)
        {
            GlobalEvents.SeenRotatingAxesPlayed = player.playVoiceLine(rotatingAxesClip, rotatingAxesClipSubtitles);
        }
    }
}
