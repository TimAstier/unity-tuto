using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

static class ButtonName {
  public const string BuildFloor = "Button - Build Floor";
  public const string BuildWall = "Button - Build Wall";
  public const string Bulldoze = "Button - Bulldoze";
}

public class BuildModeController : MonoBehaviour {

  bool buildModeIsObjects = false;
  TileType buildModeTile = TileType.Floor;
  string buildModeObjectType;
  string selectedButton;
  string previousSelectedButton;


  void Start() {
  }

  void Update() {

    if (WorldController.Instance.world.gameMode != GameMode.Build) {
      if (selectedButton != null) {
        GameObject go = GameObject.Find(selectedButton);
        go.GetComponentInChildren<Text>().color = Color.black;
        selectedButton = null;
      }
    }

    if (selectedButton != null) {
      GameObject go = GameObject.Find(selectedButton);
      if (go == null) {
        Debug.LogError("BuildModeController -- No GO found for this button.");
        return;
      }
      go.GetComponentInChildren<Text>().color = Color.blue;
      if (previousSelectedButton != null && previousSelectedButton != selectedButton) {
        GameObject previousGo = GameObject.Find(previousSelectedButton);
        if (previousGo == null) {
          Debug.LogError("BuildModeController -- No GO found for previous button.");
          return;
        }
        previousGo.GetComponentInChildren<Text>().color = Color.black;
      }
    }
  }

  public void SetMode_BuildFloor() {
    WorldController.Instance.world.SetGameMode(GameMode.Build);
    buildModeIsObjects = false;
    buildModeTile = TileType.Floor;
    previousSelectedButton = selectedButton;
    selectedButton = ButtonName.BuildFloor;
  }

  public void SetMode_Bulldoze() {
    WorldController.Instance.world.SetGameMode(GameMode.Build);
    buildModeIsObjects = false;
    buildModeTile = TileType.Empty;
    previousSelectedButton = selectedButton;
    selectedButton = ButtonName.Bulldoze;
  }

  public void SetMode_BuildFurniture(string objectType) {
    WorldController.Instance.world.SetGameMode(GameMode.Build);
    buildModeIsObjects = true;
    buildModeObjectType = objectType;
    previousSelectedButton = selectedButton;
    selectedButton = ButtonName.BuildWall;
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
