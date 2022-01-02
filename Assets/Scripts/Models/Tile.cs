//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Collections;
using UnityEngine;

// TileType is the base type of the tile. In some tile-based games, that might be
// the terrain type. For us, we only need to differentiate between empty space
// and floor.
public enum TileType { Empty, Floor };
public enum TileVisibility { Clear, Dim, Dark };

public class Tile {
  private TileType _type = TileType.Empty;
  public TileType Type {
    get { return _type; }
    set {
      TileType oldType = _type;
      _type = value;
      // Call the callback and let things know we've changed type.

      if (cbTileTypeChanged != null && oldType != _type) {
        cbTileTypeChanged(this);
      }
    }
  }

  public TileVisibility visibility { get; protected set; } = TileVisibility.Dark;

  // LooseObject is something like a drill or a stack of metal sitting on the floor
  Inventory inventory;

  // Furniture is something like a wall, door, or sofa.
  public Furniture furniture {
    get; protected set;
  }

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

  // The function we callback any time our tile's data changes
  Action<Tile> cbTileTypeChanged;
  Action<Tile> cbTileChanged;

  /// <summary>
  /// Initializes a new instance of the <see cref="Tile"/> class.
  /// </summary>
  /// <param name="world">A World instance.</param>
  /// <param name="x">The x coordinate.</param>
  /// <param name="y">The y coordinate.</param>
  public Tile(World world, int x, int y) {
    this.world = world;
    this.X = x;
    this.Y = y;
  }


  public void RegisterTileChangedCallback(Action<Tile> callback) {
    cbTileChanged += callback;
  }

  public void UnregisterTileChangedCallback(Action<Tile> callback) {
    cbTileChanged -= callback;
  }

  public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
    cbTileTypeChanged += callback;
  }

  public void UnregisterTileTypeChangedCallback(Action<Tile> callback) {
    cbTileTypeChanged -= callback;
  }

  public bool PlaceFurniture(Furniture objInstance) {
    if (objInstance == null) {
      // We are uninstalling whatever was here before.
      furniture = null;
      return true;
    }

    // objInstance isn't null

    if (furniture != null) {
      Debug.LogError("Trying to assign a furniture to a tile that already has one!");
      return false;
    }

    // At this point, everything's fine!

    furniture = objInstance;
    return true;
  }

  // Tells us if two tiles are adjacent.
  public bool IsNeighbour(Tile tile, bool diagOkay = false) {
    // Check to see if we have a difference of exactly ONE between the two
    // tile coordinates.  Is so, then we are vertical or horizontal neighbours.
    return
      Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Y - tile.Y) == 1 ||  // Check hori/vert adjacency
      (diagOkay && (Mathf.Abs(this.X - tile.X) == 1 && Mathf.Abs(this.Y - tile.Y) == 1)) // Check diag adjacency
      ;
  }

  /// <summary>
  /// Gets the neighbours.
  /// </summary>
  /// <returns>The neighbours.</returns>
  /// <param name="diagOkay">Is diagonal movement okay?.</param>
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
    ReportTileChanged();
  }

  private void ReportTileChanged() {
    // Call the callback and let things know we've changed.
    if (cbTileChanged != null) {
      cbTileChanged(this);
    }
  }

}
