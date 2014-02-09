using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour {
	public GameObject cube;
	public int xDimension = 20;
	public int zDimension = 20;
	private GameObject[,]grid;
	// Use this for initialization
	void Start () {
		grid = new GameObject[xDimension, zDimension];
		for(int i = 0; i< xDimension; i++){
			for(int j = 0; j< zDimension; j++){
				Vector3 position = Vector3.right*i + Vector3.forward*j;
				GameObject t = (GameObject)Instantiate(cube, position, Quaternion.identity);
				//t.Translate(new Vector3(0.1f*i, 0, 0.1f*j));
				if(((i+j) % 2) == 0) {
					t.transform.GetComponent<TileController>().setBaseColor(Color.black);
				}else {
					t.transform.GetComponent<TileController>().setBaseColor(Color.white);
				}
				t.transform.GetComponent<TileController>().x = i;
				t.transform.GetComponent<TileController>().z = j;
				grid[i, j] = t;
				
			}
		}
		BuildTerrain();
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

	public GameObject getCellAt(int x, int z) {
		if (x < 0 || x >= xDimension || z < 0 || z >= zDimension) {
			return null;
		} else {
			return grid[x,z];
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
