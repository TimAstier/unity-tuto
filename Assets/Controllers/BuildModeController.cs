using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour {

  bool buildModeIsObjects = false;
  TileType buildModeTile = TileType.Floor;
  string buildModeObjectType;

  // Use this for initialization
  void Start() {
  }

  public void SetMode_BuildFloor() {
    buildModeIsObjects = false;
    buildModeTile = TileType.Floor;
  }

  public void SetMode_Bulldoze() {
    buildModeIsObjects = false;
    buildModeTile = TileType.Empty;
  }

  public void SetMode_BuildFurniture(string objectType) {
    // Wall is not a Tile!  Wall is an "Furniture" that exists on TOP of a tile.
    buildModeIsObjects = true;
    buildModeObjectType = objectType;
  }

  public void DoBuild(Tile t) {
    if (buildModeIsObjects == true) {
      // Create the Furniture and assign it to the tile

      // Instantly place the furniture
      // WorldController.Instance.World.PlaceFurniture(buildModeObjectType, t);

      string furnitureType = buildModeObjectType;

      if (WorldController.Instance.world.IsFurniturePlacementValid(furnitureType, t) && t.pendingFurnitureJob == null) {
        Job j = new Job(t, (Job job) => {
          WorldController.Instance.world.PlaceFurniture(furnitureType, job.tile);
          t.pendingFurnitureJob = null;
        });
        t.pendingFurnitureJob = j;
        j.RegisterJobCancelCallback((j) => j.tile.pendingFurnitureJob = null);
        WorldController.Instance.world.jobQueue.Enqueue(j);
        Debug.Log("Job Queue Size: " + WorldController.Instance.world.jobQueue.Count);
      }
    } else {
      // We are in tile-changing mode.
      t.Type = buildModeTile;
    }
  }

}
