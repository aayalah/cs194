using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangerPiece : Piece {

  // Move on diagonals...
  public override List<GameObject> getMoveLocations() {
    List<GameObject> locations = new List<GameObject> ();
    for (int i = -movementRange; i <= movementRange; i++) {
      if (board.cellIsFree(x+i, z+i, this)) {
        if(!locations.Contains(board.getCellAt(x+i, z+i))) {
          locations.Add(board.getCellAt (x+i, z+i));
        }
      }
      if (board.cellIsFree(x+i, z-i, this)) {
        if(!locations.Contains(board.getCellAt(x+i, z-i))) {
          locations.Add(board.getCellAt (x+i, z-i));
        }
      }
    }
    return locations;
  }

  // ... attack on rows/columns.
  public override List<Piece> getAttackablePieces() {
    List<Piece> pieces = new List<Piece> ();
    for (int i = -attackRange; i <= attackRange; i++) {
      Piece attackablePiece = board.getPieceAt(x+i, z);
      if (attackablePiece && attackablePiece.player != this.player) {
        pieces.Add (attackablePiece);
      }
      if (i != 0) {
        attackablePiece = board.getPieceAt(x, z+i);
        if (attackablePiece && attackablePiece.player != this.player) {
          pieces.Add (attackablePiece);
        }     
      }
    }
    return pieces;
  }
}
