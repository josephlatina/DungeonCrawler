using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public RoomTemplateSO roomTemplateSO;
    private List<SpawnableObjectsByLevel<EnemyScriptableObject>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyScriptableObject> randomEnemyHelperClass;
    private GameObject instantiatedEnemy;

    private void Start() {
        // initialize list of enemy spawnable objects from room template passed in
        testLevelSpawnList = roomTemplateSO.enemiesByLevelList;

        // initialize RandomSpawnableObject by passing in list of enemy spawnable objects
        randomEnemyHelperClass = new RandomSpawnableObject<EnemyScriptableObject>(testLevelSpawnList);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.T)) {

            if (instantiatedEnemy != null) {
                Destroy(instantiatedEnemy);
            }

            EnemyScriptableObject spawnedEnemy = randomEnemyHelperClass.GetItem();

            // if enemy is picked, instantiate it close to player (without rotation)
            if (spawnedEnemy != null) {
                instantiatedEnemy = Instantiate(spawnedEnemy.prefab, HelperUtilities.GetSpawnPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition()), Quaternion.identity);
            }
        }
    }
}
