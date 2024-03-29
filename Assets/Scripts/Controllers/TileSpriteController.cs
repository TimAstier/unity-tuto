﻿//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSpriteController : MonoBehaviour {

  // Could be loaded from a data file later.
  public Sprite floorSprite;
  public Sprite emptySprite;

  Dictionary<Tile, GameObject> tileGameObjectMap;

  World world {
    get { return WorldController.Instance.world; }
  }

  void Start() {
    // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
    tileGameObjectMap = new Dictionary<Tile, GameObject>();

    // Create a GameObject for each of our tiles
    for (int x = 0; x < Constants.GRID_WIDTH; x++) {
      for (int y = 0; y < Constants.GRID_WIDTH; y++) {
        // Get the tile data
        Tile tile_data = world.GetTileAt(x, y);

        // This creates a new GameObject and adds it to our scene.
        GameObject tile_go = new GameObject();

        // Add our tile/GO pair to the dictionary.
        tileGameObjectMap.Add(tile_data, tile_go);

        tile_go.name = "Tile_" + x + "_" + y;
        tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
        tile_go.transform.SetParent(this.transform, true);

        // Add a Sprite Renderer
        // Add a default sprite for empty tiles.
        SpriteRenderer sr = tile_go.AddComponent<SpriteRenderer>();
        sr.sprite = emptySprite;
        sr.sortingLayerName = "Tiles";
      }
    }

    GameEvents.current.onTileTypeChanged += OnTileTypeChanged;
  }

  private void OnDestroy() {
    GameEvents.current.onTileTypeChanged -= OnTileTypeChanged;
  }

  void OnTileTypeChanged(Tile tile_data) {
    if (tileGameObjectMap.ContainsKey(tile_data) == false) {
      Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary?");
      return;
    }

    GameObject tile_go = tileGameObjectMap[tile_data];

    if (tile_go == null) {
      Debug.LogError("tileGameObjectMap's returned GameObject is null -- did you forget to add the tile to the dictionary?");
      return;
    }

    if (tile_data.Type == TileType.Floor) {
      tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
    } else if (tile_data.Type == TileType.Empty) {
      tile_go.GetComponent<SpriteRenderer>().sprite = emptySprite;
    } else {
      Debug.LogError("OnTileChanged - Unrecognized tile type.");
    }
  }
}
