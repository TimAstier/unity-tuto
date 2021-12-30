using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Area {
  public Vector2Int origin;
  public Vector2Int end;

  public Area(Vector2Int theOrigin, Vector2Int theEnd) {
    origin = theOrigin;
    end = theEnd;
  }
}

public class GenerateLevel {

  public const int NUMBER_0F_SPLITS = 3;
  public const int MINIMUM_ROOM_DIMENSION = 2;

  static Tile[,] CreateEmptyMap(World world, int width, int height, Action<Tile> tileChangedCb) {
    Tile[,] tiles = new Tile[width, height];

    for (int x = 0; x < width; x++) {
      for (int y = 0; y < height; y++) {
        tiles[x, y] = new Tile(world, x, y);
        tiles[x, y].RegisterTileTypeChangedCallback(tileChangedCb);
      }
    }
    return tiles;
  }

  static (Area, Area) HorizontalSplitArea(Area area) {
    Area first = new Area(new Vector2Int(area.origin.x, area.origin.y), new Vector2Int(area.end.x - 1, (int)(area.end.y + area.origin.y) / 2));
    Area second = new Area(new Vector2Int(area.origin.x, (int)((area.end.y + area.origin.y) / 2)), new Vector2Int(area.end.x - 1, area.end.y - 1));
    return (first, second);
  }

  static (Area, Area) VerticalSplitArea(Area area) {
    Area first = new Area(new Vector2Int(area.origin.x, area.origin.y), new Vector2Int((int)((area.end.x + area.origin.x) / 2), area.end.y - 1));
    Area second = new Area(new Vector2Int((int)((area.end.x + area.origin.x) / 2), area.origin.y), new Vector2Int(area.end.x - 1, area.end.y - 1));
    return (first, second);
  }

  static List<List<Area>> GetLeavesArray() {
    Area fullMap = new Area(new Vector2Int(0, 0), new Vector2Int(Constants.GridWidth, Constants.GridHeight));

    List<Area> leaves = new List<Area>();
    leaves.Add(fullMap);

    List<List<Area>> leavesList = new List<List<Area>>();

    bool toggle = Random.Range(0f, 1f) > 0.5 ? true : false;

    for (int i = 0; i <= NUMBER_0F_SPLITS; i++) {
      Func<Area, (Area, Area)> split;

      if (toggle) {
        split = HorizontalSplitArea;
      } else {
        split = VerticalSplitArea;
      }

      toggle = !toggle;

      List<Area> newLeaves = new List<Area>();

      foreach (Area leave in leaves) {
        (Area first, Area second) = split(leave);
        newLeaves.Add(first);
        newLeaves.Add(second);
      }

      leaves = newLeaves;
      leavesList.Add(leaves);
    }

    return leavesList;
  }

  static Area GetRandomAreaWithinArea(Area area) {
    int originalWidth = area.end.x - area.origin.x;
    int originalHeight = area.end.y - area.origin.y;

    int width = Random.Range(MINIMUM_ROOM_DIMENSION + 1, originalWidth);
    int height = Random.Range(MINIMUM_ROOM_DIMENSION + 1, originalHeight);

    int originX = Random.Range(area.origin.x, area.origin.x + originalWidth - width + 1);
    int originY = Random.Range(area.origin.y, area.origin.y + originalHeight - height + 1);

    Vector2Int origin = new Vector2Int(originX, originY);
    Vector2Int end = new Vector2Int(originX + width, originY + height);

    return new Area(origin, end);
  }

  static void PlaceRectangleOnMap(World world, Tile[,] map, Area area) {

    for (int i = area.origin.x; i <= area.end.x; i++) {
      for (int j = area.origin.y; j <= area.end.y; j++) {
        if (i == area.origin.x || i == area.end.x || j == area.origin.y || j == area.end.y) {
          map[i, j].Type = TileType.Floor;
          world.PlaceFurniture("Wall", map[i, j]);
        } else {
          map[i, j].Type = TileType.Floor;
        }
      }
    }
  }

