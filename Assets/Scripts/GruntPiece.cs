using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GruntPiece : Piece {

  // Move on rows/columns
  public override List<GameObject> getMoveLocations() {
    List<GameObject> locations = new List<GameObject> ();
    for (int i = -movementRange; i <= movementRange; i++) {
      GameObject tile = board.getCellAt(x+i, z);
      if (tile) {
        locations.Add (tile);
      }
      if (i != 0) {
        tile = board.getCellAt(x, z+i);
        if (tile) {
          locations.Add (tile);
        }
      }
    }
    return locations;
  }

  // ... and attack on rows/columns, too.
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
