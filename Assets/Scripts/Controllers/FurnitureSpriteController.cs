//=======================================================================
// Copyright Martin "quill18" Glaude 2015.
//		http://quill18.com
//=======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FurnitureSpriteController : MonoBehaviour {

  Dictionary<Furniture, GameObject> furnitureGameObjectMap;

  Dictionary<string, Sprite> furnitureSprites;

  World world {
    get { return WorldController.Instance.world; }
  }

  // Use this for initialization
  void Start() {
    LoadSprites();
    furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();
    GameEvents.current.onFurnitureCreated += OnFurnitureCreated;
    GameEvents.current.onFurnitureChanged += OnFurnitureChanged;
    GameEvents.current.onFurnitureDestroyed += OnFurnitureDestroyed;
  }

  private void OnDestroy() {
    GameEvents.current.onFurnitureCreated -= OnFurnitureCreated;
    GameEvents.current.onFurnitureChanged -= OnFurnitureChanged;
    GameEvents.current.onFurnitureDestroyed -= OnFurnitureDestroyed;
  }

  void LoadSprites() {
    furnitureSprites = new Dictionary<string, Sprite>();
    Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture/");

    foreach (Sprite s in sprites) {
      furnitureSprites[s.name] = s;
    }
  }

  public void OnFurnitureCreated(Furniture furn) {
    GameObject furn_go = new GameObject();

    furnitureGameObjectMap.Add(furn, furn_go);

    furn_go.name = furn.objectType + "_" + furn.tile.X + "_" + furn.tile.Y;
    furn_go.transform.position = new Vector3(furn.tile.X, furn.tile.Y, 0);
    furn_go.transform.SetParent(this.transform, true);

    SpriteRenderer sr = furn_go.AddComponent<SpriteRenderer>();
    sr.sprite = GetSpriteForFurniture(furn);
    sr.sortingLayerName = "Furnitures";
  }

  public void OnFurnitureDestroyed(Furniture furn) {
    furn.tile.PlaceFurniture(null);
    GameObject furn_go = furnitureGameObjectMap[furn];
    Destroy(furn_go);
  }

  void OnFurnitureChanged(Furniture furn) {

    if (furnitureGameObjectMap.ContainsKey(furn) == false) {
      Debug.LogError("OnFurnitureChanged -- trying to change visuals for furniture not in our map.");
      return;
    }

    GameObject furn_go = furnitureGameObjectMap[furn];
    furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
  }

  public Sprite GetSpriteForFurniture(Furniture obj) {
    if (obj.linksToNeighbour == false) {
      return furnitureSprites[obj.objectType];
    }

    string spriteName = obj.objectType + "_";

    // Check for neighbours North, East, South, West

    int x = obj.tile.X;
    int y = obj.tile.Y;

    Tile t;

    t = world.GetTileAt(x, y + 1);
    if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
      spriteName += "N";
    }
    t = world.GetTileAt(x + 1, y);
    if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
      spriteName += "E";
    }
    t = world.GetTileAt(x, y - 1);
    if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
      spriteName += "S";
    }
    t = world.GetTileAt(x - 1, y);
    if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType) {
      spriteName += "W";
    }

    if (furnitureSprites.ContainsKey(spriteName) == false) {
      Debug.LogError("GetSpriteForInstalledObject -- No sprites with name: " + spriteName);
      return null;
    }

    return furnitureSprites[spriteName];
  }


  public Sprite GetSpriteForFurniture(string objectType) {
    if (furnitureSprites.ContainsKey(objectType)) {
      return furnitureSprites[objectType];
    }

    if (furnitureSprites.ContainsKey(objectType + "_")) {
      return furnitureSprites[objectType + "_"];
    }

    Debug.LogError("GetSpriteForFurniture -- No sprites with name: " + objectType);
    return null;
  }
}
