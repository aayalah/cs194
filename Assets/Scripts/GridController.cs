using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour {
	public Transform cube;
	public int xDimension = 20;
	public int yDimension = 20;
	private Transform[,]grid;
	// Use this for initialization
	void Start () {
		grid = new Transform[xDimension, yDimension];
		for(int i = 0; i< xDimension; i++){
			for(int j = 0; j< yDimension; j++){
				Vector3 position = Vector3.right*i + Vector3.forward*j;
				Transform t = (Transform)Instantiate(cube, position, Quaternion.identity);
				//t.Translate(new Vector3(0.1f*i, 0, 0.1f*j));
				if(((i+j) % 2) == 0) {
					t.transform.GetComponent<TileController>().setBaseColor(Color.black);
				}else {
					t.transform.GetComponent<TileController>().setBaseColor(Color.white);
				}
				t.transform.GetComponent<TileController>().x = i;
				t.transform.GetComponent<TileController>().y = j;
				grid[i, j] = t;
				
			}
		}
		BuildTerrain();
	}
	
	void BuildTerrain(){
		for(int i = 0; i < 100; i++){
			int row = Random.Range(0, xDimension);
			int col = Random.Range(0, yDimension);
			Transform t = grid[row, col];
			t.transform.localScale = t.transform.localScale+Vector3.up*.5f;
			t.Translate(Vector3.up*.25f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
