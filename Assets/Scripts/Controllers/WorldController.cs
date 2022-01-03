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
    world = new World(Constants.GRID_WIDTH, Constants.GRID_HEIGHT);

    // Center the Camera
    Camera.main.transform.position = new Vector3(world.Width / 2, world.Height / 2, Camera.main.transform.position.z);
  }

  void Update() {
    world.Update(Time.deltaTime);
  }

  /// <summary>
  /// Gets the tile at the unity-space coordinates
  /// </summary>
  /// <returns>The tile at world coordinate.</returns>
  /// <param name="coord">Unity World-Space coordinates.</param>
  public Tile GetTileAtWorldCoord(Vector3 coord) {
    int x = Mathf.FloorToInt(coord.x);
    int y = Mathf.FloorToInt(coord.y);
    return world.GetTileAt(x, y);
  }

}
