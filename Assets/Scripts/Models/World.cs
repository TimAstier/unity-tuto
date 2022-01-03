//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameMode { Select, Build };

public class World {

  public Tile[,] tiles { get; protected set; }
  public GameMode gameMode { get; protected set; } = GameMode.Select;
  public List<Character> characters { get; protected set; }
  public Path_TileGraph tileGraph;
  public JobQueue jobQueue;

  Dictionary<string, Furniture> furniturePrototypes;

  public World() {
    jobQueue = new JobQueue();
    CreateFurniturePrototypes();
    tiles = GenerateLevel.GenerateMap(this);
    characters = new List<Character>();
  }

  public void Update(float deltaTime) {
    foreach (Character c in characters) {
      c.Update(deltaTime);
    }
  }

  public Character CreateCharacter(Tile t) {
    Character c = new Character(t);
    characters.Add(c);
    GameEvents.current.CharacterCreated(c);
    return c;
  }

  void CreateFurniturePrototypes() {
    furniturePrototypes = new Dictionary<string, Furniture>();

    furniturePrototypes.Add("Wall",
      Furniture.CreatePrototype(
                "Wall",
                0,  // Impassable
                1,  // Width
                1,  // Height
                true // Links to neighbours and "sort of" becomes part of a large object
              )
    );
  }

  public void SetupPathfindingExample() {
    Debug.Log("SetupPathfindingExample");

    // Make a set of floors/walls to test pathfinding with.

    int l = Constants.GRID_WIDTH / 2 - 5;
    int b = Constants.GRID_HEIGHT / 2 - 5;

    for (int x = l - 5; x < l + 15; x++) {
      for (int y = b - 5; y < b + 15; y++) {
        tiles[x, y].Type = TileType.Floor;
        if (x == l || x == (l + 9) || y == b || y == (b + 9)) {
          if (x != (l + 9) && y != (b + 4)) {
            PlaceFurniture("Wall", tiles[x, y]);
          }
        }
      }
    }
  }

  public Tile GetTileAt(int x, int y) {
    if (x >= Constants.GRID_WIDTH || x < 0 || y >= Constants.GRID_HEIGHT || y < 0) {
      return null;
    }
    return tiles[x, y];
  }


  public void PlaceFurniture(string objectType, Tile t) {
    // TODO: This function assumes 1x1 tiles -- change this later!

    if (furniturePrototypes.ContainsKey(objectType) == false) {
      Debug.LogError("furniturePrototypes doesn't contain a proto for key: " + objectType);
      return;
    }

    Furniture obj = Furniture.PlaceInstance(furniturePrototypes[objectType], t);

    if (obj == null) {
      // Failed to place object -- most likely there was already something there.
      return;
    }

    GameEvents.current.FurnitureCreated(obj);
    InvalidateTileGraph();
  }

  public void DestroyFurniture(Tile t) {
    if (t.furniture != null) {
      GameEvents.current.FurnitureDestroyed(t.furniture);
    }
    return;
  }

  public void InvalidateTileGraph() {
    tileGraph = null;
  }

  public bool IsFurniturePlacementValid(string furnitureType, Tile t) {
    return furniturePrototypes[furnitureType].IsValidPosition(t);
  }

  public Furniture GetFurniturePrototype(string objectType) {
    if (furniturePrototypes.ContainsKey(objectType) == false) {
      Debug.LogError("No furniture with type: " + objectType);
      return null;
    }

    return furniturePrototypes[objectType];
  }

  public void SetGameMode(GameMode gameMode) {
    this.gameMode = gameMode;
  }

  // Based on https://www.redblobgames.com/grids/circle-drawing/#bounding-box
  List<Tile> GetTilesInCircle(Vector2Int origin, float radius) {
    int top = Mathf.FloorToInt(origin.y - radius);
    int bottom = Mathf.CeilToInt(origin.y + radius);
    int left = Mathf.FloorToInt(origin.x - radius);
    int right = Mathf.CeilToInt(origin.x + radius);

    List<Tile> result = new List<Tile>();
    for (int x = left; x <= right; x++) {
      for (int y = top; y <= bottom; y++) {
        Tile tile = this.GetTileAt(x, y);
        if (tile != null && GridUtils.IsInsideCircle(origin, new Vector2Int(x, y), radius)) {
          result.Add(tile);
        }
      }
    }
    return result;
  }

  public void UpdateTilesVisibility(Tile origin, int radius) {
    // TODO: DRY if possible
    // TODO: Any way to avoid three loops?

    // Clear tiles
    List<Tile> clearTiles = GetTilesInCircle(new Vector2Int(origin.X, origin.Y), Constants.MAX_CLEAR_VISIBILITY);
    foreach (Tile t in clearTiles) {
      List<Vector2Int> inBetweenPositions = GridUtils.Line(new Vector2Int(t.X, t.Y), new Vector2Int(origin.X, origin.Y));
      List<Vector2Int> opaquePositions = inBetweenPositions.FindAll(p => {
        Tile myTile = this.GetTileAt(p.x, p.y);
        if (myTile != null && myTile.CanSeeThrough() == false) {
          return true;
        }
        return false;
      });

      if (opaquePositions.Count == 0 || (opaquePositions.Count == 1 && t.CanSeeThrough() == false)) {
        t.SetVisibility(TileVisibility.Clear);
      } else {
        t.SetVisibility(TileVisibility.Dark);
      }

    }

    // Dim tiles
    List<Tile> candidateDimTiles = GetTilesInCircle(new Vector2Int(origin.X, origin.Y), Constants.MAX_DIM_VISIBILITY);
    foreach (Tile t in candidateDimTiles) {
      if (clearTiles.Contains(t)) {
        continue;
      }

      List<Vector2Int> inBetweenPositions = GridUtils.Line(new Vector2Int(t.X, t.Y), new Vector2Int(origin.X, origin.Y));
      List<Vector2Int> opaquePositions = inBetweenPositions.FindAll(p => {
        Tile myTile = this.GetTileAt(p.x, p.y);
        if (myTile != null && myTile.CanSeeThrough() == false) {
          return true;
        } else {
          return false;
        }
      });
      if (opaquePositions.Count == 0 || (opaquePositions.Count == 1 && t.CanSeeThrough() == false)) {
        t.SetVisibility(TileVisibility.Dim);
      } else {
        t.SetVisibility(TileVisibility.Dark);
      }
    }

    // Darken tiles around
    // NOTE: We use a "+2" circle as "+1" fails to work for four diagonal tiles.
    List<Tile> toDarkenTiles = GetTilesInCircle(new Vector2Int(origin.X, origin.Y), Constants.MAX_DIM_VISIBILITY + 2);
    foreach (Tile t in toDarkenTiles) {
      if (candidateDimTiles.Contains(t)) {
        continue;
      }
      t.SetVisibility(TileVisibility.Dark);
    }
  }
}
