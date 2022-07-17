using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEvents : MonoBehaviour
{
    private static bool playerPlayingVoiceLine = false;

    private static bool spawnedForTheFirstTime = false;

    ///MAZE

    private static bool spawnedInMaze = false;
    private static bool spawnedInMazePlayed = false;

    private static bool usedCompass = false;
    private static bool usedCompassPlayed = false;

    private static bool takenFirstQuiz = false;
    private static bool takenFirstQuizPlayed = false;

    private static bool firstGhostSpawn = false;
    private static bool firstGhostSpawnPlayed = false;

    private static bool firstGhostSlap = false;
    private static bool firstGhostSlapPlayed = false;

    private static bool firstGhostDeath = false;
    private static bool firstGhostDeathPlayed = false;

    private static bool firstWrongAnswer = false;
    private static bool firstWrongAnswerPlayed = false;

    private static bool firstQuizCorrectAnswer = false;
    private static bool firstQuizCorrectAnswerPlayed = false;

    private static bool secondQuizCorrectAnswer = false;
    private static bool secondQuizCorrectAnswerPlayed = false;

    private static bool thirdQuizCorrectAnswer = false;
    private static bool thirdQuizCorrectAnswerPlayed = false;

    private static bool fourthQuizCorrectAnswer = false;
    private static bool fourthQuizCorrectAnswerPlayed = false;

    private static bool fifthQuizCorrectAnswer = false;
    private static bool fifthQuizCorrectAnswerPlayed = false;

    private static bool openedMazeDoor = false;
    private static bool openedMazeDoorPlayed = false;

    private static bool completedFirstLevel = false;
    private static bool completedFirstLevelPlayed = false;

    //PARKOUR

    private static bool spawnedInParkour = false;
    private static bool spawnedInParkourPlayed = false;

    private static bool firstCannonShot = false;
    private static bool firstCannonShotPlayed = false;

    private static bool firstPlatformRide = false;
    private static bool firstPlatformRidePlayed = false;

    private static bool halfParkourCompletion = false;
    private static bool halfParkourCompletionPlayed = false;

    private static bool firstLevitation = false;
    private static bool firstLevitationPlayed = false;

    private static bool seenRotatingAxes = false;
    private static bool seenRotatingAxesPlayed = false;

    private static bool completedSecondLevel = false;
    private static bool completedSecondLevelPlayed = false;

    //COMBAT
    private static bool spawnedInCombat = false;
    private static bool spawnedInCombatPlayed = false;

    private static bool firstGuardianAttack = false;
    private static bool firstGuardianAttackPlayed = false;

    private static bool halfGuardianLife = false;
    private static bool halfGuardianLifePlayed = false;

    private static bool guardianDeath = false;
    private static bool guardianDeathPlayed = false;
    
    private static bool firstPlayerAttack = false;
    private static bool firstPlayerAttackPlayed = false;


    //GettersAndSetters
    public static bool PlayerPlayingVoiceLine { get => playerPlayingVoiceLine; set => playerPlayingVoiceLine = value; }
    public static bool SpawnedForTheFirstTime { get => spawnedForTheFirstTime; set => spawnedForTheFirstTime = value; }
    public static bool SpawnedInMaze { get => spawnedInMaze; set => spawnedInMaze = value; }
    public static bool SpawnedInMazePlayed { get => spawnedInMazePlayed; set => spawnedInMazePlayed = value; }
    public static bool UsedCompass { get => usedCompass; set => usedCompass = value; }
    public static bool UsedCompassPlayed { get => usedCompassPlayed; set => usedCompassPlayed = value; }
    public static bool TakenFirstQuiz { get => takenFirstQuiz; set => takenFirstQuiz = value; }
    public static bool TakenFirstQuizPlayed { get => takenFirstQuizPlayed; set => takenFirstQuizPlayed = value; }
    public static bool FirstGhostSpawn { get => firstGhostSpawn; set => firstGhostSpawn = value; }
    public static bool FirstGhostSpawnPlayed { get => firstGhostSpawnPlayed; set => firstGhostSpawnPlayed = value; }
    public static bool FirstGhostSlap { get => firstGhostSlap; set => firstGhostSlap = value; }
    public static bool FirstGhostSlapPlayed { get => firstGhostSlapPlayed; set => firstGhostSlapPlayed = value; }
    public static bool FirstGhostDeath { get => firstGhostDeath; set => firstGhostDeath = value; }
    public static bool FirstGhostDeathPlayed { get => firstGhostDeathPlayed; set => firstGhostDeathPlayed = value; }
    public static bool FirstWrongAnswer { get => firstWrongAnswer; set => firstWrongAnswer = value; }
    public static bool FirstWrongAnswerPlayed { get => firstWrongAnswerPlayed; set => firstWrongAnswerPlayed = value; }
    public static bool FirstQuizCorrectAnswer { get => firstQuizCorrectAnswer; set => firstQuizCorrectAnswer = value; }
    public static bool FirstQuizCorrectAnswerPlayed { get => firstQuizCorrectAnswerPlayed; set => firstQuizCorrectAnswerPlayed = value; }
    public static bool SecondQuizCorrectAnswer { get => secondQuizCorrectAnswer; set => secondQuizCorrectAnswer = value; }
    public static bool SecondQuizCorrectAnswerPlayed { get => secondQuizCorrectAnswerPlayed; set => secondQuizCorrectAnswerPlayed = value; }
    public static bool ThirdQuizCorrectAnswer { get => thirdQuizCorrectAnswer; set => thirdQuizCorrectAnswer = value; }
    public static bool ThirdQuizCorrectAnswerPlayed { get => thirdQuizCorrectAnswerPlayed; set => thirdQuizCorrectAnswerPlayed = value; }
    public static bool FourthQuizCorrectAnswer { get => fourthQuizCorrectAnswer; set => fourthQuizCorrectAnswer = value; }
    public static bool FourthQuizCorrectAnswerPlayed { get => fourthQuizCorrectAnswerPlayed; set => fourthQuizCorrectAnswerPlayed = value; }
    public static bool FifthQuizCorrectAnswer { get => fifthQuizCorrectAnswer; set => fifthQuizCorrectAnswer = value; }
    public static bool FifthQuizCorrectAnswerPlayed { get => fifthQuizCorrectAnswerPlayed; set => fifthQuizCorrectAnswerPlayed = value; }
    public static bool OpenedMazeDoor { get => openedMazeDoor; set => openedMazeDoor = value; }
    public static bool OpenedMazeDoorPlayed { get => openedMazeDoorPlayed; set => openedMazeDoorPlayed = value; }
    public static bool CompletedFirstLevel { get => completedFirstLevel; set => completedFirstLevel = value; }
    public static bool CompletedFirstLevelPlayed { get => completedFirstLevelPlayed; set => completedFirstLevelPlayed = value; }
    public static bool SpawnedInParkour { get => spawnedInParkour; set => spawnedInParkour = value; }
    public static bool SpawnedInParkourPlayed { get => spawnedInParkourPlayed; set => spawnedInParkourPlayed = value; }
    public static bool FirstCannonShot { get => firstCannonShot; set => firstCannonShot = value; }
    public static bool FirstCannonShotPlayed { get => firstCannonShotPlayed; set => firstCannonShotPlayed = value; }
    public static bool FirstPlatformRide { get => firstPlatformRide; set => firstPlatformRide = value; }
    public static bool FirstPlatformRidePlayed { get => firstPlatformRidePlayed; set => firstPlatformRidePlayed = value; }
    public static bool HalfParkourCompletion { get => halfParkourCompletion; set => halfParkourCompletion = value; }
    public static bool HalfParkourCompletionPlayed { get => halfParkourCompletionPlayed; set => halfParkourCompletionPlayed = value; }
    public static bool FirstLevitation { get => firstLevitation; set => firstLevitation = value; }
    public static bool FirstLevitationPlayed { get => firstLevitationPlayed; set => firstLevitationPlayed = value; }
    public static bool SeenRotatingAxes { get => seenRotatingAxes; set => seenRotatingAxes = value; }
    public static bool SeenRotatingAxesPlayed { get => seenRotatingAxesPlayed; set => seenRotatingAxesPlayed = value; }
    public static bool CompletedSecondLevel { get => completedSecondLevel; set => completedSecondLevel = value; }
    public static bool CompletedSecondLevelPlayed { get => completedSecondLevelPlayed; set => completedSecondLevelPlayed = value; }
    public static bool SpawnedInCombat { get => spawnedInCombat; set => spawnedInCombat = value; }
    public static bool SpawnedInCombatPlayed { get => spawnedInCombatPlayed; set => spawnedInCombatPlayed = value; }
    public static bool FirstGuardianAttack { get => firstGuardianAttack; set => firstGuardianAttack = value; }
    public static bool FirstGuardianAttackPlayed { get => firstGuardianAttackPlayed; set => firstGuardianAttackPlayed = value; }
    public static bool HalfGuardianLife { get => halfGuardianLife; set => halfGuardianLife = value; }
    public static bool HalfGuardianLifePlayed { get => halfGuardianLifePlayed; set => halfGuardianLifePlayed = value; }
    public static bool GuardianDeath { get => guardianDeath; set => guardianDeath = value; }
    public static bool GuardianDeathPlayed { get => guardianDeathPlayed; set => guardianDeathPlayed = value; }
    public static bool FirstPlayerAttack { get => firstPlayerAttack; set => firstPlayerAttack = value; }
    public static bool FirstPlayerAttackPlayed { get => firstPlayerAttackPlayed; set => firstPlayerAttackPlayed = value; }
}
