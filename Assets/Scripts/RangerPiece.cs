using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangerPiece : Piece {

  public Color LIGHT_BLUE = new Color(134, 240, 233);

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

  public List<GameObject> getAttackableTiles() {
    List<GameObject> attackableTiles = new List<GameObject>();
    for (int i = -attackRange; i <= attackRange; i++) {
      GameObject tile = board.getCellAt(x+i, z);
      if (tile) {
        attackableTiles.Add(tile);
      }
      if (i != 0) {
        tile = board.getCellAt(x, z+i);
        if (tile) {
          attackableTiles.Add(tile);
        }
      }
    }
    return attackableTiles;  
  }

  public override void setAttackHighlights(bool onOrOff) {
    attacksHighlighted = onOrOff;
    foreach (Piece piece in getAttackablePieces()) {
      GameObject tile = board.getCellAt(piece.x, piece.z);
      piece.setColor(onOrOff ? Color.yellow : piece.baseColor);
    }
    foreach (GameObject tile in getAttackableTiles()) {
      TileController tc = tile.GetComponent<TileController>();
      tc.setColor(onOrOff ? Color.cyan : tc.baseColor);
    }
  }

  public override IEnumerator attack() {
    if (dead) {
      yield return null;
    } else {
      List<Piece> attackablePieces = getAttackablePieces();
      if (attackablePieces.Count == 0) {
        // If no attacks, just move on
        yield return null;
      } else {
        setAttackHighlights(true);
        GameObject selectedObject = null;
        Piece selectedPiece = null;
        while (!attackablePieces.Contains(selectedPiece)) {
          yield return null;
          while (!Input.GetMouseButtonDown(0)) {
            yield return null;
          }
          selectedObject = getSelectedObject();
          if (selectedObject) {
            selectedPiece = selectedObject.GetComponent<Piece>();
          }
        }

        damageEnemy(selectedPiece);
      }
      setAttackHighlights(false);
    }
  }
}
