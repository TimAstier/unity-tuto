using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour {

  public float baseSpeed = 1.0f;

  void Start() { }

  void Update() {
    float zoom = Camera.main.orthographicSize;
    float speed = baseSpeed * (zoom - 1);

    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
      Camera.main.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
    }
    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q)) {
      Camera.main.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
    }
    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
      Camera.main.transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
    }
    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z)) {
      Camera.main.transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
    }
  }
}
