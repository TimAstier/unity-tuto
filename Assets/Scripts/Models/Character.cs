using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character {

    public float X {
        get {
            return Mathf.Lerp(currTile.X, destTile.X, movementPercentage);
        }
    }

    public float Y {
        get {
            return Mathf.Lerp(currTile.Y, destTile.Y, movementPercentage);
        }
    }

    public Tile currTile {
        get; protected set;
    }
    Tile destTile;

    float movementPercentage;
    float speed = 2f; // Tiles per second

    Action<Character> cbCharacterChanged;

    Job myJob;

    public Character(Tile tile) {
        currTile = destTile = tile;
    }

    public void Update(float deltaTime) {

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
            return;
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X - destTile.X, 2) + Mathf.Pow(currTile.Y - destTile.Y, 2));
        float distanceThisFrame = speed * deltaTime;
        float percThisFrame = distanceThisFrame / distToTravel;
        movementPercentage += percThisFrame;

        if (movementPercentage >= 1) {
            currTile = destTile;
            movementPercentage = 0;
        }

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
