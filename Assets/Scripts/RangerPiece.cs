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
      if (other) {
        break;
      }
      if (tile) {
        locations.Add(tile);
      }
    }
    // Right/Down
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x+i, z-i);
      other = board.getPieceAt(x+i, z-i);
      if (other) {
        break;
      }
      if (tile) {
        locations.Add(tile);
      }
    }
    // Left/Up
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x-i, z+i);
      other = board.getPieceAt(x-i, z+i);
      if (other) {
        break;
      }
      if (tile) {
        locations.Add(tile);
      }
    }
    // Right/Up
    for (int i = 1; i <= movementRange; i++) {
      tile = board.getCellAt(x+i, z+i);
      other = board.getPieceAt(x+i, z+i);
      if (other) {
        break;
      }
      if (tile) {
        locations.Add(tile);
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

  // Shoot a bullet object from this piece's location towards the given piece.
  protected void fireBulletAt(Piece piece) {
    Bullet bullet = (Bullet) Instantiate(bulletPrefab, transform.position + new Vector3(0,1,0), Quaternion.identity);
    bullet.creator = this;
  	if (attackHistogram.Length > 0) {
  		int index = Random.Range(0, attackHistogram.Length);
  		bullet.damage = attackHistogram[index];
  		attackFor = bullet.damage;
  	}
    bullet.velocity = bulletSpeed * (piece.transform.position - this.transform.position).normalized;
  }

  // AIattackOrCharge has to be overwritten since we're using bullets 
  // rather than a standard attack.
  public override IEnumerator AIattackOrCharge() {
    if (dead) {
      yield return null;
    } else {
      List<Piece> attackablePieces = getAttackablePieces();
      if (attackablePieces.Count == 0) {
        // If no attacks, just move on
        yield return null;
      } else {
        setAttackHighlights(true);
        if (Random.value < 0.5) {
          // attack
          yield return new WaitForSeconds(1.5f);
          Piece selectedPiece = attackablePieces[0];
          int minEnemyHP = attackablePieces[0].currentHP;
          foreach (Piece p in attackablePieces) {
            if (p.currentHP < minEnemyHP) {
              minEnemyHP = p.currentHP;
              selectedPiece = p;
            }
          }
          if (!dead) {
            fireBulletAt(selectedPiece);
          }
        } else {
          // special
          yield return new WaitForSeconds(0.5f);
          if (!dead) {
            if (currentSpecial < maxSpecial) {
              incrementSpecial();
            } else {
              yield return StartCoroutine(AIspecialAttack());
            }
          }
        }
      }
      setAttackHighlights(false);
    }
  }

  // attackOrCharge also has to be overwritten since we're using bullets 
  // rather than a standard attack.
  public override IEnumerator attackOrCharge() {

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
          if (dead) {
            break;
          }
          yield return null;
          while (!Input.GetMouseButtonDown(0) && !Input.GetKeyUp("space")) {
            yield return null;
          }
          if (Input.GetMouseButtonDown(0)) {
            selectedObject = getSelectedObject();
            if (selectedObject) {
              selectedPiece = selectedObject.GetComponent<Piece>();
            }
          } else if (Input.GetKeyUp("space")) {
            if (currentSpecial < maxSpecial) {
              incrementSpecial();
            } else {
              yield return StartCoroutine(specialAttack());
            }
            break;
          }
        }
        if (selectedPiece) {
          fireBulletAt(selectedPiece);
        }
      }
      setAttackHighlights(false);
    }
  }
}
