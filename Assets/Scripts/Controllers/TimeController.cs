using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController {

  public static void TogglePause() {
    if (Time.timeScale == 0) {
      Time.timeScale = 1;
    } else {
      Time.timeScale = 0;
    }
  }
}
