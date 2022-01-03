﻿using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour {
  float soundCooldown = 0;

  // Use this for initialization
  void Start() {
    GameEvents.current.onTileTypeChanged += OnTileTypeChanged;
    GameEvents.current.onFurnitureCreated += OnFurnitureCreated;
  }

  // Update is called once per frame
  void Update() {
    soundCooldown -= Time.deltaTime;
  }

  void OnTileTypeChanged(Tile tile) {
    if (soundCooldown > 0)
      return;

    AudioClip ac = Resources.Load<AudioClip>("Sounds/Floor_OnCreated");
    AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
    soundCooldown = 0.1f;
  }

  public void OnFurnitureCreated(Furniture furn) {
    if (soundCooldown > 0)
      return;

    AudioClip ac = Resources.Load<AudioClip>("Sounds/" + furn.objectType + "_OnCreated");

    if (ac == null) {
      ac = Resources.Load<AudioClip>("Sounds/Wall_OnCreated");
    }

    AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
    soundCooldown = 0.1f;
  }
}
