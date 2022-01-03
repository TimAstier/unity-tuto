//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Collections;
using UnityEngine;

public enum TileType { Empty, Floor };
public enum TileVisibility { Clear, Dim, Dark };

public class Tile {
  private TileType _type = TileType.Empty;
  public TileType Type {
    get { return _type; }
    set {
      TileType oldType = _type;
      _type = value;

      if (oldType != _type) {
        GameEvents.current.TileTypeChanged(this);
      }
    }
  }

  public TileVisibility visibility { get; protected set; } = TileVisibility.Dark;
  Inventory inventory;
  public Furniture furniture { get; protected set; }
  // FIXME: This seems like a terrible way to flag if a job is pending
  // on a tile.  This is going to be prone to errors in set/clear.
  public Job pendingFurnitureJob;

  public World world { get; protected set; }

  public int X { get; protected set; }
  public int Y { get; protected set; }

  public float movementCost {
    get {

      if (Type == TileType.Empty)
        return 0; // 0 is unwalkable

      if (furniture == null)
        return 1;

      return 1 * furniture.movementCost;
    }
  }

  public Tile(World world, int x, int y) {
    this.world = world;
    this.X = x;
    this.Y = y;
  }

  public bool PlaceFurniture(Furniture objInstance) {
    if (objInstance == null) {
      furniture = null;
      return true;
    }

    if (furniture != null) {
      Debug.LogError("Trying to assign a furniture to a tile that already has one!");
      return false;
    }

    furniture = objInstance;
    return true;
  }

  public bool IsNeighbour(Tile tile, bool diagOkay = false) {
    return
      Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Y - tile.Y) == 1 ||  // Check hori/vert adjacency
      (diagOkay && (Mathf.Abs(this.X - tile.X) == 1 && Mathf.Abs(this.Y - tile.Y) == 1)) // Check diag adjacency
      ;
  }

  public Tile[] GetNeighbours(bool diagOkay = false) {
    Tile[] ns;

    if (diagOkay == false) {
      ns = new Tile[4]; // Tile order: N E S W
    } else {
      ns = new Tile[8]; // Tile order : N E S W NE SE SW NW
    }

    Tile n;

    n = world.GetTileAt(X, Y + 1);
    ns[0] = n;  // Could be null, but that's okay.
    n = world.GetTileAt(X + 1, Y);
    ns[1] = n;  // Could be null, but that's okay.
    n = world.GetTileAt(X, Y - 1);
    ns[2] = n;  // Could be null, but that's okay.
    n = world.GetTileAt(X - 1, Y);
    ns[3] = n;  // Could be null, but that's okay.

    if (diagOkay == true) {
      n = world.GetTileAt(X + 1, Y + 1);
      ns[4] = n;  // Could be null, but that's okay.
      n = world.GetTileAt(X + 1, Y - 1);
      ns[5] = n;  // Could be null, but that's okay.
      n = world.GetTileAt(X - 1, Y - 1);
      ns[6] = n;  // Could be null, but that's okay.
      n = world.GetTileAt(X - 1, Y + 1);
      ns[7] = n;  // Could be null, but that's okay.
    }

    return ns;
  }

  public void SetVisibility(TileVisibility visibility) {
    this.visibility = visibility;
    GameEvents.current.TileChanged(this);
  }

}
