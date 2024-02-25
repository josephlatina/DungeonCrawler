/*
 * GameManager.cs
 * Author: Joseph Latina
 * Created: February 14, 2024
 * Description: Utility script to getting a random spawnable object from a given list
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class: From a given list, gets a random spawnable object based on its spawn ratio
/// </summary>
public class RandomSpawnableObject<T>
{
    // helper data container for the spawn ratio interval (spawn chance) for a spawnable object
    private struct chanceBoundaries {
        public T spawnableObject;
        public int lowBoundaryValue;
        public int highBoundaryValue;
    }

    // total amount of all the relative enemy spawn ratios added together
    private int ratioValueTotal = 0;
    // list of chance boundaries struct
    private List<chanceBoundaries> chanceBoundariesList = new List<chanceBoundaries>();
    // list of possible spawnable objects passed in
    private List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList;

    /// <summary>
    /// Constructor method for this class
    /// </summary>
    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList) {

        // save the passed in list to the member variable
        this.spawnableObjectsByLevelList = spawnableObjectsByLevelList;
    }

    /// <summary>
    /// Class we want for getting the random spawnable object
    /// </summary>
    public T GetItem() {

        // initialize variables
        int upperBoundary = -1;
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        // initialize to the default value of this spawnable object type (ie. 0, null)
        T spawnableObject = default(T);

        // iterate through each list of spawnable objects
        foreach (SpawnableObjectsByLevel<T> spawnableObjectsByLevel in spawnableObjectsByLevelList) {

            // if the dungeon level pertaining to this spawnable object list matches the current dungeon level in current game session,
            if (spawnableObjectsByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel()) {
                // in each list, iterate through spawnable object ratios of each spawnable object
                foreach (SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjectsByLevel.spawnableObjectRatios) {
                   
                   // build the chance boundaries values for this spawnable object 
                    int lowerBoundary = upperBoundary + 1;
                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;
                    ratioValueTotal += spawnableObjectRatio.ratio;
                    // and add it to our list
                    chanceBoundariesList.Add(new chanceBoundaries() { 
                        spawnableObject = spawnableObjectRatio.dungeonObject, 
                        lowBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary
                    });
                }
            }
        }

        // if no chance boundaries list set up, return default value of spawnable object
        if (chanceBoundariesList.Count == 0) return default(T);

        // otherwise, generate a random look up value to select random spawnable object out of list
        int lookUpValue = Random.Range(0, ratioValueTotal);
        foreach (chanceBoundaries spawnChance in chanceBoundariesList) {
            // if the look up value is within the spawn chance interval of a certain object,
            if (lookUpValue >= spawnChance.lowBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue) {
                // save this spawnable object
                spawnableObject = spawnChance.spawnableObject;
                break;
            }
        }

        // return the saved spawnable object (or if default value if none found)
        return spawnableObject;
    }

}
