using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWarController : MonoBehaviour {

  public Tilemap fogOfWarTilememap;
  public TileBase dimTile;
  public TileBase darkTile;

  // Start is called before the first frame update
  void Start() {
    if (Constants.ENABLE_FOW == true) {
      WorldController.Instance.world.RegisterTileChanged(OnTileChanged);
      WorldController.Instance.world.RegisterCharacterCreated(OnCharacterCreated);

      fogOfWarTilememap.size = new Vector3Int(Constants.GRID_WIDTH, Constants.GRID_WIDTH, 0);
      fogOfWarTilememap.BoxFill(new Vector3Int(0, 0, 0), darkTile, 0, 0, Constants.GRID_WIDTH, Constants.GRID_HEIGHT);
    }
  }

  // Update is called once per frame
  void Update() { }

  public void UpdateVisibility(TileVisibility visibility, Vector2Int position) {
    if (visibility == TileVisibility.Clear) {
      fogOfWarTilememap.SetTile(new Vector3Int(position.x, position.y, 0), null);
    } else if (visibility == TileVisibility.Dim) {
      fogOfWarTilememap.SetTile(new Vector3Int(position.x, position.y, 0), dimTile);
    } else if (visibility == TileVisibility.Dark) {
      fogOfWarTilememap.SetTile(new Vector3Int(position.x, position.y, 0), darkTile);
    }
  }

  void OnTileChanged(Tile tile_data) {
    FogOfWarController fc = GameObject.FindObjectOfType<FogOfWarController>();
    fc.UpdateVisibility(tile_data.visibility, new Vector2Int(tile_data.X, tile_data.Y));
  }

  void OnCharacterCreated(Character character) {
    Tile tile_data = character.currTile;
    tile_data.SetVisibility(TileVisibility.Clear);
  }
}
