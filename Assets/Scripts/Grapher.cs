using UnityEngine;
using System.Collections;

public class Grapher : MonoBehaviour {
	public Transform bar;
	public Material mat;
	
	private int[,] values;
	private Transform[,] bars;
	public int[,] colorIndex;
	public Color[,] colorMix;
	
	private int currentGraph;
	private int currentColor;
	
	private int numGraphs = 3;
	private int numColors = 5;
	private float prev = 0;
	public int numBars = 25;
	// Use this for initialization
	void Start () {
		colorMix = new Color[numGraphs,numColors];
		resetColorMix();
		bars = new Transform[numGraphs,numBars];
		values = new int[numGraphs,numBars];
		Vector3 position = Vector3.up;
		SetColors();
		for(int g = 0; g < numGraphs; g++){
			for(int i = 0; i<numBars/*numBars*/; i++){
				float randomNumber = Random.Range(max(prev-10, 0f), min(prev+20, 50f));
				float multiplier = Random.Range(0.1f, 1.0f);
				values[g, i] = Mathf.CeilToInt(randomNumber*multiplier);
				Transform t = (Transform)Instantiate(bar, position, Quaternion.identity);
				t.transform.localScale = new Vector3(.5f, randomNumber*multiplier+1, .5f);
				t.transform.Translate(0, randomNumber*multiplier/2f, 0);
				t.gameObject.renderer.material.color = GetColorMix(i);
				bars[g,i] = t;
				if(g != currentGraph)
					t.gameObject.renderer.enabled = false;
				position = position + Vector3.right*.55f;
			}
			position = Vector3.up;
		}
		for(int i = 0; i < numColors; i++){
			
		}
		
	}
	void resetColorMix(){
		for(int l = 0; l < numGraphs; l++){
			for(int i = 0; i < numColors; i++){
				colorMix[l,i] = Color.clear;
			}
		}
	}
	
	void SetColors(){
		colorIndex = new int[numGraphs, numColors];
		int prevIndex = -1;
		for(int c = 0; c < numGraphs; c++){
			for(int i = 0; i < numColors; i++){
				int index = 0;
				int slice = (numBars/numColors);
				index = (int)Mathf.Floor(Random.Range(prevIndex+1, slice*(i+1)));
				colorIndex[c,i] = ((i == numColors-1)) ? numBars : index;
				prevIndex = index;
			}
		}
		ScrambleColors();
	}
	
	void ScrambleColors(){
		for(int c = 0; c < numGraphs; c++){
		for(int i = 0; i < numColors; i++){
			int count = 0;
			while(count < 200){
				int index = Random.Range(0, numColors);
				if(colorMix[c,index] == Color.clear){
					colorMix[c,index] = GetColor(i);
					break;
				}
				count++;
			}
		}
		}
	}
	
	Color GetColor(int i){
		Debug.Log("color: "+ i);
		switch(i){
		case 0: return Color.red;
		case 1: return Color.yellow;
		case 2: return Color.green;
		case 3: return Color.blue;
		case 4: return Color.magenta;
		default: return Color.black;
		}
		//return Color.black;
	}
	
	Color GetColorMix(int i){
		if(i<= colorIndex[currentColor,0]) return colorMix[currentColor,0];
		if(i<= colorIndex[currentColor,1]) return colorMix[currentColor,1];
		if(i<= colorIndex[currentColor,2]) return colorMix[currentColor,2];
		if(i<= colorIndex[currentColor,3]) return colorMix[currentColor,3];
		if(i<= colorIndex[currentColor,4]) return colorMix[currentColor,4];
		return Color.black;
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.RightArrow)){
			currentGraph = currentGraph+1;
			if(currentGraph >= numGraphs) currentGraph = 0;
			drawNewGraph();	
			Debug.Log(currentGraph);
		}
		if(Input.GetKeyUp(KeyCode.LeftArrow)){
			currentGraph = currentGraph-1;
			if(currentGraph < 0) currentGraph = numGraphs-1;
			drawNewGraph();
		}
					
		if(Input.GetKeyUp(KeyCode.UpArrow)){
			currentColor = currentColor+1;
			if(currentColor >= numGraphs) currentColor = 0;
			drawNewGraph();	
		}
		if(Input.GetKeyUp(KeyCode.DownArrow)){
			currentColor = currentColor-1;
			if(currentColor < 0) currentColor = numGraphs-1;
			drawNewGraph();
		}
	}
				
	void OnGUI(){
		GUI.Label(new Rect(Screen.width/2-50, Screen.height-125, 300, 200), "Current Graph #"+(currentGraph+1));
		GUI.Label(new Rect(Screen.width/2-60, Screen.height-100, 300, 200), "Current Color Distribution #"+(currentColor+1));
	}
			
	void drawNewGraph(){
		for(int i = 0; i < numBars/*numBars*/; i++){
			Transform oldBar;
			if(currentGraph != 0){
				oldBar = bars[currentGraph-1, i];
			}else{
				oldBar = bars[numGraphs-1, i];	
			}
			oldBar.gameObject.renderer.enabled = false;
			Transform newBar = bars[currentGraph, i];
			newBar.gameObject.renderer.material.color = GetColorMix(i);
			newBar.gameObject.renderer.enabled = true;
			}	
	}
	
	float max(float a, float b){
		 return a > b ? a : b;
	}
	float min(float a, float b){
		return a < b ? a : b;
	}
}
