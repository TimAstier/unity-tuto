using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
// TODO: What's a monobehavior?
{
  public Sprite floorSprite;

  World world;

  // Start is called before the first frame update
  void Start()
  {
    // Create a world with empty tiles
    world = new World();

    for (int x = 0; x < world.Width; x++)
    {
      for (int y = 0; y < world.Height; y++)
      {
        Tile tile_data = world.GetTileAt(x, y);
        GameObject tile_go = new GameObject();
        tile_go.name = "Tile_" + x + "_" + y;
        tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
        // We add a sprite renderer, but no need to set a sprite
        // because all tiles are empty for now. 
        tile_go.AddComponent<SpriteRenderer>();
      }
    }
    world.RandomizeTiles();
  }

  float randomizeTileTimer = 2f;

  // Update is called once per frame
  void Update()
  {
    randomizeTileTimer -= Time.deltaTime;

    if (randomizeTileTimer < 0)
    {
      world.RandomizeTiles();
      randomizeTileTimer = 2f;
    }
  }

  void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
  {
    if (tile_data.Type == Tile.TileType.Floor)
    {
      tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
    }
    else if (tile_data.Type == Tile.TileType.Empty)
    {
      tile_go.GetComponent<SpriteRenderer>().sprite = null;
    }
    else
    {
      Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
    }
  }
}
