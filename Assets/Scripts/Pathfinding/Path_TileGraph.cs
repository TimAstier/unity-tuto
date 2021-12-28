using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph {



  // This class constructs a simple path-finding compatible graph
  // of our world. Each tile is a node and each walklable neighbour
  // is linked though and edge connection.

  public Dictionary<Tile, Path_Node<Tile>> nodes;

  public Path_TileGraph(World world) {
    Debug.Log("Path_TileGraph");
    nodes = new Dictionary<Tile, Path_Node<Tile>>();

    for (int x = 0; x < world.Width; x++) {
      for (int y = 0; y < world.Height; y++) {
        Tile t = world.GetTileAt(x, y);
        if (t.movementCost > 0) {
          Path_Node<Tile> n = new Path_Node<Tile>();
          n.data = t;
          nodes.Add(t, n);
        }
      }
    }

    Debug.Log("Path_TileGraph: Created " + nodes.Count + "nodes.");

    int edgeCount = 0;

    foreach (Tile t in nodes.Keys) {
      Path_Node<Tile> n = nodes[t];

      List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

      // Get a list of neighbours

      Tile[] neighbours = t.GetNeighbours(true);

      for (int i = 0; i < neighbours.Length; i++) {
        if (neighbours[i] != null && neighbours[i].movementCost > 0) {
          Path_Edge<Tile> e = new Path_Edge<Tile>();
          e.cost = neighbours[i].movementCost;
          e.node = nodes[neighbours[i]];
          edges.Add(e);
          edgeCount++;
        }
      }

      n.edges = edges.ToArray();
    }
    Debug.Log("Path_TileGraph: Created " + edgeCount + "edges.");
  }
}
