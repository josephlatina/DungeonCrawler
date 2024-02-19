/*
 * SpawnTest.cs
 * Author: Joseph Latina
 * Created: February 18, 2024
 * Description: Test script for spawning. Designed to be added as a component to the Game Manager object
 */

using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    private List<SpawnableObjectsByLevel<EnemyScriptableObject>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyScriptableObject> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

    /// <summary>
    /// Method called when the game object this script is attached to is enabled in scene hierarchy
    /// </summary>
    private void OnEnable() {
        Debug.Log("hello");

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
        Debug.Log("hello2");
        // Destroy any currently spawned enemies
        if (instantiatedEnemyList != null && instantiatedEnemyList.Count > 0) {
            foreach (GameObject enemy in instantiatedEnemyList) {
                Destroy(enemy);
            }
        }

        Debug.Log("hello3");
        // Get the room template SO associated with current instantiated room
        RoomTemplateSO roomTemplateSO = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateID);
        Debug.Log(roomTemplateSO);

        if (roomTemplateSO != null) {
            // initialize list of enemy spawnable objects from room template passed in
            testLevelSpawnList = roomTemplateSO.enemiesByLevelList;

            // initialize RandomSpawnableObject by passing in list of enemy spawnable objects
            randomEnemyHelperClass = new RandomSpawnableObject<EnemyScriptableObject>(testLevelSpawnList);
        }
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.T)) {

            EnemyScriptableObject spawnedEnemy = randomEnemyHelperClass.GetItem();

            // if enemy is picked, instantiate it close to player (without rotation) and add to instantiated enemy list
            if (spawnedEnemy != null) {
                instantiatedEnemyList.Add(Instantiate(spawnedEnemy.prefab, HelperUtilities.GetSpawnPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition()), Quaternion.identity));
            }
        }
    }
}
