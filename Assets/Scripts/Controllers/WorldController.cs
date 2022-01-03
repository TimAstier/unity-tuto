//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldController : MonoBehaviour {

  public static WorldController Instance { get; protected set; }

  public World world { get; protected set; }

  void OnEnable() {
    if (Instance != null) {
      Debug.LogError("There should never be two world controllers.");
    }
    Instance = this;

    // Create a world
    world = new World();

    // Center the Camera
    Camera.main.transform.position = new Vector3(Constants.GRID_WIDTH / 2, Constants.GRID_HEIGHT / 2, Camera.main.transform.position.z);
  }

  void Update() {
    world.Update(Time.deltaTime);
  }

  public Tile GetTileAtWorldCoord(Vector3 coord) {
    int x = Mathf.FloorToInt(coord.x);
    int y = Mathf.FloorToInt(coord.y);
    return world.GetTileAt(x, y);
  }

}
