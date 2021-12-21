using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

  public GameObject circleCursor;

  Vector3 lastFramePosition;
  Vector3 dragStartPosition;

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

    Vector3 currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    currFramePosition.z = 0;

    // Update the circle cursor position
    Tile tileUnderMouse = GetTileAtWorldCoord(currFramePosition);
    if (tileUnderMouse != null) {
      circleCursor.SetActive(true);
      Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
      circleCursor.transform.position = cursorPosition;
    } else {
      circleCursor.SetActive(false);
    }

    // Start Drag

    if (Input.GetMouseButtonDown(0)) {
      dragStartPosition = currFramePosition;
    }

    // End Drag
    if (Input.GetMouseButtonUp(0)) {
      int start_x = Mathf.FloorToInt(dragStartPosition.x);
      int end_x = Mathf.FloorToInt(currFramePosition.x);
      if (end_x < start_x) {
        int temp = start_x;
        start_x = end_x;
        end_x = temp;
      }

      int start_y = Mathf.FloorToInt(dragStartPosition.y);
      int end_y = Mathf.FloorToInt(currFramePosition.y);
      if (end_y < start_y) {
        int temp = start_y;
        start_y = end_y;
        end_y = temp;
      }

      for (int x = start_x; x <= end_x; x++) {
        for (int y = start_y; y <= end_y; y++) {
          Tile t = WorldController.Instance.World.GetTileAt(x, y);
          t.Type = Tile.TileType.Floor;
        }
      }
    }

    // Handle screen dragging
    if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
      Vector3 diff = lastFramePosition - currFramePosition;
      Camera.main.transform.Translate(diff);
    }

    lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    lastFramePosition.z = 0;
  }

  Tile GetTileAtWorldCoord(Vector3 coord) {
    int x = Mathf.FloorToInt(coord.x);
    int y = Mathf.FloorToInt(coord.y);

    return WorldController.Instance.World.GetTileAt(x, y);
  }
}