  static List<Vector2Int> FindEmptyCellsInArea(Area area, Tile[,] map) {
    List<Vector2Int> positions = new List<Vector2Int>();
    for (int j = area.origin.y; j <= area.end.y; j++) {
      for (int i = area.origin.x; i <= area.end.x; i++) {
        if (map[j, i].Type == TileType.Floor && map[j, i].furniture == null) {
          positions.Add(new Vector2Int(i, j));
        }
      }
    }
    return positions;
  }

  // From https://www.redblobgames.com/grids/line-drawing.html#stepping
  static List<Vector2Int> WalkGrid(Vector2Int p0, Vector2Int p1) {
    int dx = p1[0] - p0[0];
    int dy = p1[1] - p0[1];
    int nx = Math.Abs(dx);
    int ny = Math.Abs(dy);
    int signX = dx > 0 ? 1 : -1;
    int signY = dy > 0 ? 1 : -1;

    Vector2Int p = new Vector2Int(p0.x, p0.y);
    List<Vector2Int> positions = new List<Vector2Int>();
    positions.Add(p);

    for (int ix = 0, iy = 0; ix < nx || iy < ny;) {
      if ((0.5 + ix) / nx < (0.5 + iy) / ny) {
        // next step is horizontal
        p.x += signX;
        ix++;
      } else {
        // next step is vertical
        p.y += signY;
        iy++;
      }
      positions.Add(p);
    }
    return positions;
  }

  static void ConnectLeaves(Area leafA, Area leafB, World world) {
    // Find one empty cell in boths areas
    List<Vector2Int> candidatesA = FindEmptyCellsInArea(leafA, world.tiles);
    Vector2Int positionA = candidatesA[(int)(Random.Range(0f, 1f) * candidatesA.Count)];

    List<Vector2Int> candidatesB = FindEmptyCellsInArea(leafB, world.tiles);
    Vector2Int positionB = candidatesB[(int)(Random.Range(0f, 1f) * candidatesB.Count)];

    // Get walking path between the two positions
    List<Vector2Int> positions = WalkGrid(positionA, positionB);

    // Dig tunnel between the two positions
    foreach (Vector2Int position in positions) {
      world.tiles[position[1], position[0]].Type = TileType.Floor;
      world.DestroyFurniture(world.tiles[position[1], position[0]]);
    }
  }

  static void ConnectAdjacentLeaves(List<List<Area>> leavesArray, World world, int index, int leavesDepth) {
    ConnectLeaves(
      leavesArray[leavesDepth][leavesArray[leavesDepth].Count - (index - 1)],
      leavesArray[leavesDepth][leavesArray[leavesDepth].Count - index],
      world
    );
  }

  static void ConnectAllLeaves(World world, List<List<Area>> leavesArray) {
    for (int i = 0; i <= NUMBER_0F_SPLITS; i++) {
      for (int j = 1; j <= Math.Pow(2, NUMBER_0F_SPLITS - i); j++) {
        ConnectAdjacentLeaves(leavesArray, world, j * 2, NUMBER_0F_SPLITS - i);
        if (i == NUMBER_0F_SPLITS) {
          ConnectAdjacentLeaves(leavesArray, world, j * 2, NUMBER_0F_SPLITS - i);
          ConnectAdjacentLeaves(leavesArray, world, j * 2, NUMBER_0F_SPLITS - i);
          ConnectAdjacentLeaves(leavesArray, world, j * 2, NUMBER_0F_SPLITS - i);
        }
      }
    }
  }

  static public Tile[,] GenerateMap(World world, Action<Tile> tileChangedCb) {
    // Get an empty map
    Tile[,] emptyMap = CreateEmptyMap(world, Constants.GridWidth, Constants.GridHeight, tileChangedCb);
    return emptyMap;
  }

  static public void CreateRandomLevel(World world) {
    // Cut empty map into leaves using Binary Space Partitioning (BSP) Trees
    List<List<Area>> leavesArray = GetLeavesArray();

    // Get random rooms from each leaf
    List<Area> rooms = leavesArray[NUMBER_0F_SPLITS].Select(leaf => GetRandomAreaWithinArea(leaf)).ToList();

    // Place rooms on the map
    Tile[,] resultMap = world.tiles;
    foreach (Area room in rooms) {
      PlaceRectangleOnMap(world, resultMap, room);
    }

    // Connect rooms
    ConnectAllLeaves(world, leavesArray);
  }

}
