using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
  public enum TileType { Empty, Floor };

  TileType type = TileType.Empty;

  public TileType Type
  {
    get
    {
      return type;
    }
    set
    {
      type = value;
      // Call the callback and let know about the change.
    }
  }

  LooseObject looseObject;
  InstalledObject installedObject;

  World world;
  int x;

  public int X
  {
    get
    {
      return x;
    }
  }
  int y;

  public int Y
  {
    get
    {
      return y;
    }
  }

  // Constructor, never returns anything

  public Tile(World world, int x, int y)
  {
    this.world = world;
    this.x = x;
    this.y = y;
  }
}
