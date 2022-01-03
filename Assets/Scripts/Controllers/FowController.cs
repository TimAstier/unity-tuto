using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FowController : MonoBehaviour {

  public Tilemap fowTilemap;
  public TileBase dimTile;
  public TileBase darkTile;

  // Start is called before the first frame update
  void Start() {
    if (Constants.ENABLE_FOW == true) {
      GameEvents.current.onTileChanged += OnTileChanged;
      GameEvents.current.onCharacterCreated += OnCharacterCreated;

      fowTilemap.size = new Vector3Int(Constants.GRID_WIDTH, Constants.GRID_WIDTH, 0);
      fowTilemap.BoxFill(new Vector3Int(0, 0, 0), darkTile, 0, 0, Constants.GRID_WIDTH, Constants.GRID_HEIGHT);
    }
  }

  private void OnDestroy() {
    if (Constants.ENABLE_FOW == true) {
      GameEvents.current.onTileChanged -= OnTileChanged;
      GameEvents.current.onCharacterCreated -= OnCharacterCreated;
    }
  }

  // Update is called once per frame
  void Update() { }

  public void UpdateVisibility(TileVisibility visibility, Vector2Int position) {
    if (visibility == TileVisibility.Clear) {
      fowTilemap.SetTile(new Vector3Int(position.x, position.y, 0), null);
    } else if (visibility == TileVisibility.Dim) {
      fowTilemap.SetTile(new Vector3Int(position.x, position.y, 0), dimTile);
    } else if (visibility == TileVisibility.Dark) {
      fowTilemap.SetTile(new Vector3Int(position.x, position.y, 0), darkTile);
    }
  }

  void OnTileChanged(Tile tile) {
    FowController fc = GameObject.FindObjectOfType<FowController>();
    fc.UpdateVisibility(tile.visibility, new Vector2Int(tile.X, tile.Y));
  }

  void OnCharacterCreated(Character character) {
    Tile tile_data = character.currTile;
    tile_data.SetVisibility(TileVisibility.Clear);
  }
}
