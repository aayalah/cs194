using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangerPiece : Piece {

  public Color LIGHT_BLUE = new Color(134, 240, 233);
  public Bullet bulletPrefab;
  public float bulletSpeed = 3;

  // Move on diagonals...
  public override List<GameObject> getMoveLocations() {
    List<GameObject> locations = new List<GameObject> ();
    GameObject tile;
    Piece other;
    // Center
    locations.Add(board.getCellAt(x,z));
    // Left/Down
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x-i, z-i);
      other = board.getPieceAt(x-i, z-i);
      if (tile) {
        locations.Add(tile);
      }
      if (other) {
        break;
      }
    }
    // Right/Down
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x+i, z-i);
      other = board.getPieceAt(x+i, z-i);
      if (tile) {
        locations.Add(tile);
      }
      if (other) {
        break;
      }
    }
    // Left/Up
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x-i, z+i);
      other = board.getPieceAt(x-i, z+i);
      if (tile) {
        locations.Add(tile);
      }
      if (other) {
        break;
      }
    }
    // Right/Up
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x+i, z+i);
      other = board.getPieceAt(x+i, z+i);
      if (tile) {
        locations.Add(tile);
      }
      if (other) {
        break;
      }
    }
    
    return locations; 
  }
  // ... attack on rows/columns.
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
	/*
  public override IEnumerator attack() {

    if (dead) {
      yield return null;
    } else {
      List<Piece> attackablePieces = getAttackablePieces();
      if (attackablePieces.Count == 0) {
        // If no attacks, just move on
        Debug.Log("There are no pieces this RangerPiece can attack");
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
       Bullet bullet = (Bullet) Instantiate(bulletPrefab, transform.position + new Vector3(0,1,0), Quaternion.identity);
       bullet.creator = this;
       bullet.velocity = bulletSpeed * (selectedPiece.transform.position - this.transform.position).normalized;
      }
      setAttackHighlights(false);
    }
  }*/
}
