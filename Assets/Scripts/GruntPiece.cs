using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GruntPiece : Piece {

  // Move on rows/columns
  public override List<GameObject> getMoveLocations() {
    List<GameObject> locations = new List<GameObject> ();
    GameObject tile;
    Piece other;
    // Center
    locations.Add(board.getCellAt(x,z));
    // Left
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x-i, z);
      other = board.getPieceAt(x-i, z);
      if (other) {
        break;
      } else if (tile) {
        locations.Add(tile);
      }
    }
    // Right
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x+i, z);
      other = board.getPieceAt(x+i, z);
      if (other) {
        break;
      } else if (tile) {
        locations.Add(tile);
      }
    }
    // Down 
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x, z-i);
      other = board.getPieceAt(x, z-i);
      if (other) {
        break;
      } else if (tile) {
        locations.Add(tile);
      }
    }
    // Up
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x, z+i);
      other = board.getPieceAt(x, z+i);
      if (other) {
        break;
      } else if (tile) {
        locations.Add(tile);
      }
    }
    
    return locations;
  }

  // ... and attack on rows/columns, too.
  public override List<GameObject> getAttackableTiles() {
    List<GameObject> locations = new List<GameObject> ();
    GameObject tile;
    Piece other;

    // Left
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x-i, z);
      other = board.getPieceAt(x-i, z);
      if (tile) {
        locations.Add(tile);
      }
      if (other) {
        break;
      }
    }
    // Right
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x+i, z);
      other = board.getPieceAt(x+i, z);
      if (tile) {
        locations.Add(tile);
      }
      if (other) {
        break;
      }
    }
    // Down 
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x, z-i);
      other = board.getPieceAt(x, z-i);
      if (tile) {
        locations.Add(tile);
      }
      if (other) {
        break;
      }
    }
    // Up
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x, z+i);
      other = board.getPieceAt(x, z+i);
      if (tile) {
        locations.Add(tile);
      }
      if (other) {
        break;
      }
    }
    
    return locations; 
  }

}
