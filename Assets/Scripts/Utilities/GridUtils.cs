using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtils {

  public static bool IsInsideCircle(Vector2Int center, Vector2Int tile, float radius) {
    float dx = center.x - tile.x,
          dy = center.y - tile.y;
    float distance_squared = dx * dx + dy * dy;
    return distance_squared <= radius * radius;
  }

  static int DiagonalDistance(Vector2Int p0, Vector2Int p1) {
    int dx = p1.x - p0.x;
    int dy = p1.y - p0.y;
    return Math.Max(Math.Abs(dx), Math.Abs(dy));
  }

  static Vector2Int RoundPosition(Vector2 p) {
    return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
  }

  static float Lerp(float start, float end, float t) {
    return start + t * (end - start);
  }

  static Vector2 LerpPoint(Vector2 p0, Vector2 p1, float t) {
    return new Vector2(Lerp(p0.x, p1.x, t), Lerp(p0.y, p1.y, t));
  }

  public static List<Vector2Int> Line(Vector2Int p0, Vector2Int p1) {
    List<Vector2Int> positions = new List<Vector2Int>();
    int N = DiagonalDistance(p0, p1);
    for (int step = 0; step <= N; step++) {
      float t = N == 0 ? 0f : (float)step / (float)N;
      positions.Add(RoundPosition(LerpPoint(p0, p1, t)));
    }
    return positions;
  }

}
