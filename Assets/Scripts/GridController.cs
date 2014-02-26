using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour {
	public Material grass;
	public Material flowers;
	
	public GameObject cube1;
	public GameObject cube2;
	public int xDimension = 10;
	public int zDimension = 10;
	private GameObject[,]grid;
	private Piece[,] pieceGrid;

	private UnitManager man;
	// Use this for initialization
	void Start () {
		man = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		if (man.w > 0) {
						xDimension = man.w;
		}
		if (man.h > 0) {
						zDimension = man.h;
		}

		grid = new GameObject[xDimension, zDimension];
		pieceGrid = new Piece[xDimension, zDimension];
		for(int i = 0; i< xDimension; i++){
			for(int j = 0; j< zDimension; j++){
				Vector3 position = Vector3.right*i + Vector3.forward*j;
				GameObject t; 
				//t.Translate(new Vector3(0.1f*i, 0, 0.1f*j));
				if(((i+j) % 2) == 0) {
					t = (GameObject)Instantiate(cube1, position, Quaternion.identity);
					//t.transform.renderer.material = grass;
					//t.transform.GetComponent<TileController>().setBaseColor(Color.black);
				}else {
					t = (GameObject)Instantiate(cube2, position, Quaternion.identity);
					//t.transform.renderer.material = flowers;
					//t.transform.GetComponent<TileController>().setBaseColor(Color.white);
				}
				t.transform.GetComponent<TileController>().x = i;
				t.transform.GetComponent<TileController>().z = j;
				grid[i, j] = t;
				pieceGrid[i,j] = null;
				
			}
		}
		//BuildTerrain();
	}
	
	void BuildTerrain(){
		for(int i = 0; i < 100; i++){
			int row = Random.Range(0, xDimension);
			int col = Random.Range(0, zDimension);
			GameObject t = grid[row, col];
			t.transform.localScale = t.transform.localScale+Vector3.up*.5f;
			t.transform.Translate(Vector3.up*.25f);
		}
	}

	public float maxHeightOnPath(int startX, int startZ, int endX, int endZ) {
		int minX = Mathf.Min(startX, endX);
		int maxX = Mathf.Max(startX, endX);
		int minZ = Mathf.Min(startZ, endZ);
		int maxZ = Mathf.Max(startZ, endZ);
		if (minX < 0 || maxX >= xDimension || minZ < 0 || maxZ >= zDimension) {
			return 0;
		} else {
			float maxHeight = 0;
			GameObject localCell;
			float localHeight;
			if (startX == endX) {
				// vertical
				for (int j = minZ; j <= maxZ; j++) {
					localCell = grid[startX, j];
					localHeight = localCell.transform.position.y + localCell.transform.localScale.y / 2;
					if (localHeight > maxHeight) {
						maxHeight = localHeight;
					}
				}
			} else if (startZ == endZ) {
				// horizontal
				for (int i = minX; i <= maxX; i++) {
					localCell = grid[i, startZ];
					localHeight = localCell.transform.position.y + localCell.transform.localScale.y / 2;
					if (localHeight > maxHeight) {
						maxHeight = localHeight;
					}
				}
			} else if (maxX - minX == maxZ - minZ) {
				// diagonal
				for (int i = minX; i <= maxX; i++) {
					for (int j = -1; j <= 1; j++) {
						if (i + j >= 0 && i + j < zDimension) {
							localCell = grid[i, i + j];
							localHeight = localCell.transform.position.y + localCell.transform.localScale.y / 2;
							if (localHeight > maxHeight) {
								maxHeight = localHeight;
							}
						}
					}
				}
			} else {
				// assume whole rectangle because why the heck not
				for (int i = minX; i <= maxX; i++) {
					for (int j = minZ; j <= maxZ; j++) {
						localCell = grid[i, j];
						localHeight = localCell.transform.position.y + localCell.transform.localScale.y / 2;
						if (localHeight > maxHeight) {
							maxHeight = localHeight;
						}						
					}
				}
			}

			return maxHeight;
		}
	}


	public GameObject getCellAt(int x, int z) {
		if (x < 0 || x >= xDimension || z < 0 || z >= zDimension) {
			return null;
		} else {
			return grid[x,z];
		}
	}

	public Piece getPieceAt(int x, int z) {
		if (x < 0 || x >= xDimension || z < 0 || z >= zDimension) {
			return null;
		} else {
			return pieceGrid[x,z];
		}
	}

	public bool cellIsFree(int x, int z, Piece pieceToIgnore) {
		return getCellAt(x, z) && (getPieceAt(x,z) == pieceToIgnore || !getPieceAt(x,z));
	}

	public void movePiece(Piece piece, int newX, int newZ) {
		if (newX != piece.x || newZ != piece.z) {
			pieceGrid[newX, newZ] = piece;
			pieceGrid[piece.x, piece.z] = null;
		}
	}

	public void removePiece(Piece piece) {
		pieceGrid[piece.x, piece.z] = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
