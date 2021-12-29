﻿//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Collections;
using UnityEngine;

// TileType is the base type of the tile. In some tile-based games, that might be
// the terrain type. For us, we only need to differentiate between empty space
// and floor (a.k.a. the station structure/scaffold). Walls/Doors/etc... will be
// InstalledObjects sitting on top of the floor.
public enum TileType { Empty, Floor };

public class Tile {
  private TileType _type = TileType.Empty;
  public TileType Type {
    get { return _type; }
    set {
      TileType oldType = _type;
      _type = value;
      // Call the callback and let things know we've changed.

      if (cbTileChanged != null && oldType != _type)
        cbTileChanged(this);
    }
  }

  // LooseObject is something like a drill or a stack of metal sitting on the floor
  Inventory inventory;

  // Furniture is something like a wall, door, or sofa.
  public Furniture furniture {
    get; protected set;
  }

  public Job pendingFurnitureJob;

  // We need to know the context in which we exist. Probably. Maybe.
  public World world { get; protected set; }

  public int X { get; protected set; }
  public int Y { get; protected set; }

  public float movementCost {
    get {
      if (Type == TileType.Empty) {
        return 0;
      }
      if (furniture == null) {
        return 1;
      }
      return 1 * furniture.movementCost;
    }
  }

  // The function we callback any time our type changes
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

  /// <summary>
  /// Register a function to be called back when our tile type changes.
  /// </summary>
  public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
    cbTileChanged += callback;
  }

  /// <summary>
  /// Unregister a callback.
  /// </summary>
  public void UnregisterTileTypeChangedCallback(Action<Tile> callback) {
    cbTileChanged -= callback;
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

  public bool IsNeighbour(Tile tile, bool diagOkay = false) {
    return Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Y - tile.Y) == 1 ||  // Check hori/vert adjacency
  (diagOkay && (Mathf.Abs(this.X - tile.X) == 1 && Mathf.Abs(this.Y - tile.Y) == 1)) // Check diag adjacency
  ;
  }

  public Tile[] GetNeighbours(bool diagOkay = false) {
    Tile[] ns;

    if (diagOkay == false) {
      ns = new Tile[4]; // Tile order: N E S W
    } else {
      ns = new Tile[8]; // Tile order: N E S W NE SE SW NW
    }

    Tile n;

    n = world.GetTileAt(X, Y + 1);
    ns[0] = n;
    n = world.GetTileAt(X + 1, Y);
    ns[1] = n;
    n = world.GetTileAt(X, Y - 1);
    ns[2] = n;
    n = world.GetTileAt(X - 1, Y);
    ns[3] = n;

    if (diagOkay == true) {
      n = world.GetTileAt(X + 1, Y + 1);
      ns[4] = n;
      n = world.GetTileAt(X + 1, Y - 1);
      ns[5] = n;
      n = world.GetTileAt(X - 1, Y - 1);
      ns[6] = n;
      n = world.GetTileAt(X - 1, Y + 1);
      ns[7] = n;
    }

    return ns;
  }

}
