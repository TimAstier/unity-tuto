using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

  public float X {
    get {
      return Mathf.Lerp(currTile.X, nextTile.X, movementPercentage);
    }
  }

  public float Y {
    get {
      return Mathf.Lerp(currTile.Y, nextTile.Y, movementPercentage);
    }
  }

  public Tile currTile {
    get; protected set;
  }
  Tile destTile;
  Tile nextTile; // The next tile in the pathfinding sequence.
  Path_AStar pathAStar;
  float movementPercentage;
  float speed = 2f; // Tiles per second

  Action<Character> cbCharacterChanged;

  Job myJob;

  void Update_DoJob(float deltaTime) {
    if (myJob == null) {
      myJob = currTile.world.jobQueue.Dequeue();

      if (myJob != null) {
        destTile = myJob.tile;
        myJob.RegisterJobCancelCallback(OnJobEnded);
        myJob.RegisterJobCompleteCallback(OnJobEnded);
      }
    }

    if (currTile == destTile) {
      if (myJob != null) {
        myJob.DoWork(deltaTime);
      }
    }
  }

  public void AbandonJob() {
    nextTile = destTile = currTile;
    pathAStar = null;
    currTile.world.jobQueue.Enqueue(myJob);
    myJob = null;
  }

  void Update_DoMovement(float deltaTime) {
    if (currTile == destTile) {
      pathAStar = null;
      return;
    }

    if (nextTile == null || nextTile == currTile) {
      if (pathAStar == null || pathAStar.Length() == 0) {
        pathAStar = new Path_AStar(currTile.world, currTile, destTile);
        if (pathAStar.Length() == 0) {
          Debug.LogError("Path_AStar returned no path to destination!");
          AbandonJob();
          pathAStar = null;
          return;
        }
      }
      nextTile = pathAStar.Dequeue();
      if (nextTile == currTile) {
        Debug.LogError("Update_DoMovement -- nextTile is currTile?");
      }
    }

    float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X - destTile.X, 2) + Mathf.Pow(currTile.Y - destTile.Y, 2));
    float distanceThisFrame = speed * deltaTime;
    float percThisFrame = distanceThisFrame / distToTravel;
    movementPercentage += percThisFrame;

    if (movementPercentage >= 1) {
      currTile = nextTile;
      movementPercentage = 0;
    }
  }

  public Character(Tile tile) {
    currTile = destTile = nextTile = tile;
  }

  public void Update(float deltaTime) {
    Update_DoJob(deltaTime);
    Update_DoMovement(deltaTime);
    if (cbCharacterChanged != null) {
      cbCharacterChanged(this);
    }
  }

  public void SetDestination(Tile tile) {
    if (currTile.IsNeighbour(tile, true) == false) {
      Debug.Log("Destination tile is not a neighbour");
    }
    destTile = tile;
  }

  public void RegisterOnChangedCallback(Action<Character> cb) {
    cbCharacterChanged += cb;
  }

  public void UnregisterOnChangedCallback(Action<Character> cb) {
    cbCharacterChanged -= cb;
  }

  void OnJobEnded(Job j) {
    if (j != myJob) {
      Debug.LogError("Character being told about job that isn't his. You forgot to unregister something.");
      return;
    }
    myJob = null;
  }
}
