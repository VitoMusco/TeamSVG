using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEvents : MonoBehaviour
{
    private static bool playerPlayingVoiceLine = false;

    private static bool spawnedForTheFirstTime = false;

    ///MAZE

    private static bool spawnedInMaze = false;
    private static bool usedCompass = false;
    private static bool takenFirstQuiz = false;
    private static bool firstGhostSpawn = false;
    private static bool firstGhostSlap = false;
    private static bool firstGhostDeath = false;
    private static bool firstWrongAnswer = false;
    private static bool firstQuizCorrectAnswer = false;
    private static bool secondQuizCorrectAnswer = false;
    private static bool thirdQuizCorrectAnswer = false;
    private static bool fourthQuizCorrectAnswer = false;
    private static bool fifthQuizCorrectAnswer = false;
    private static bool openedMazeDoor = false;
    private static bool completedFirstLevel = false;

    //PARKOUR

    private static bool spawnedInParkour = false;
    private static bool firstCannonShot = false;
    private static bool firstPlatformRide = false;
    private static bool halfParkourCompletion = false;
    private static bool firstLevitation = false;
    private static bool seenRotatingAxes = false;
    private static bool completedSecondLevel = false;

    //COMBAT
    private static bool spawnedInCombat = false;
    private static bool firstGuardianAttack = false;
    private static bool halfGuardianLife = false;
    private static bool guardianDeath = false;
    private static bool firstPlayerAttack = false;

    //GettersAndSetters

    public static bool PlayerPlayingVoiceLine { get => playerPlayingVoiceLine; set => playerPlayingVoiceLine = value; }
    public static bool SpawnedForTheFirstTime { get => spawnedForTheFirstTime; set => spawnedForTheFirstTime = value; }

    //MAZE

    public static bool SpawnedInMaze { get => spawnedInMaze; set => spawnedInMaze = value; }
    public static bool UsedCompass { get => usedCompass; set => usedCompass = value; }
    public static bool TakenFirstQuiz { get => takenFirstQuiz; set => takenFirstQuiz = value; }
    public static bool FirstGhostSpawn { get => firstGhostSpawn; set => firstGhostSpawn = value; }
    public static bool FirstGhostSlap { get => firstGhostSlap; set => firstGhostSlap = value; }
    public static bool FirstGhostDeath { get => firstGhostDeath; set => firstGhostDeath = value; }
    public static bool FirstWrongAnswer { get => firstWrongAnswer; set => firstWrongAnswer = value; }
    public static bool FirstQuizCorrectAnswer { get => firstQuizCorrectAnswer; set => firstQuizCorrectAnswer = value; }
    public static bool SecondQuizCorrectAnswer { get => secondQuizCorrectAnswer; set => secondQuizCorrectAnswer = value; }
    public static bool ThirdQuizCorrectAnswer { get => thirdQuizCorrectAnswer; set => thirdQuizCorrectAnswer = value; }
    public static bool FourthQuizCorrectAnswer { get => fourthQuizCorrectAnswer; set => fourthQuizCorrectAnswer = value; }
    public static bool FifthQuizCorrectAnswer { get => fifthQuizCorrectAnswer; set => fifthQuizCorrectAnswer = value; }
    public static bool OpenedMazeDoor { get => openedMazeDoor; set => openedMazeDoor = value; }
    public static bool CompletedFirstLevel { get => completedFirstLevel; set => completedFirstLevel = value; }

    //PARKOUR

    public static bool SpawnedInParkour { get => spawnedInParkour; set => spawnedInParkour = value; }
    public static bool FirstCannonShot { get => firstCannonShot; set => firstCannonShot = value; }
    public static bool FirstPlatformRide { get => firstPlatformRide; set => firstPlatformRide = value; }
    public static bool HalfParkourCompletion { get => halfParkourCompletion; set => halfParkourCompletion = value; }
    public static bool FirstLevitation { get => firstLevitation; set => firstLevitation = value; }
    public static bool SeenRotatingAxes { get => seenRotatingAxes; set => seenRotatingAxes = value; }
    public static bool CompletedSecondLevel { get => completedSecondLevel; set => completedSecondLevel = value; }

    //COMBAT

    public static bool SpawnedInCombat { get => spawnedInCombat; set => spawnedInCombat = value; }
    public static bool FirstGuardianAttack { get => firstGuardianAttack; set => firstGuardianAttack = value; }
    public static bool HalfGuardianLife { get => halfGuardianLife; set => halfGuardianLife = value; }
    public static bool GuardianDeath { get => guardianDeath; set => guardianDeath = value; }
    public static bool FirstPlayerAttack { get => firstPlayerAttack; set => firstPlayerAttack = value; }
}
