using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

    float X {
        get {
            return Mathf.Lerp(currTile.X, destTile.X, movementPercentage);
        }
    }

    float Y {
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

    public Character(Tile tile) {
        currTile = destTile = tile;
    }

    public void Update(float deltaTime) {
        if (currTile == destTile) {
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
    }

    public void SetDestination(Tile tile) {
        if (currTile.IsNeighboor(tile, true) == false) {
            Debug.Log("Destination tile is not a neighbour");
        }
        destTile = tile;
    }
}
