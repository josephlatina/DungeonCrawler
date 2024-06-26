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
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Tilemaps;

/// <summary>
/// Game Manager class that is of singleton mono behaviour type
/// </summary>
[DisallowMultipleComponent] // attribute for preventing us from adding the same component more than once in the game
public class GameManager : SingletonMonoBehavior<GameManager>
{
    // Game object references
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("GAMEOBJECT REFERENCES")]
    #endregion Header GAMEOBJECT REFERENCES

    #region Tooltip
    [Tooltip("Populate with pause menu gameobject in hierarchy")]
    #endregion Tooltip
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject playerStatsScreen;
    [SerializeField] private GameObject instructionsScreen;


    #region Tooltip
    [Tooltip("Populate with the MessageText TMPro component in the FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI messageTextTMP;
    [SerializeField] private TextMeshProUGUI teethValue;
    [SerializeField] private TextMeshProUGUI strengthValue;
    [SerializeField] private TextMeshProUGUI defenceValue;
    [SerializeField] private TextMeshProUGUI attackSpeedValue;
    [SerializeField] private GameObject heartDisplay;

     #region Tooltip
    [Tooltip("Populate with the FadeImage canvasgroup component in the FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private CanvasGroup canvasGroup;

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
    private Boolean isBossDefeated;


    // public variable for holding the current and previous game state
    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    private GameObject continueText;
    private GameObject backText;

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

        // if player enters the exit room, apply logic to determine whether player has level completed or game is won
        if (currentRoom.roomNodeType.isExit) {
            ExitRoomCheck();
        }
    }

    /// <summary>
    /// Start method - called before the first frame update
    /// </summary>
    private void Start()
    {
        // keep track of previous and current game states
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted; 

        // Instruction Screen
        continueText = FindChildGameObject(instructionsScreen.transform, "ContinueButton");
        backText = FindChildGameObject(instructionsScreen.transform, "BackButton");

        // set screen to black
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
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

            // While playing the current level,
            case GameState.playingLevel:
                
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    PauseGameMenu();
                }
                break;

            // While engaging the boss,
            case GameState.bossStage:
                
                // check if boss has been defeated
                BossRoomEnemyDefeated(currentRoom);

                if (Input.GetKeyDown(KeyCode.Escape)) {
                    PauseGameMenu();
                }
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

            // While in paused mode,
            case GameState.gamePaused:
                
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    PauseGameMenu();
                }
                break;
        }
    }

    /// <summary>
    /// Enter the boss room
    /// </summary>
    public void EnterBossRoom() {

        StartCoroutine(BossStage());
    }

    /// <summary>
    /// Pause game menu
    /// </summary>
    public void PauseGameMenu() {

        // disable player movement and surface the pause menu
        if (gameState != GameState.gamePaused) {
            pauseMenu.SetActive(true);
            player.GetComponent<PlayerInput>().enabled = false;
            player.GetComponent<Collider2D>().enabled = false;

            // Set game state
            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }
        // otherwise if already paused, then unpause game
        else if (gameState == GameState.gamePaused) {
            // enable player movement again and hide the pause menu
            pauseMenu.SetActive(false);
            playerStatsScreen.SetActive(false);
            instructionsScreen.SetActive(false);
            player.GetComponent<PlayerInput>().enabled = true;
            player.GetComponent<Collider2D>().enabled = true;

            // Set game state
            gameState = previousGameState;
            previousGameState = GameState.gamePaused;
        }
    }

    /// <summary>
    /// Show Pause Menu Sub Item
    /// </summary>
    public void ShowPauseMenuSubItem(GameObject previousScreen, GameObject nextScreen) {

        previousScreen.SetActive(false);
        nextScreen.SetActive(true);
    }

    /// <summary>
    /// Player Stats Screen
    /// </summary>
    public void ShowPlayerStatsScreen() {
        ShowPauseMenuSubItem(pauseMenu, playerStatsScreen);
        DisplayStats();
    }
    public void UnshowPlayerStatsScreen() {
        ShowPauseMenuSubItem(playerStatsScreen, pauseMenu);
    }

    /// <summary>
    /// Instructions Screen
    /// </summary>
    public void ShowInstructionsScreen() {
        ShowPauseMenuSubItem(pauseMenu, instructionsScreen);
    }
    public void UnshowInstructionsScreen() {
        ShowPauseMenuSubItem(instructionsScreen, pauseMenu);
    }
    public void ShowStartInstructionsScreen() {
        if (currentDungeonLevelListIndex == 0) {
            if (instructionsScreen.activeSelf) {
                instructionsScreen.SetActive(false);
                continueText.SetActive(false);
                backText.SetActive(true);
                // Display Dungeon Level Text
                StartCoroutine(DisplayDungeonLevelText());
            }
            else {
                continueText.SetActive(true);
                backText.SetActive(false);
                instructionsScreen.SetActive(true);
            }
        } else 
        {
            // Display Dungeon Level Text
            StartCoroutine(DisplayDungeonLevelText());
        }
    }


    /// <summary>
    /// Find child object
    /// </summary>
    private GameObject FindChildGameObject(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }

            GameObject foundChild = FindChildGameObject(child, childName);
            if (foundChild != null)
            {
                return foundChild;
            }
        }
        return null;
    }

    /// <summary>
    /// Enter the boss stage - TODO: lock doors
    /// </summary>
    private IEnumerator BossStage()
    {
        gameState = GameState.bossStage;
        // Ensure player does not escape room until boss is defeated
        Tilemap tilemapObject = currentRoom.instantiatedRoom.bossCollisionTilemap;
        TilemapCollider2D tilemapCollider = tilemapObject.GetComponent<TilemapCollider2D>();

        if (tilemapCollider != null)
        {
            tilemapCollider.enabled = true;
        }
       // Wait 2 seconds
       yield return new WaitForSeconds(2f);

    }

    /// <summary>
    /// Room enemies defeated - if boss is defeated, load next dungeon game level
    /// </summary>
    private void BossRoomEnemyDefeated(Room bossRoom)
    {
        // check if boss has been cleared
        if (bossRoom.isClearedOfBoss && !isBossDefeated) {

            isBossDefeated = true;

            Tilemap tilemapObject = currentRoom.instantiatedRoom.bossCollisionTilemap;
            TilemapCollider2D tilemapCollider = tilemapObject.GetComponent<TilemapCollider2D>();

            if (tilemapCollider != null)
            {
                tilemapCollider.enabled = false;
            }
        }
    }

    // <summary>
    /// Check if prerequisite is cleared
    /// </summary>
    private void ExitRoomCheck()
    {

        // check if boss has been cleared
        if (isBossDefeated) {

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

        // Fade in canvas to display the text message
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // Display level completed
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE \n\n YOU'VE SURVIVED \n\n THIS DUNGEON LEVEL", Color.white, 2f));
        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO DESCEND \n\n FURTHER INTO THE \n\n NEXT LEVEL", Color.white, 5f));

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
    /// Fade Canvas Group
    /// </summary>
    private IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        // Modify the image component
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;

        while (time <= fadeSeconds) {
            // increment time
            time += Time.deltaTime;
            // set the alpha value gradually as time increments
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
    }

    /// <summary>
    /// Fade Canvas Group
    /// </summary>
    private IEnumerator SurfaceUI(CanvasGroup canvas, float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        // Modify the image component
        Image image = canvas.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;

        while (time <= fadeSeconds) {
            // increment time
            time += Time.deltaTime;
            // set the alpha value gradually as time increments
            canvas.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
    }

    /// <summary>
    /// Display the message text in x display seconds
    /// </summary>
    public void DisplayStats() {

        // set the string
        teethValue.SetText(player.player.CurrentCurrency.ToString());
        strengthValue.SetText(player.player.CurrentStrength.ToString());
        defenceValue.SetText(player.player.CurrentDefence.ToString());
        attackSpeedValue.SetText(player.player.CurrentAttackSpeed.ToString());

        PlayerHealthDisplay playerHealthDisplay = heartDisplay.GetComponent<PlayerHealthDisplay>();
        playerHealthDisplay.DrawHearts();
    }


    /// <summary>
    /// Handles game state of game being won
    /// </summary>
    private IEnumerator GameWon()
    {
        // Set previous state as game won to avoid game won being called multiple times
        previousGameState = GameState.gameWon;

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // Display level completed
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE \n\n YOU HAVE DEFEATED THE DUNGEON", Color.white, 3f));
        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO \n\n GO BACK TO MAIN MENU", Color.white, 5f));

        // Go back to main menu
        SceneManager.LoadScene("Home");

    }

    /// <summary>
    /// Handles game state of game being lost
    /// </summary>
    private IEnumerator GameLost()
    {
        // Set previous state as game won to avoid game lost being called multiple times
        previousGameState = GameState.gameLost;

        // Wait 1 second
        yield return new WaitForSeconds(1f);

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // Display level completed
        yield return StartCoroutine(DisplayMessageRoutine("YOU HAVE UNFORTUNATELY SUCCUMBED TO THE DARKNESS OF THE SANITORIUM", Color.white, 2f));
        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO \n\n GO BACK TO MAIN MENU", Color.white, 5f));

        // Go back to main menu
        SceneManager.LoadScene("Home");;

    }

    /// <summary>
    /// Handles game state to Game Restart
    /// </summary>
    public void RestartGame()
    {
        // Reload scene
        SceneManager.LoadScene("MainGameScene");

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

        // Set player roughly mid-room
        player.gameObject.transform.position = new Vector3(currentRoom.lowerBounds.x + ((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 3f), (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // set boolean to false
        isBossDefeated = false;

        // Display Instruction Screen if first level
        ShowStartInstructionsScreen();
    }

    /// <summary>
    /// Display the dungeon level text
    /// </summary>
    public IEnumerator DisplayDungeonLevelText() {

        // set screen to black
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
        
        // set the string
        string messageText = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + "\n\n" + dungeonLevelList[currentDungeonLevelListIndex].levelName.ToUpper();

        // display the string
        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        // Fade In to the game scene
        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }

    /// <summary>
    /// Display the message text in x display seconds
    /// </summary>
    public IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds) {

        // Set the text
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        // Display the message for the specified time
        if (displaySeconds > 0f) {
            
            float timer = displaySeconds;

            // while the user has not pressed the return key, decrement the timer
            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return)) {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        // else if no displaySeconds specified, display the message text until the return button is pressed (second option)
        else {
            while (!Input.GetKeyDown(KeyCode.Return)) {
                yield return null;
            }
        }

        yield return null;

        // Clear the text
        messageTextTMP.SetText("");
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

    /// <summary>
    /// Spawn an item
    /// </summary>
    public void SpawnItem(Vector3 spawnPosition) {
        // Instantiate the item
        ConsumableItemController consumableItemController = GameResources.Instance.consumablePrefab.GetComponent<ConsumableItemController>();
        GameObject lootItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, spawnPosition, Quaternion.identity);
        lootItemGameObject.tag = "interactableObject";
        ChestItem lootItem = lootItemGameObject.GetComponent<ChestItem>();

        // Instantiate teeth
        ConsumableItemController itemController = lootItemGameObject.AddComponent<ConsumableItemController>();
        itemController.priceView = consumableItemController.priceView;
        itemController.priceText = consumableItemController.priceText;
        itemController.item = GameResources.Instance.currencySO;
        itemController.sprite = consumableItemController.sprite;
        lootItemGameObject.AddComponent<CircleCollider2D>();

        lootItem.Initialize(itemController.item.itemSprite, spawnPosition, Color.yellow);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Home");
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
