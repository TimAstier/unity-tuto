using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

  public GameObject circleCursorPrefab;

  // The world-position of the mouse last frame.
  Vector3 lastFramePosition;
  Vector3 currFramePosition;

  // The world-position start of our left-mouse drag operation
  Vector3 dragStartPosition;
  List<GameObject> dragPreviewGameObjects;

  Vector3 hoverPosition;
  GameObject mouseHoverGameObject;

  // Use this for initialization
  void Start() {
    dragPreviewGameObjects = new List<GameObject>();
  }

  // Update is called once per frame
  void Update() {
    currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    currFramePosition.z = 0;

    GameMode gameMode = WorldController.Instance.world.gameMode;

    UpdateMouseHover(gameMode);
    UpdateDragging(gameMode);
    UpdateMoveCharacter(gameMode);

    UpdateCameraMovement();

    // Save the mouse position from this frame
    // We don't use currFramePosition because we may have moved the camera.
    lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    lastFramePosition.z = 0;
  }

  void UpdateMouseHover(GameMode gameMode) {

    if (gameMode == GameMode.Select) {
      if (mouseHoverGameObject != null) {
        SimplePool.Despawn(mouseHoverGameObject);
        mouseHoverGameObject = null;
      }
      return;
    }

    if (gameMode == GameMode.Build) {
      int currTileX = Mathf.FloorToInt(currFramePosition.x);
      int currTileY = Mathf.FloorToInt(currFramePosition.y);

      if (currTileX != hoverPosition.x || currTileY != hoverPosition.y) {
        hoverPosition = new Vector3(currTileX, currTileY, 0);
        GameObject go = SimplePool.Spawn(circleCursorPrefab, new Vector3(currTileX, currTileY, 0), Quaternion.identity);
        go.transform.SetParent(this.transform, true);

        if (mouseHoverGameObject != null) {
          GameObject previousGo = mouseHoverGameObject;
          mouseHoverGameObject = null;
          SimplePool.Despawn(previousGo);
        }

        mouseHoverGameObject = go;
      }
    }
  }

  void UpdateDragging(GameMode gameMode) {
    if (gameMode != GameMode.Build) {
      return;
    }

    // If we're over a UI element, then bail out from this.
    if (EventSystem.current.IsPointerOverGameObject()) {
      return;
    }

    // Start Drag
    if (Input.GetMouseButtonDown(0)) {
      dragStartPosition = currFramePosition;
    }

    int start_x = Mathf.FloorToInt(dragStartPosition.x);
    int end_x = Mathf.FloorToInt(currFramePosition.x);
    int start_y = Mathf.FloorToInt(dragStartPosition.y);
    int end_y = Mathf.FloorToInt(currFramePosition.y);

    // We may be dragging in the "wrong" direction, so flip things if needed.
    if (end_x < start_x) {
      int tmp = end_x;
      end_x = start_x;
      start_x = tmp;
    }
    if (end_y < start_y) {
      int tmp = end_y;
      end_y = start_y;
      start_y = tmp;
    }

    // Clean up old drag previews
    while (dragPreviewGameObjects.Count > 0) {
      GameObject go = dragPreviewGameObjects[0];
      dragPreviewGameObjects.RemoveAt(0);
      SimplePool.Despawn(go);
    }

    if (Input.GetMouseButton(0)) {
      // Display a preview of the drag area
      for (int x = start_x; x <= end_x; x++) {
        for (int y = start_y; y <= end_y; y++) {
          Tile t = WorldController.Instance.world.GetTileAt(x, y);
          if (t != null) {
            // Display the building hint on top of this tile position
            GameObject go = SimplePool.Spawn(circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
            go.transform.SetParent(this.transform, true);
            dragPreviewGameObjects.Add(go);
          }
        }
      }
    }

    // End Drag
    if (Input.GetMouseButtonUp(0)) {

      BuildModeController bmc = GameObject.FindObjectOfType<BuildModeController>();

      // Loop through all the tiles
      for (int x = start_x; x <= end_x; x++) {
        for (int y = start_y; y <= end_y; y++) {
          Tile t = WorldController.Instance.world.GetTileAt(x, y);

          if (t != null) {
            bmc.DoBuild(t);
          }
        }
      }
    }
  }

  void UpdateCameraMovement() {
    Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
    Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, Constants.CAMERA_MIN_ZOOM, Constants.CAMERA_MAX_ZOOM);
  }

  void UpdateMoveCharacter(GameMode gameMode) {
    if (gameMode != GameMode.Select) {
      return;
    }

    if (Input.GetMouseButtonUp(1)) {
      int currTileX = Mathf.FloorToInt(currFramePosition.x);
      int currTileY = Mathf.FloorToInt(currFramePosition.y);
      Character character = WorldController.Instance.world.characters.First();
      if (character != null) {
        character.SetDestination(WorldController.Instance.world.GetTileAt(currTileX, currTileY));
      }
    }
  }

}
