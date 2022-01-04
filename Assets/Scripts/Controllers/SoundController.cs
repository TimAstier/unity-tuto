using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour {
  float soundCooldown = 0;
  public AudioSource pauseAudioSource;
  public AudioSource playAudioSource;

  // Use this for initialization
  void Start() {
    GameEvents.current.onTileTypeChanged += OnTileTypeChanged;
    GameEvents.current.onFurnitureCreated += OnFurnitureCreated;
    GameEvents.current.onToggledPause += OnToggledPause;
  }

  private void OnDestroy() {
    GameEvents.current.onTileTypeChanged -= OnTileTypeChanged;
    GameEvents.current.onFurnitureCreated -= OnFurnitureCreated;
    GameEvents.current.onToggledPause -= OnToggledPause;
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

  public void OnToggledPause(float time) {
    if (time == 0f) {
      pauseAudioSource.Play();
    } else if (time == 1f) {
      playAudioSource.Play();
    }
  }

}
