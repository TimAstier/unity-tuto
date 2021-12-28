using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph {

    // This class constructs a simple path-finding compatible graph
    // of our world. Each tile is a node and each walklable neighbour
    // is linked though and edge connection.

    Dictionary<Tile, Path_Node<Tile>> nodes;

    public Path_TileGraph(World world) {
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

        foreach(Tile t in nodes.Keys) {
            // Get a list of neighbours
        }
    }
}
