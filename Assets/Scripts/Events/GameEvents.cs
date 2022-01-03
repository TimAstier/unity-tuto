using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour {
  public static GameEvents current;

  // TODO: DRY cbs checks

  private void Awake() {
    current = this;
  }

  public event Action<Tile> onTileTypeChanged;
  public void TileTypeChanged(Tile tile) {
    if (onTileTypeChanged != null) {
      onTileTypeChanged(tile);
    }
  }

  public event Action<Tile> onTileChanged;
  public void TileChanged(Tile tile) {
    if (onTileChanged != null) {
      onTileChanged(tile);
    }
  }

  public event Action<Furniture> onFurnitureCreated;
  public void FurnitureCreated(Furniture furniture) {
    if (onFurnitureCreated != null) {
      onFurnitureCreated(furniture);
    }
  }

  public event Action<Furniture> onFurnitureChanged;
  public void FurnitureChanged(Furniture furniture) {
    if (onFurnitureChanged != null) {
      onFurnitureChanged(furniture);
    }
  }

  public event Action<Furniture> onFurnitureDestroyed;
  public void FurnitureDestroyed(Furniture furniture) {
    if (onFurnitureDestroyed != null) {
      onFurnitureDestroyed(furniture);
    }
  }

  public event Action<Character> onCharacterCreated;
  public void CharacterCreated(Character character) {
    if (onCharacterCreated != null) {
      onCharacterCreated(character);
    }
  }

  public event Action<Character> onCharacterChanged;
  public void CharacterChanged(Character character) {
    if (onCharacterChanged != null) {
      onCharacterChanged(character);
    }
  }
}
