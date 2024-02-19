/*
 * DungeonBuilder.cs
 * Author: Joseph Latina
 * Created: February 7, 2024
 * Description: Contains logic for the dungeon builder algorithm and when added as a component to a game object, will hold all of the instantiated rooms within the scene
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// This will be a singleton class that can be called from any game component (ie. game manager) - mainly to call on the GenerateDungeon() method
/// </summary>
[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonoBehavior<DungeonBuilder>
{
    // Dictionary for room objects (to keep track of rooms being created and placed during the dungeon builder process)
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    // Dictionary for room templates for easy access based on room type wanted
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    // List of room templates given for the current dungeon level passed in
    private List<RoomTemplateSO> roomTemplateList = null;
    // container to hold all of the different type of room node types that exist in the game
    private RoomNodeTypeListSO roomNodeTypeList;
    // boolean value to determine if the dungeon build was successful or not
    private bool dungeonBuildSuccessful;

    protected override void Awake()
    {
        // call the awake method in the base class
        base.Awake();

        // Load the room node type list
        LoadRoomNodeTypeList();

    }

    /// <summary>
    /// Loads the room node type list from game resources
    /// </summary>
    private void LoadRoomNodeTypeList() {

        // initialize the room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// Helper function: Load the scriptable object room templates into a dictionary (for later easy access)
    /// </summary>
    private void LoadRoomTemplatesIntoDictionary() {
        
        // populate room template dictionary with room templates from the given list
        foreach (RoomTemplateSO roomTemplate in roomTemplateList) {
            // add ones that have not been added yet, otherwise report duplicate if added twice
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid)) {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            } else {
                Debug.Log("Duplicate Room Template Key In " + roomTemplateList);
            }
        }
    }

     /// <summary>
    /// Helper function: Randomly select a room node graph out of the list provided
    /// </summary>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList) {
        
        // pick a random node graph if list is not empty
        if (roomNodeGraphList.Count > 0) {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        } else {
            Debug.Log("No room node graphs in list");
            return null;
        }
    }

    /// <summary>
    /// Generate random dungeon and returns true if successful, false if not
    /// </summary>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel) {
        
        // assign the given dungeon level's room template list to this variable
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        // load those room templates into a dictionary for later easy access
        LoadRoomTemplatesIntoDictionary();

        // initialize values for loop
        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        // outer loop for trying to build dungeon by going through all of the room node graphs while checking against max attempts
        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts) {

            dungeonBuildAttempts++;

            // select a random room node graph
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            // initialize values for inner while loop
            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // inner loop for maximizing build attempts with the selected random room node graph
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph) {

                // Clean the slate (destroy gameobjects from last attempt) and increment attempt count
                ClearDungeon();
                dungeonRebuildAttemptsForNodeGraph++;

                // Attempt to build a random dungeon for the selected room node graph
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            // If successful, instantiate the room gameobjects within the scene
            if (dungeonBuildSuccessful) {
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    /// <summary>
    /// Attempts to randomly build dungeon for the given room node graph and returns boolean value if successful or not
    /// </summary>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph) {
        
        // Create open room node queue
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // Add Entrance node to the queue from the room node graph if available
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));
       
        if (entranceNode != null) {
            openRoomNodeQueue.Enqueue(entranceNode);
        } else {
            Debug.Log("No Entrance Node");
            return false;
        }

        // Detect for overlaps
        bool noRoomOverlaps = true;
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // If all room nodes have been processed and there hasn't been a room overlap, then return true
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Process rooms in the open room node queue and returns true if no room overlaps
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps) {
        
        // while queue is still populated and still no room overlaps detected,
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true) {
            // Get the next room node from the queue and dequeue it
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            // enqueue the child room nodes of the given room node
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode)) {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // if the given room node is the entrance,
            if (roomNode.roomNodeType.isEntrance) {
                // grab a random room template for it
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
                // create a room object for the given room temple
                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);
                // flag this room as positioned in the map
                room.isPositioned = true;
                // add the room to our dictionary of rooms
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }

            // otherwise if not the entrance,
            else {
                // get the parent room for the given node
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];
                // and see if the node and its parent can be placed without overlaps
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    /// <summary>
    /// Attempt to place the given room node next to its parent in the dungeon. Return boolean value
    /// </summary>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom) {
        
        // initialize and assume overlap until proven false
        bool roomOverlaps = true;

        // loop to try and place room against all available doorways of parent until successful
        while (roomOverlaps) {

            // select random unconnected doorway that's available from parent room
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

            // if there are no more available doorways in the parent room to connect, return false and exit out of function
            if (unconnectedAvailableParentDoorways.Count == 0) {
                return false;
            }

            // Otherwise, return a random unconnected available parent doorway
            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            // and get a random room template that is consistent with the parent doorway orientation
            RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            // then create the room object
            Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

            // Try to place the room. If placement of room without overlap is successful,
            if (PlaceTheRoom(parentRoom, doorwayParent, room)) {

                // mark room object as positioned
                room.isPositioned = true;
                // and add room object to dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);
                // then flag roomOverlaps as false and exit the while loop
                roomOverlaps = false;
            } 
            // otherwise if overlap exists, continue the next loop iteration
            else {
                roomOverlaps = true;
            }
        }

        // if room was successfully placed during the loop, return true
        return true;
    }

    /// <summary>
    /// Retrieve unconnected doorways from given list
    /// </summary>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList) {
        
        // iterate through the given doorway list
        foreach (Doorway doorway in roomDoorwayList) {
            // and only return doorway if it has not been connected and is available
            if (!doorway.isConnected && !doorway.isUnavailable) {
                yield return doorway;
            }
        }
    }

    /// <summary>
    /// Retrieves a random room template for given room node while taking into account the parent doorway orientation
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent) {
        
        // first initialize the roomt emplate variable
        RoomTemplateSO roomtemplate = null;

        // If room type is a corridor,
        if (roomNode.roomNodeType.isCorridor) {
            // determine the parent's doorway orientation and find the corridor template that matches orientation
            // note that the corridor type (corridorNS or corridorEW) gets set in this method depending on the orientation of the selected random doorway from the parent room
            switch (doorwayParent.orientation) {

                // Corridor NS
                case Orientation.north:
                case Orientation.south:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;

                // Corridor EW
                case Orientation.east:
                case Orientation.west:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;

                case Orientation.none:
                    break;

                default:
                    break;
            }
        }

        // Otherwise if just another room,
        else {
            // then select random room template
            roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomtemplate;
    }

    /// <summary>
    /// Grabs a random room template from the room template list, matching the given room type
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType) {
        
        // Instantiate matching room template list
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // Iterate through room template list to grab all that match the specified room node type
        foreach (RoomTemplateSO roomTemplate in roomTemplateList) {

            if (roomTemplate.roomNodeType == roomNodeType) {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        // return null if no match. otherwise return random room template
        if (matchingRoomTemplateList.Count == 0) {
            return null;
        } else {
            return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
        }
        
    }

    /// <summary>
    /// Create room based on the given room template and room node
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode) {
        
        Room room = new Room();

        // reference all the member variable values from the room template
        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.enemiesByLevelList = roomTemplate.enemiesByLevelList;
        room.roomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParametersList;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;

        // create deep copies of these lists
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

        // set parent ID for room
        if (roomNode.parentRoomNodeIDList.Count == 0) {
            // if Entrance,
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            // set entrance in game manager as the current room
            GameManager.Instance.SetCurrentRoom(room);
        } else {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        // If there are no enemies to spawn, then flag this room as cleared of enemies
        if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0) {
            room.isClearedOfEnemies = true;
        }
        
        return room;
    }

    /// <summary>
    /// Places the room next to its parent using the given doorway object (returns boolean value if successful or not)
    /// </summary>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room) {
        
        // Get the current room doorway position of room if available (by checking orientation of parent doorway)
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        // If no doorway available in opposite orientation,
        if (doorway == null) {
            // flag the parent doorway as unavailable to avoid trying to fit this room with this parent again and return false
            doorwayParent.isUnavailable = true;
            return false;
        }

        // Now if there is an available doorway, calculate the 'world' grid parent doorway position
        // this is done by calculating the distance from the position of the doorway relative to the corner of the room
        Vector2Int doorwayPosition = doorwayParent.position - parentRoom.templateLowerBounds;
        // and add that to the 'world' position of the corner of the room
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayPosition;
        
        // Now calculate the adjustment needed to the positioning of the room based on parent doorway position to align correctly
        // ie. if this doorway is west then we need to add (1,0) to the east parent doorway
        Vector2Int adjustment = Vector2Int.zero;
        // done through a switch statement to account for all orientations
        switch (doorway.orientation) {

            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;

            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;

            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;

            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;

            case Orientation.none:
                break;

            default:
                break; 
        }

        // And finally, calculate the room lower bounds and upper bounds based on 'world' grid parent doorway position
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        // Now check for room overlap
        Room overlappingRoom = CheckForRoomOverlap(room);

        // If no overlap,
        if (overlappingRoom == null) {

            // mark the room doorway and parent doorway as connected and unavailable
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;
            doorway.isConnected = true;
            doorway.isUnavailable = true;

            // return true
            return true;
        } else {

            // flag the parent doorway as unavailable so we avoid trying to connect again to this one
            doorwayParent.isUnavailable = true;

            return false;
        }

    }

    /// <summary>
    /// Get the doorway object of room based on parent's doorway orientation
    /// </summary>
    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList) {
        
        // iterate through the given room's doorway list
        foreach (Doorway doorwayToCheck in doorwayList) {

            // if parent doorway is facing east, return room doorway facing west
            if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west) {
                return doorwayToCheck;
            }
            // if parent doorway is facing west, return room doorway facing east
            else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east) {
                return doorwayToCheck;
            }
            // if parent doorway is facing north, return room doorway facing south
            else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south) {
                return doorwayToCheck;
            }
            // if parent doorway is facing south, return room doorway facing north
            else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north) {
                return doorwayToCheck;
            }
        }

        // otherwise return null if no doorway in opposite orientation found
        return null;
    }

    /// <summary>
    /// Check for room overlap. If so, return the room overlapping with the room being tested
    /// </summary>
    private Room CheckForRoomOverlap(Room roomToTest) {
        
        // iterate through all of the rooms already created within the room dictionary
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary) {

            Room room = keyvaluepair.Value;
            
            // skip the room selected if it's the same as the room being tested or if it has not been positioned yet
            if (room.id == roomToTest.id || !room.isPositioned) {
                continue;
            }

            // if it is overlapping, return the room it's overlapping with
            if (IsOverLappingRoom(roomToTest, room)) {
                return room;
            }
        }

        // return null if no overlap
        return null;
    }

    /// <summary>
    /// Check if two rooms are overlapping with each other
    /// </summary>
    private bool IsOverLappingRoom(Room room1, Room room2) {
        
        bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);

        bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        // if overlapping in both x and y axis, return true
        if (isOverlappingX && isOverlappingY) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Check if the two intervals overlap with each other
    /// </summary>
    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2) {
        
        // If iMin2 <= iMax1 and vice versa, then return true
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2)) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Helper function: Get a room template SO from the dictionary by its room template ID
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID) {
        
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate)) {
            return roomTemplate;
        } else {
            return null;
        }
    }

    /// <summary>
    /// Helper function: Get a room object from the room dictionary by its room ID
    /// </summary>
    public Room GetRoomByRoomID(string roomID) {
        
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room)) {
            return room;
        } else {
            return null;
        }
    }

    /// <summary>
    /// Helper function: Instantiate the dungeon room gameobjects from the prefabs
    /// </summary>
    private void InstantiateRoomGameobjects() {
        
        // Iterate through all of the dungeon rooms in the room dictionary
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary) {

            Room room = keyvaluepair.Value;

            // Calculate the actual room position within the scene in the game (needs to be adjusted by room template bounds)
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);
        
            // Instantiate the room
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);
        
            // Get the 'Instantiated Room' component from this newly instantiated room prefab
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            // save the game object reference of the room object to the corresponding member variable in the 'Instantiated Room' class
            instantiatedRoom.room = room;

            // Initialize the instantiated room (basically populate the member variables and block off unused doorways)
            instantiatedRoom.Initialize(roomGameobject);

            // save the game object reference for this instantiated room in this class' member variable
            room.instantiatedRoom = instantiatedRoom;
        }
       
    }

    /// <summary>
    /// Helper function: destroy dungeon room game objects and dungeon room dictionary from last dungeon builder attempt
    /// </summary>
    private void ClearDungeon() {
        
        // if dictionary is not empty
        if (dungeonBuilderRoomDictionary.Count > 0) {
            
            // iterate through it and destroy the gameobjects for the instantiated room nodes
            foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary) {

                Room room = keyvaluepair.Value;

                if (room.instantiatedRoom != null) {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungeonBuilderRoomDictionary.Clear();
        }
    }

    /// <summary>
    /// Create deep copy of string list
    /// </summary>
    private List<string> CopyStringList(List<string> oldStringList) {
        
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList) {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    /// <summary>
    /// Helper function: Create deep copy of doorway list
    /// </summary>
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList) {
        
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList) {
            
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }
}
