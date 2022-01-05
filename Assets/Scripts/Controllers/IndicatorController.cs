using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class IndicatorController : MonoBehaviour {

  public GameObject moveCursorPrefab;

  void OnEnable() {
    // moveCursorPrefab.SetActive(false);
    GameEvents.current.onDestinationChanged += OnDestinationChanged;
  }

  void OnDestroy() {
    GameEvents.current.onDestinationChanged -= OnDestinationChanged;
  }

  void OnDestinationChanged(Tile tile) {
    // moveCursorPrefab.SetActive(true);
    moveCursorPrefab.transform.position = new Vector3(tile.X, tile.Y, 0);
  }
}


