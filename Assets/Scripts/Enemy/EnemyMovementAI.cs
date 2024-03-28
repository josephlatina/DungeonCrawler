/*
 * EnemyMovementAI.cs
 * Authors: Joseph Latina
 * Created: March 11, 2024
 * Description: Script for impolementing enemy movement AI logic. Designed to be attached to the enemy prefab
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    // reference to the enemy controller component
    private EnemyController enemy;
    private EnemyScriptableObject enemySO;
    // stack of movement steps for AStar algorithm
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    // hold previous position of player to see how far player has moved
    private Vector3 playerReferencePosition;
    // reference to coroutine to move enemy along a path frame by frame
    private Coroutine moveEnemyRoutine;
    // cooldown time until enemy rebuilds a path to player
    private float currentEnemyPathRebuildCooldown;
    // to yield in our coroutine rather than continuously recreating it
    private WaitForFixedUpdate waitForFixedUpdate;
    // player's movement speed
    [HideInInspector] public float moveSpeed;
    // if enemy should chase player or not
    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1; // default value.  This is set by the enemy spawner.
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

    private void Awake()
    {
        // Load enemy SO component
        enemy = GetComponent<EnemyController>();
        enemySO = enemy.enemyStats;
        // Get enemy's movement speed
        moveSpeed = enemy.GetMoveSpeed();
    }

    private void Start()
    {
        // Create waitforfixed update for use in coroutine
        waitForFixedUpdate = new WaitForFixedUpdate();

        // Get player's initial position
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

    }

    private void Update()
    {
        // move enemy on every frame if player should be chased
        MoveEnemy();

        // change enemy state to idle if player already exited the room
        if (GameManager.Instance.GetCurrentRoom().roomNodeType.isCorridorEW
            || GameManager.Instance.GetCurrentRoom().roomNodeType.isCorridorNS
            || Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) >= enemy.enemyStats.chaseDistance)
        {
            enemy.EnemyStateMachine.TransitionTo(enemy.EnemyStateMachine.idleState);
            chasePlayer = false;
            StopMovement();
        }
    }


    /// <summary>
    /// Use AStar pathfinding to build a path to the player - and then move the enemy to each grid location on the path
    /// </summary>
    private void MoveEnemy()
    {
        // Reduce Movement cooldown timer until next path rebuild
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        // Check distance to player to see if enemy should start chasing
        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyStats.chaseDistance)
        {
            chasePlayer = true;
            moveSpeed = enemy.GetMoveSpeed();
        }

        // If not close enough to chase player then return
        if (!chasePlayer)
            return;

        // Only process A Star path rebuild on certain frames to spread the load between enemies
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber) return;

        if (GameManager.Instance.GetPlayer() == null)
        {
            return;
        }
        // if the movement cooldown timer has been reached or player has moved more than required distance, then rebuild the enemy path and move the enemy
        if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) > Settings.playerMoveDistanceToRebuildPath))
        {
            // Reset path rebuild cooldown timer back to the max count
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            // Get the player's recent position
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            // Move the enemy using AStar pathfinding - Trigger rebuild of path to player
            CreatePath();

            // If a path has been found, then move the enemy along this path
            if (movementSteps != null)
            {
                // if we are already moving the enemy
                if (moveEnemyRoutine != null)
                {
                    // Ensure there's only one coroutine running
                    StopCoroutine(moveEnemyRoutine);
                }

                // Move enemy along the path using a coroutine
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));

            }
        }
    }


    /// <summary>
    /// Coroutine to move the enemy to the next location on the path by passing in the stack of movement steps for the path the enemy should take
    /// </summary>
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        // while there is still a step to be taken,
        while (movementSteps.Count > 0)
        {
            // pop off the next position to be taken
            Vector3 nextPosition = movementSteps.Pop();

            // while the enemy's position is not very close to the next position yet, continue to move
            // when it is close (< 0.2) then step out of this while loop and move on to the next movement step in the stack
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                // Trigger movement event to the next position in the direction passed
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, moveSpeed, (nextPosition - transform.position).normalized);
                // moving the enemy using 2D physics so wait until the next fixed update
                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        }

    }

    /// <summary>
    /// Use the AStar static class to create a path for the enemy
    /// </summary>
    private void CreatePath()
    {
        // Get the current room the enemy is in
        Room currentRoom = GameManager.Instance.GetCurrentRoom();
        // Get the grid component of the room
        Grid grid = currentRoom.instantiatedRoom.grid;

        // Get players position on this room grid that we want to pathfind to, if they are in the same room
        if (enemy.spawnRoomID == currentRoom.id)
        {
            Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

            // Convert it to the cell position
            Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

            // Build a path (stack of movement steps) for the enemy to move on using the enemy's position as start node and player's position as the target node
            movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

            // If path is found, pop off first step on stack of movement steps - this is the grid square the enemy is already on
            if (movementSteps != null)
            {
                movementSteps.Pop();
            }
            else
            {

            }
        }
    }

    /// <summary>
    /// Set the frame number that the enemy path will be recalculated on - to avoid performance spikes
    /// </summary>
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    /// <summary>
    /// Get the nearest position to the player that isn't on an obstacle (since players can be on an obstacle - ie. half-collision tile wall)
    /// </summary>
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        // get player position
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
        // convert from world coords to cell position
        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);
        // adjust it using the room template's lower bounds
        Vector2Int adjustedPlayerCellPositon = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x, playerCellPosition.y - currentRoom.templateLowerBounds.y);

        // check if there's an obstacle on that position the player is on
        int obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y];

        // if the player isn't on a cell square marked as an obstacle then return that position
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        // otherwise, find a surounding cell that isn't an obstacle - required because with the 'half collision' tiles
        // and tables the player can be on a grid square that is marked as an obstacle
        else
        {
            // Empty surrounding position list of obstacle
            surroundingPositionList.Clear();

            // Populate surrounding position list of obstacle - this will hold the 8 possible vector locations surrounding a (0,0) grid square
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }


            // Loop through all the surrounding position list of the obstacle
            for (int l = 0; l < 8; l++)
            {
                // Generate a random index for the list
                int index = Random.Range(0, surroundingPositionList.Count);

                // See if there is an obstacle in the selected surrounding position (check if we are accessing a position outside the Astar movement penalty array)
                try
                {
                    // check this random surrounding position in our penalty array
                    obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y], currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y]);

                    // If this position is not an obstacle (penalty is not 0), then return the cell position to navigate to
                    if (obstacle != 0)
                    {
                        return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x, playerCellPosition.y + surroundingPositionList[index].y, 0);
                    }

                }
                // Catch errors where the surrounding positon is outside the grid
                catch
                {
                    continue;
                }

                // Remove the surrounding position with the obstacle so we can try again
                surroundingPositionList.RemoveAt(index);
            }


            // If no non-obstacle cells found surrounding the player - send the enemy in the direction of an enemy spawn position
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

        }
    }

    public void StopMovement()
    {
        moveSpeed = 0;
    }
}
