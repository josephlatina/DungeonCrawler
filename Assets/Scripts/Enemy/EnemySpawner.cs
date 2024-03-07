using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonoBehavior<EnemySpawner>
{
    private int enemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawnedSoFar;
    private int enemyMaxConcurrentSpawnNumber;
    // populated when the onRoomChanged event gets triggered
    private Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;

    /// <summary>
    /// Method called when the game object this script is attached to is enabled in scene hierarchy
    /// </summary>
    private void OnEnable() {

        // Add StaticEventHandler_OnRoomChanged as a subscriber to OnRoomChanged event
        // Therefore, start listening for room change events
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    /// <summary>
    /// Method called when this script is disabled in scene hierarchy
    /// </summary>
    private void OnDisable() {

        // unsubscribe to change of room event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    /// <summary>
    /// Subscriber method that requires the current room object as parameter
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs) {

        // initialize variables
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;
        currentRoom = roomChangedEventArgs.room;

        // If room is corridor or entrancy type, then return (no need to spawn here)
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isCorridorEW) {
            return;
        }

        // If room has already been flagged as defeated, then return as well
        if (currentRoom.isClearedOfEnemies) return;

        // Retrieve enemy details based on dungeon level
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());
        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());
        // If no enemies to spawn, then mark it as cleared
        if (enemiesToSpawn == 0) {
            currentRoom.isClearedOfEnemies = true;
            return;
        }
        // Get concurrent number of enemies to spawn
        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        // Spawn the enemies
        SpawnEnemies();
    }

    /// <summary>
    /// get random number of concurrent enemies between min and max values
    /// </summary>
    private int GetConcurrentEnemies() {

        return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
    }

    /// <summary>
    /// Spawn the enemies for this room in dungeon level
    /// </summary>
    private void SpawnEnemies() {

        // Spawn the enemies
        StartCoroutine(SpawnEnemiesRoutine());
    }

    /// <summary>
    /// Spawn the enemies coroutine
    /// </summary>
    private IEnumerator SpawnEnemiesRoutine() {

        // Get grid component of instantiate room to convert spawn position into world position
        Grid grid = currentRoom.instantiatedRoom.grid;

        // Instantiate our helper class for selecting a random enemy
        RandomSpawnableObject<EnemyScriptableObject> randomEnemyHelperClass = new RandomSpawnableObject<EnemyScriptableObject>(currentRoom.enemiesByLevelList);

        // check if we have spawn positions to spawn the enemies from
        if (currentRoom.spawnPositionArray.Length > 0) {

            // If so, loop through list of possible enemies to spawn
            for (int i = 0; i < enemiesToSpawn; i++) {

                // keep instantiate until it hits the max concurrent spawn amount
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber) {
                    yield return null;
                }
            
                // pick a cell position to spawn from
                Vector3Int cellPosition = (Vector3Int) currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                // Instantiate the enemy at grid position
                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                // wait for number of interval seconds until spawning
                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    } 

    /// <summary>
    /// Get a random spawn interval between min and max values
    /// </summary>
    private void CreateEnemy(EnemyScriptableObject enemySO, Vector3 position) {

       // keep track of spawned enemies so far
       enemiesSpawnedSoFar++;

       // increment current existing enemy count (reduced when enemy is destroyed)
       currentEnemyCount++;

        // get current dungeon level
        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        // instantiate the enemy and be created as children objects beneath the EnemySpawn object (tranform as sole parent game object)
        GameObject spawnedEnemy = Instantiate(enemySO.prefab, position, Quaternion.identity, transform);

        // initialize enemy
        spawnedEnemy.GetComponent<EnemyController>().EnemyInitialization(enemySO, enemiesSpawnedSoFar, dungeonLevel);
       
    }

    /// <summary>
    /// Get a random spawn interval between min and max values
    /// </summary>
    private float GetEnemySpawnInterval() {

       return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
    }
}
