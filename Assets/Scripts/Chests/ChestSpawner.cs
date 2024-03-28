using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [System.Serializable]
    private struct RangeByLevel {

        // reference to the current dungeon level
        public DungeonLevelSO dungeonLevel;
        // min and max spawn chance values
        [Range(0, 100)] public int min;
        [Range(0, 100)] public int max;
    }

    // CHEST PREFAB DETAILS
    #region Header CHEST PREFAB
    [Space(10)]
    [Header("CHEST PREFAB")]
    #endregion Header CHEST PREFAB
    #region Tooltip
    [Tooltip("Populate with the chest prefab")]
    #endregion
    // reference to the chest prefab game object
    [SerializeField] private GameObject chestPrefab;

    #region Header CHEST SPAWN CHANCE
    [Space(10)]
    [Header("CHEST SPAWN CHANCE")]
    #endregion Header CHEST SPAWN CHANCE
    #region Tooltip
    [Tooltip("The minimum probability for spawning a chest")]
    #endregion
    // represents the min spawn chance of chest
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMin;

    // represents the min spawn chance of chest
    #region Tooltip
    [Tooltip("The maximum probability for spawning a chest")]
    #endregion
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMax;

    // can override the chest spawn chance by dungeon level
    #region Tooltip
    [Tooltip("You can override the chest spawn chance by dungeon level")]
    #endregion Tooltip
    [SerializeField] private List<RangeByLevel> chestSpawnChanceByLevelList;

    // CHEST SPAWN DETAILS
    #region Header CHEST SPAWN DETAILS
    [Space(10)]
    [Header("CHEST SPAWN DETAILS")]
    #endregion Header CHEST SPAWN DETAILS
    [SerializeField] private ChestSpawnEvent chestSpawnEvent;
    [SerializeField] private ChestSpawnPosition chestSpawnPosition;

    // CHEST CONTENT DETAILS
    #region Header CHEST CONTENT DETAILS
    [Space(10)]
    [Header("CHEST CONTENT DETAILS")]
    #endregion Header CHEST CONTENT DETAILS
    
    // possible weapons to spawn for each dungeon level and their spawn ratios
    #region Tooltip
    [Tooltip("The weapons to spawn for each dungeon level and their spawn ratios")]
    #endregion Tooltip
    [SerializeField] private List<SpawnableObjectsByLevel<GameObject>> weaponSpawnByLevelList;

    // member variable for keeping track if we've spawned chest
    private bool chestSpawned = false;
    // reference to the room object that this chest is in
    private Room chestRoom;

    private void OnEnable()
    {
        // Subscribe to room changed event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        // Subscribe to room enemies defeated event
        StaticEventHandler.OnBossDefeated += StaticEventHandler_OnBossDefeated;
    }

    private void OnDisable()
    {
        // Unsubscribe from room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        // Unsubscribe from room enemies defeated event
        StaticEventHandler.OnBossDefeated -= StaticEventHandler_OnBossDefeated;
    }

    /// <summary>
    /// Subscriber method for On Room Changed Event that requires the current room object as parameter
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // Get the room the chest is in if we don't already have it
        if (chestRoom == null)
        {
            // get the room property from the 'InstantiatedRoom' component of the room template object
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        // If the chest being spawned is on the room the player just entered (arg passed in subscriber event), then spawn chest
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onRoomEntry && chestRoom == roomChangedEventArgs.room)
        {
            SpawnChest();
        }
    }

    /// <summary>
    /// Subscriber method for On Room Enemies Defeated Event that requires the current room object as parameter
    /// </summary>
    private void StaticEventHandler_OnBossDefeated(BossDefeatedArgs bossDefeatedArgs)
    {

        // If the chest event is it being spawned when enemies are defeated and the chest is in the room that the
        // enemies have been defeated
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onBossDefeated && bossDefeatedArgs.isBossDefeated)
        {
            SpawnChest();
        }
    }

    /// <summary>
    /// Spawn the chest prefab
    /// </summary>
    private void SpawnChest()
    {
        // set the chestSpawned variable to true
        chestSpawned = true;

        // If chest should not be spawned based on specified chance, then return
        if (!RandomSpawnChest()) return;

        // Get Number Of Ammo,Health, & Weapon Items To Spawn (max 1 of each)
        // GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum);

        // Instantiate the chest prefab to a game object within the scene
        GameObject chestGameObject = Instantiate(chestPrefab, this.transform);

        // Position chest either at specified spawn position or by player
        if (chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition)
        {
            chestGameObject.transform.position = this.transform.position;
        }
        else if (chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
        {
            // Get nearest spawn position to player
            Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(GameManager.Instance.GetPlayer().transform.position);

            // Calculate some random variation on distance from player
            Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            // Calculate final spawn position
            chestGameObject.transform.position = spawnPosition + variation;
        }

        // Retrieve Chest component from the game object
        Chest chest = chestGameObject.GetComponent<Chest>();

        // Using the Chest component, Initialize chest based on type of spawn event specified and pass in a random weapon to spawn (if weapon is the chest item type)
        if (chestSpawnEvent == ChestSpawnEvent.onRoomEntry)
        {
            // Don't use materialize effect
            chest.Initialize(false, GetWeaponItemToSpawn());
        }
        else
        {
            // use materialize effect
            chest.Initialize(true, GetWeaponItemToSpawn());
        }
    }

    /// <summary>
    /// Check if a chest should be spawned based on the chest spawn chance - returns true if chest should be spawned false otherwise
    /// </summary>
    private bool RandomSpawnChest()
    {
        // get random spawn chance based on range provided
        int chancePercent = Random.Range(chestSpawnChanceMin, chestSpawnChanceMax + 1);

        // Check if an override chance percent has been set for the current level
        foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevelList)
        {
            if (rangeByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                // override with chance percent specified for that dungeon level
                chancePercent = Random.Range(rangeByLevel.min, rangeByLevel.max + 1);
                break;
            }
        }

        // get random value between 1 and 100
        int randomPercent = Random.Range(1, 100 + 1);

        // if random value is within the random chance percent we calculated, then return true. otherwise return false
        if (randomPercent <= chancePercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Get the random weapon item to spawn
    /// </summary>
    private WeaponItemController GetWeaponItemToSpawn()
    {
        // Create an instance of the class used to select a random item from a list based on the
        // relative 'ratios' of the items specified
        RandomSpawnableObject<GameObject> weaponRandomItem = new RandomSpawnableObject<GameObject>(weaponSpawnByLevelList);

        GameObject weaponItem = weaponRandomItem.GetItem();
        WeaponItemController weaponItemController = weaponItem.GetComponent<WeaponItemController>();

        return weaponItemController;
    }

}
