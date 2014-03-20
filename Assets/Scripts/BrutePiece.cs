using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrutePiece : Piece {

  // Move within radius
  public override List<GameObject> getMoveLocations() {
    // Default movement is, let's say... everything forward, backward, left, and right.
    List<GameObject> locations = new List<GameObject> ();
    for (int i = -movementRange; i <= movementRange; i++) {
      for(int j = -movementRange; j <= movementRange; j++){
        if(Mathf.Abs(i)+Mathf.Abs(j) <= movementRange){
          if (board.cellIsFree(x+i, z+j, this)) {
            if(!locations.Contains(board.getCellAt(x+i, z+j)))
              locations.Add(board.getCellAt (x+i, z+j));
          }
        }
      }
    }
    return locations;
  }


  // And attack within radius, too.
  public override List<GameObject> getAttackableTiles() {
    List<GameObject> tiles = new List<GameObject> ();
    for (int i = x - attackRange; i <= x + attackRange; i++) {
      for (int j = z - attackRange; j <= z + attackRange; j++) {
        if (Mathf.Sqrt((x-i)*(x-i) + (z-j)*(z-j)) <= attackRange) {
          GameObject attackableTile = board.getCellAt (i, j);
          if (attackableTile) {
            tiles.Add (attackableTile);
          }
        }
      }
    }
    return tiles;
  }   
}