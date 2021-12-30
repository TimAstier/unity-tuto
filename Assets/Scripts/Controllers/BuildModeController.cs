using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour {

  bool buildModeIsObjects = false;
  TileType buildModeTile = TileType.Floor;
  string buildModeObjectType;

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
    buildModeIsObjects = true;
    buildModeObjectType = objectType;
  }

  public void DoPathfindingTest() {
    WorldController.Instance.world.SetupPathfindingExample();
  }

  public void DoGenerateLevel() {
    GenerateLevel.CreateRandomLevel(WorldController.Instance.world);
  }

  public void DoBuild(Tile t) {
    if (buildModeIsObjects == true) {
      string furnitureType = buildModeObjectType;

      if (
        WorldController.Instance.world.IsFurniturePlacementValid(furnitureType, t) &&
        t.pendingFurnitureJob == null
      ) {

        Job j = new Job(t, furnitureType, (job) => {
          WorldController.Instance.world.PlaceFurniture(furnitureType, job.tile);
          t.pendingFurnitureJob = null;
        }
        );

        t.pendingFurnitureJob = j;
        j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingFurnitureJob = null; });

        WorldController.Instance.world.jobQueue.Enqueue(j);

      }

    } else {
      t.Type = buildModeTile;
    }
  }
}
