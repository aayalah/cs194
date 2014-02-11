using UnityEngine;
using System.Collections;

public class TeamBuilder : MonoBehaviour {
	public UnitManager manager;
	public Transform bar;
	public Material mat;
	
	public Transform brute;
	public Transform grunt;
	public Transform ranger;
	private Transform[] units;
	
	private int[,] values;
	private Transform[,] bars;
	public int[,] colorIndex;
	public Color[,] colorMix;
	
	private int currentGraph;
	private int currentColor;
	
	private int unitNum = 1;
	private int armySize = 3;
	private int unitsCreated = 0;
	private int numGraphs = 3;
	private int numColors = 3;
	private float prev = 0;
	public int numBars = 25;
	// Use this for initialization
	void Start () {
		manager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		colorMix = new Color[numGraphs,numColors];
		resetColorMix();
		bars = new Transform[numGraphs,numBars];
		values = new int[numGraphs,numBars];
		units = new Transform[3];
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
		
		setUpUnitSelect();
		
	}
	
	void setUpUnitSelect(){
		brute = (Transform)Instantiate(brute, new Vector3(40f, 5f, 2.5f), Quaternion.identity);
		brute.transform.localScale = new Vector3(5f, 5f, 5f);
		grunt = (Transform)Instantiate(grunt, new Vector3(40f, 5f, 2.5f), Quaternion.identity);
		grunt.gameObject.renderer.enabled = false;
		grunt.transform.localScale = new Vector3(5f, 5f, 5f);
		ranger = (Transform)Instantiate(ranger, new Vector3(40f, 5f, 2.5f), Quaternion.identity);
		ranger.transform.localScale = new Vector3(5f, 5f, 5f);
		ranger.gameObject.renderer.enabled = false;
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
		int prevIndex = 0;
		for(int c = 0; c < numGraphs; c++){
			for(int i = 0; i < numColors; i++){
				int index = 0;
				int slice = (numBars/(numColors));
				index = (int)Random.Range((prevIndex+2), (prevIndex+4));
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
			if(numGraphs > 0) drawNewGraph();	
		}
		if(Input.GetKeyUp(KeyCode.LeftArrow)){
			currentGraph = currentGraph-1;
			if(currentGraph < 0) currentGraph = numGraphs-1;
			if(numGraphs > 0) drawNewGraph();
		}
					
		if(Input.GetKeyUp(KeyCode.UpArrow)){
			currentColor = currentColor+1;
			if(currentColor >= numGraphs) currentColor = 0;
			if(numGraphs > 0) drawNewGraph();	
		}
		if(Input.GetKeyUp(KeyCode.DownArrow)){
			currentColor = currentColor-1;
			if(currentColor < 0) currentColor = numGraphs-1;
			if(numGraphs > 0)drawNewGraph();
		}
		if(Input.GetKeyUp(KeyCode.Alpha1)){
			unitNum = 1;
			brute.renderer.enabled = true;
			grunt.renderer.enabled = false;
			ranger.renderer.enabled = false;
		}
		if(Input.GetKeyUp(KeyCode.Alpha2)){
			unitNum = 2;
			brute.renderer.enabled = false;
			grunt.renderer.enabled = true;
			ranger.renderer.enabled = false;
		}
		if(Input.GetKeyUp(KeyCode.Alpha3)){
			unitNum = 3;
			brute.renderer.enabled = false;
			grunt.renderer.enabled = false;
			ranger.renderer.enabled = true;
		}
	}
				
	void OnGUI(){
		GUI.Label(new Rect(Screen.width/10, Screen.height-130, 200, 100), "Current Graph #"+(currentGraph+1));
		GUI.Label(new Rect(Screen.width/10, Screen.height-115, 200, 100), "Current Color Distribution #"+(currentColor+1));
		
		GUI.Label(new Rect(Screen.width/10, Screen.height-100, 200, 100), "Avg. Attack: "+averageSkill(Color.red));
		GUI.Label(new Rect(Screen.width/10+150, Screen.height-100, 200, 100), "Max Attack: "+maxSkill(Color.red));
		GUI.Label(new Rect(Screen.width/10, Screen.height-85, 200, 100), "Avg. Shield: "+averageSkill(Color.green));
		GUI.Label(new Rect(Screen.width/10+150, Screen.height-85, 200, 100), "Max Shield: "+maxSkill(Color.green));
		GUI.Label(new Rect(Screen.width/10, Screen.height-70, 200, 100), "Avg. Special: "+averageSkill(Color.yellow));
		GUI.Label(new Rect(Screen.width/10+150, Screen.height-70, 200, 100), "Max Special: "+maxSkill(Color.yellow));
		
		UnitLabels();
		
		if(unitsCreated < armySize){
			if (GUI.Button (new Rect (Screen.width/2-75,Screen.height/2-50,150,100), "Create Unit", GUI.skin.GetStyle("button"))) {
				int [] attack = findArray(Color.red);
				int [] shield = findArray(Color.green);
				int [] special = findArray(Color.yellow);
				manager.addUnit(unitNum, attack, shield, special);
				unitsCreated++;
				removeCombination();
			}
		}else{
			if (GUI.Button (new Rect (Screen.width/2-75,Screen.height/2-50,150,100), "Confirm", GUI.skin.GetStyle("button"))) {
				manager.teamsBuilt++;
				Application.LoadLevel(0);
			}
		}
	}
	
	void UnitLabels(){
		if(brute.renderer.enabled){
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-130, 200, 100), "Unit Type: Brute");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-115, 200, 100), "Hit Points: 100");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-100, 200, 100), "Attack Range: 1");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-85, 200, 100), "Movement: 4");
		}
		if(grunt.renderer.enabled){
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-130, 200, 100), "Unit Type: Grunt");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-115, 200, 100), "Hit Points: 50");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-100, 200, 100), "Attack Range: 3");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-85, 200, 100), "Movement: 5");
		}
		if(ranger.renderer.enabled){
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-130, 200, 100), "Unit Type: Ranger");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-115, 200, 100), "Hit Points: 20");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-100, 200, 100), "Attack Range: 6");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-85, 200, 100), "Movement: 8");
		}
	}
	
	void removeCombination(){
		numGraphs--;
		for(int i = currentGraph; i < numGraphs; i++){
			for(int j = 0; j < numBars; j++){
				bars[i,j].renderer.enabled = false;
				bars[i,j] = bars[i+1, j];
			}
			for(int k = 0; k < numColors; k++){
				colorMix[i, k] = colorMix[i+1, k];
				colorIndex[i, k] = colorIndex[i+1, k];
			}
		}
		if(numGraphs > 0){
		currentGraph = currentGraph+1;
		if(currentGraph >= numGraphs) currentGraph = 0;
		currentColor = currentColor+1;
		if(currentColor >= numGraphs) currentColor = 0;
		drawNewGraph();	
		}
	}
	
	
	int [] findArray(Color skill){
		int count = 0;
		for(int i = 0; i < numBars; i++){
			if(GetColorMix(i).Equals(skill)){
				count++;
			}
		}
		int[] ret = new int[count];
		for(int j = 0; j < count; j++){
			if(GetColorMix(j).Equals(skill)){
				ret[j] = values[currentGraph,j];
			}
		}
		return ret;
	}
	
	float averageSkill(Color skill){
		int counter = 0;
		int sum = 0;
		for(int i = 0; i < numBars; i++){
			if(GetColorMix(i).Equals(skill)){
				sum += values[currentGraph, i];
				counter++;
			}
		}
		return Mathf.Round(((float)sum/(float)counter)*100f)/100f;
	}
	
	float maxSkill(Color skill){
		int max = 0;
		for(int i = 0; i < numBars; i++){
			if(GetColorMix(i).Equals(skill)){
				if(values[currentGraph, i] > max){
					max = values[currentGraph, i];
				}
			}
		}
		return max;
	}
			
	void drawNewGraph(){
		for(int i = 0; i < numBars; i++){
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
