using System;
using System.Collections;
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

  public Tile currTile { get; protected set; }
  Tile nextTile;
  Tile destTile;
  Path_AStar pathAStar;
  float movementPercentage; // Goes from 0 to 1 as we move from currTile to nextTile
  float speed = Constants.CHARACTER_BASE_SPEED; // Tiles per second
  Job myJob;

  public Character(Tile tile) {
    currTile = destTile = nextTile = tile;
  }

  void Update_DoJob(float deltaTime) {
    if (myJob == null) {
      myJob = currTile.world.jobQueue.Dequeue();
      if (myJob != null) {
        // TODO: Check to see if the job is REACHABLE!
        destTile = myJob.tile;
        myJob.RegisterJobCompleteCallback(OnJobEnded);
        myJob.RegisterJobCancelCallback(OnJobEnded);
      }
    }

    if (myJob != null && currTile == myJob.tile) {
      //if(pathAStar != null && pathAStar.Length() == 1)	{ // We are adjacent to the job site.
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
      return; // We're already were we want to be.
    }

    if (nextTile == null || nextTile == currTile) {
      // Get the next tile from the pathfinder.
      if (pathAStar == null || pathAStar.Length() == 0) {
        // Generate a path to our destination
        pathAStar = new Path_AStar(currTile.world, currTile, destTile); // This will calculate a path from curr to dest.
        if (pathAStar.Length() == 0) {
          Debug.LogError("Path_AStar returned no path to destination!");
          AbandonJob();
          pathAStar = null;
          return;
        }
      }

      // Grab the next waypoint from the pathing system!
      nextTile = pathAStar.Dequeue();

      if (nextTile == currTile) {
        Debug.LogError("Update_DoMovement - nextTile is currTile?");
      }
    }

    float distToTravel = Mathf.Sqrt(
      Mathf.Pow(currTile.X - nextTile.X, 2) +
      Mathf.Pow(currTile.Y - nextTile.Y, 2)
    );

    // How much distance can be travel this Update?
    float distThisFrame = speed * deltaTime;

    // How much is that in terms of percentage to our destination?
    float percThisFrame = distThisFrame / distToTravel;

    // Add that to overall percentage travelled.
    movementPercentage += percThisFrame;

    if (movementPercentage >= 1) {
      currTile = nextTile;
      GameEvents.current.CharacterMoved(this);
      movementPercentage = 0;
    }
  }

  public void Update(float deltaTime) {
    Update_DoJob(deltaTime);
    Update_DoMovement(deltaTime);
    GameEvents.current.CharacterChanged(this);
  }

  public void SetDestination(Tile tile) {
    destTile = tile;
  }

  void OnJobEnded(Job j) {
    // Job completed or was cancelled.

    if (j != myJob) {
      Debug.LogError("Character being told about job that isn't his. You forgot to unregister something.");
      return;
    }

    myJob = null;
  }
}
