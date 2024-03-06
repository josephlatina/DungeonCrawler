/*
 * GameManager.cs
 * Author: Joseph Latina
 * Created: February 02, 2024
 * Description: Script for managing the overall game session (ie. game states, etc)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // used to reload the main scene

/// <summary>
/// Game Manager class that is of singleton mono behaviour type
/// </summary>
[DisallowMultipleComponent] // attribute for preventing us from adding the same component more than once in the game
public class GameManager : SingletonMonoBehavior<GameManager>
{
    // serialized private field for containing the list of dungeon levels for this game session
    #region Header DUNGEON LEVELS
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    #endregion Header DUNGEON LEVELS
    #region Tooltip
    [Tooltip("Populate with the dungeon level scriptable objects")]
    #endregion Tooltip
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    // serialized private field indicating the first dungeon level
    #region Tooltip
    [Tooltip("Populate with the starting dungeon level (for testing)")]
    #endregion Tooltip
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    // private fields
    private Room currentRoom;
    private PlayerController player;


    // public variable for holding the current and previous game state
    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    protected override void Awake()
    {
        // Call base class
        base.Awake();

        player = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerController>();    
    
    }

     private void OnEnable()
    {
        // Subscribe to room changed event.
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    /// <summary>
    /// Handle room changed event
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // set the new room as the current room
        SetCurrentRoom(roomChangedEventArgs.room);

        // if player enters boss room
        if (currentRoom.roomNodeType.isBossRoom) {
            EnterBossRoom();
        }

        // if player enters the exit room, apply logic to determine whether player has level completed or game is won
        if (currentRoom.roomNodeType.isExit) {
            BossRoomEnemyDefeated(currentRoom);
        }
    }

    /// <summary>
    /// Start method - called before the first frame update
    /// </summary>
    private void Start() {
        // keep track of previous and current game states
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted; 
    }

    /// <summary>
    /// Update method - called once per frame
    /// </summary>
    private void Update() {
        // handle each game state
        HandleGameState();

        // For Testing, reset and reload the dungeon level
        // TODO: remove at the end
        if (Input.GetKeyDown(KeyCode.R)) {
            gameState = GameState.gameStarted;
        }
    }

    /// <summary>
    /// Handle different game states
    /// </summary>
    private void HandleGameState() {

        // Handle all the different game states
        switch (gameState) {

            // Game Started State
            case GameState.gameStarted:
                // Play first level
                PlayDungeonLevel(currentDungeonLevelListIndex);
                // Set the current game state to Playing Level
                gameState = GameState.playingLevel;

                break;

            // Level Completed
            case GameState.levelCompleted:
                // Display level completed UI
                StartCoroutine(LevelCompleted());
                break;

            // Game won (only triggered once)
            case GameState.gameWon:
                // check if this is the first time entering this game state
                if (previousGameState != GameState.gameWon) {
                    // if it is, proceed
                    StartCoroutine(GameWon());
                }
                break;

            // Game lost (only triggered once)
            case GameState.gameLost:
                // check if this is the first time entering this game state
                if (previousGameState != GameState.gameLost) {
                    // if it is, stop all currently running coroutines and proceed
                    StopAllCoroutines(); // if you clear the level just as you get killed, prevent the 'level clear' messages from appearing
                    StartCoroutine(GameLost());
                }
                break;

            // Restart the game
            case GameState.restartGame:
                RestartGame();
                break;
        }
    }

    private void EnterBossRoom() {

        // set the game state to boss stage. This should trigger SpawnEnemies() in EnemySpawner script
        gameState = GameState.bossStage;
        StartCoroutine(BossStage());
    }

    /// <summary>
    /// Enter the boss stage - TODO: lock doors
    /// </summary>
    private IEnumerator BossStage()
    {

        // TODO: Room Lock Functionality
       // Unlock boss room
    //    bossRoom.UnlockDoors(0f);
       // Wait 2 seconds
       yield return new WaitForSeconds(2f);

    }

    /// <summary>
    /// Room enemies defeated - if boss is defeated, load next dungeon game level
    /// </summary>
    private void BossRoomEnemyDefeated(Room bossRoom)
    {

        // check if boss has been cleared
        if (bossRoom.isClearedOfEnemies) {

            // if there are more dungeon levels to traverse through
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1) {
                // then set game state to have current level completed
                gameState = GameState.levelCompleted;
            }
            // if we've defeated the boss in the last level,
            else {
                // game has been won
                gameState = GameState.gameWon;
            }
        }
    }

    /// <summary>
    /// Handles game state of level being completed
    /// </summary>
    private IEnumerator LevelCompleted()
    {
        // Play next level
        gameState = GameState.playingLevel;

        // Wait 2 seconds
        yield return new WaitForSeconds(2f);

        Debug.Log("Level Completed - Press Enter to Progress to the Next Level");

        // Wait for player to press return key
        while (!Input.GetKeyDown(KeyCode.Return)) {
            yield return null;
        }

        // to avoid enter being detected twice
        yield return null;

        // Increase the index to next level
        currentDungeonLevelListIndex++;
        
        // pass in next dungeon level 
        PlayDungeonLevel(currentDungeonLevelListIndex);

    }

    /// <summary>
    /// Handles game state of game being won
    /// </summary>
    private IEnumerator GameWon()
    {
        // Set previous state as game won to avoid game won being called multiple times
        previousGameState = GameState.gameWon;

        Debug.Log("Game Won! All levels completed and boss defeated. Game will restart in 10 seconds.");

        // Wait 10 seconds
        yield return new WaitForSeconds(10f);

        // Set game to restart
        gameState = GameState.restartGame;

    }

    /// <summary>
    /// Handles game state of game being lost
    /// </summary>
    private IEnumerator GameLost()
    {
        // Set previous state as game won to avoid game lost being called multiple times
        previousGameState = GameState.gameLost;

        Debug.Log("Game Lost! Better luck next time. Game will restart in 10 seconds.");

        // Wait 10 seconds
        yield return new WaitForSeconds(10f);

        // Set game to restart
        gameState = GameState.restartGame;

    }

    /// <summary>
    /// Handles game state to Game Restart
    /// </summary>
    private void RestartGame()
    {
        // Reload scene
        SceneManager.LoadScene("MainGameScene");

    }

    /// <summary>
    /// Handles the play state for the dungeon level
    /// </summary>
    private void PlayDungeonLevel(int dungeonLevelListIndex) {

        // Try to build dungeon for current level and instantiate it
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        // If not successful, log an error
        if (!dungeonBuiltSuccessfully) {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }

        // Call static event that room has changed due to play start of level (with room being the entrance)
        StaticEventHandler.CallRoomChangedEvent(currentRoom);
    }

    /// <summary>
    /// Get the current dungeon level
    /// </summary>
    public DungeonLevelSO GetCurrentDungeonLevel() {

        return dungeonLevelList[currentDungeonLevelListIndex];
    }

    /// <summary>
    /// Get the current room the player is in
    /// </summary>
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    /// <summary>
    /// Set the current room the player in in
    /// </summary>
    public void SetCurrentRoom(Room room)
    {
        currentRoom = room;
    }

     /// <summary>
    /// Get the player
    /// </summary>
    public PlayerController GetPlayer()
    {
        return player;
    }

    /// <summary>
    /// Set the player
    /// </summary>
    public void SetPlayer(PlayerController currentPlayer)
    {
        player = currentPlayer;
    }

    #region Validation

    // Only to be executed in the unity editor
    #if UNITY_EDITOR

    /// <summary>
    /// Validation method for this class
    /// </summary>
    private void OnValidate() {

        // Check if dungeon level list is populated and not null
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

    #endif

    #endregion Validation
}
