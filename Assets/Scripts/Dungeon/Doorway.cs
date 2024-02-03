/*
 * Doorway.cs
 * Author: Joseph Latina
 * Created: January 30, 2024
 * Description: Used to hold information (ie. position, orientation) regarding this doorway instance.
 */

using UnityEngine;
[System.Serializable]

/// <summary>
/// Represents the doorway instance
/// </summary>
public class Doorway
{
  public Vector2Int position;
  public Orientation orientation;
  public GameObject doorPrefab;

  #region Header
  [Header("The Upper Left Position To Start Copying From")]
  #endregion
  public Vector2Int doorwayStartCopyPosition;
  #region Header
  [Header("The width of tiles in the doorway to copy over")]
  #endregion
  public int doorwayCopyTileWidth; 
  #region Header
  [Header("The height of tiles in the doorway to copy over")]
  #endregion
  public int doorwayCopyTileHeight; 

  //For algorithm use only
  [HideInInspector] public bool isConnected = false; //indicates whether doorway has been connected or not
  [HideInInspector] public bool isUnavailable = false; //indicates whether doorway is available or not. This is set to true if doorway can't be connected due to overlap of rooms


}
