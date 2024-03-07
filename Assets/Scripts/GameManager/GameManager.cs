/*
 * GameManager.cs
 * Author: Joseph Latina
 * Created: February 02, 2024
 * Description: Script for managing the overall game session (ie. game states, etc)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // private field for indicating current room the player is on
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
    }

    /// <summary>
    /// Start method - called before the first frame update
    /// </summary>
    private void Start()
    {
        // keep track of previous and current game states
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;
    }

    /// <summary>
    /// Update method - called once per frame
    /// </summary>
    private void Update()
    {
        // handle each game state
        HandleGameState();

        // For Testing, reset and reload the dungeon level
        // TODO: remove at the end
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
    }

    /// <summary>
    /// Handle different game states
    /// </summary>
    private void HandleGameState()
    {

        // Handle all the different game states
        switch (gameState)
        {

            // Game Started State
            case GameState.gameStarted:
                // Play first level
                PlayDungeonLevel(currentDungeonLevelListIndex);
                // Set the current game state to Playing Level
                gameState = GameState.playingLevel;
                break;
        }
    }

    /// <summary>
    /// Handles the play state for the dungeon level
    /// </summary>
    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {

        // Try to build dungeon for current level and instantiate it
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        // If not successful, log an error
        if (!dungeonBuiltSuccessfully)
        {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }

        // Call static event that room has changed due to play start of level (with room being the entrance)
        StaticEventHandler.CallRoomChangedEvent(currentRoom);
    }

    /// <summary>
    /// Get the current dungeon level
    /// </summary>
    public DungeonLevelSO GetCurrentDungeonLevel()
    {

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

    public void QuitGame()
    {
        SceneManager.LoadScene("Home");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    #region Validation

    // Only to be executed in the unity editor
#if UNITY_EDITOR

    /// <summary>
    /// Validation method for this class
    /// </summary>
    private void OnValidate()
    {

        // Check if dungeon level list is populated and not null
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

#endif

    #endregion Validation
}
