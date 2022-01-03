using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour {

  Dictionary<Character, GameObject> characterGameObjectMap;

  Dictionary<string, Sprite> characterSprites;

  World world {
    get { return WorldController.Instance.world; }
  }

  void Start() {
    LoadSprites();

    characterGameObjectMap = new Dictionary<Character, GameObject>();

    GameEvents.current.onCharacterCreated += OnCharacterCreated;
    GameEvents.current.onCharacterChanged += OnCharacterChanged;
  }

  void LoadSprites() {
    characterSprites = new Dictionary<string, Sprite>();
    Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/");

    foreach (Sprite s in sprites) {
      characterSprites[s.name] = s;
    }
  }

  public void OnCharacterCreated(Character c) {
    GameObject char_go = new GameObject();

    characterGameObjectMap.Add(c, char_go);

    char_go.name = "Character";
    char_go.transform.position = new Vector3(c.X, c.Y, 0);
    char_go.transform.SetParent(this.transform, true);

    SpriteRenderer sr = char_go.AddComponent<SpriteRenderer>();
    sr.sprite = characterSprites["p1_front"];
    sr.sortingLayerName = "Characters";
  }

  void OnCharacterChanged(Character c) {

    if (characterGameObjectMap.ContainsKey(c) == false) {
      Debug.LogError("OnCharacterChanged -- trying to change visuals for character not in our map.");
      return;
    }

    GameObject char_go = characterGameObjectMap[c];
    char_go.transform.position = new Vector3(c.X, c.Y, 0);
  }



}
