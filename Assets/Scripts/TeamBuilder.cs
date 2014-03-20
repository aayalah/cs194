using UnityEngine;
using System.Collections;

public class TeamBuilder : MonoBehaviour {
	public GUISkin skin;
	public UnitManager manager;
	public Transform bar;
	public Material mat;
	public HealthBar health;

	//prefabs for each of the unit types (of each color)
	public Transform redBumblebee;
	public Transform redWorker;
	public Transform redHornet;
	public Transform blueBumblebee;
	public Transform blueWorker;
	public Transform blueHornet;


	//These will be redBumblebee, redWorker, and redHornet for player1's build phase; blue for player2
	private Transform currentWorker;
	private Transform currentHornet;
	private Transform currentBumble;
	
	private Transform[] units;

	//integers corresponding to the index of the graph being displayed and the index of the color distribution being displayed
	private int currentGraph;
	private int currentColor;


	//private variables for graphs and color distributions
	private int[,] values;
	private Transform[,] bars;
	public int[,] colorIndex;
	public Color[,] colorMix;

	private int[] nextGraph;
	private int[] nextColor;
	private int[] prevGraph;
	private int[] prevColor;
	
	private int unitNum = 1;
	private int armySize;
	private int unitsCreated = 0;
	private int numGraphs;
	private int numColors = 3;
	private int statPoints = 250;
	private int numBars = 25;

	public bool usingAI = false;
	public int personalityType = 0;
	
	/*Randomizes all histograms and color distrobutions and stores them in 2D arrays*/
	void Start () {
		if(usingAI) personalityType = Random.Range(0, 2);
		manager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		armySize = manager.armySize;
		numGraphs = armySize+2;
		colorMix = new Color[numGraphs,numColors];
		resetColorMix();
		bars = new Transform[numGraphs,numBars];
		values = new int[numGraphs,numBars];
		units = new Transform[armySize];
		initGraphOrder();
		initColorOrder();
		Vector3 position = Vector3.up;
		SetColors();
		initializeGraphs();
		for(int g = 0; g < numGraphs; g++){
			for(int i = 0; i < statPoints; i++){
				int barNum = Random.Range(0, numBars-1);
				values[g, barNum] += 1;
			}
			for(int k = 0; k < numBars; k++){
				Transform t = (Transform)Instantiate(bar, position, Quaternion.identity);
				t.transform.localScale = new Vector3(.5f, values[g, k], .5f);
				t.transform.Translate(3, values[g, k]/2f, 0);
				t.gameObject.renderer.material.color = GetColorMix(k);
				bars[g,k] = t;
				if(g != currentGraph)
					t.gameObject.renderer.enabled = false;
				position = position + Vector3.right*.55f;
			}
			position = Vector3.up;
		}
		for(int i = 0; i < numColors; i++){
			
		}
		
		setUpUnitSelect();
		if(usingAI){
			for(int i = 0; i < armySize; i++){
				int rand = Random.Range(1, 4);
				unitNum = rand;
				StartCoroutine(AIcreateNewUnit(rand));
			}
			
		}
		
	}
	/*Initialized the array of histograms to default values*/
	void initializeGraphs(){
		for(int g = 0; g < numGraphs; g++){
			for(int i = 0; i < numBars; i++){
				values[g, i] = 1;
			}
		}
	}

	/*Creates an array which keeps track of relative order of remaining graphs*/
	void initGraphOrder(){
		nextGraph = new int[numGraphs];
		prevGraph = new int[numGraphs];
		for(int i = 0; i < numGraphs; i++){
			nextGraph[i] = i+1;
			prevGraph[i] = i-1;
			if(i == numGraphs-1)
				nextGraph[i] = 0;
			if(i == 0)
				prevGraph[i] = numGraphs-1;
		}
	}

	/*Creates an array which keeps track of relative order of remaining colors*/
	void initColorOrder(){
		nextColor = new int[numGraphs];
		prevColor = new int[numGraphs];
		for(int i = 0; i < numGraphs; i++){
			prevColor[i] = i-1;
			nextColor[i] = i+1;
			if(i == 0)
				prevColor[i] = numGraphs-1;
			if(i == numGraphs -1)
				nextColor[i] = 0;
		}
	}


	/*Displays red flavored units if player 1's build phase, blue units if player 2's build phase*/
	void setUpUnitSelect(){
		if(manager.teamsBuilt == 0){
		Transform redB = (Transform)Instantiate(redBumblebee, new Vector3(36f, 1f, 5f), Quaternion.identity);
		redB.transform.localScale = new Vector3(6f, 6f, 6f);
		Transform redW = (Transform)Instantiate(redWorker, new Vector3(35f, 1f, 2.5f), Quaternion.identity);
		redW.transform.localScale = new Vector3(5f, 5f, 5f);
		redW.GetComponentInChildren<Renderer>().enabled = false;
		Transform redH = (Transform)Instantiate(redHornet, new Vector3(35f, 1f, 2.5f), Quaternion.identity);
		redH.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
		redH.GetComponentInChildren<Renderer>().enabled = false;

		currentWorker = redW;
		currentHornet = redH;
		currentBumble = redB;

		}else if(manager.teamsBuilt == 1){
			Transform blueB = (Transform)Instantiate(blueBumblebee, new Vector3(36f, 1f, 5f), Quaternion.identity);
			blueB.transform.localScale = new Vector3(6f, 6f, 6f);
			Transform blueW = (Transform)Instantiate(blueWorker, new Vector3(35f, 1f, 2.5f), Quaternion.identity);
			blueW.transform.localScale = new Vector3(5f, 5f, 5f);
			blueW.GetComponentInChildren<Renderer>().enabled = false;
			Transform blueH = (Transform)Instantiate(blueHornet, new Vector3(35f, 1f, 2.5f), Quaternion.identity);
			blueH.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
			blueH.GetComponentInChildren<Renderer>().enabled = false;
		
			currentWorker = blueW;
			currentHornet = blueH;
			currentBumble = blueB;
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
		int prevIndex = 0;
		for(int c = 0; c < numGraphs; c++){
			for(int i = 0; i < numColors; i++){
				int index = (int)Random.Range((prevIndex+1), numBars-numColors*2+i*2);
				colorIndex[c,i] = ((i == numColors-1)) ? numBars-1 : index;
				prevIndex = index;
			}
			prevIndex = 0;
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
		if(!usingAI){

		if(Input.GetKeyUp(KeyCode.RightArrow)){
			currentGraph = nextGraph[currentGraph];
			if(numGraphs > 0) drawNewGraph();	
		}
		if(Input.GetKeyUp(KeyCode.LeftArrow)){
			currentGraph = prevGraph[currentGraph];
			if(numGraphs > 0) drawNewGraph();
		}
					
		if(Input.GetKeyUp(KeyCode.UpArrow)){
			currentColor = nextColor[currentColor];
			if(numGraphs > 0) drawNewGraph();	
		}
		if(Input.GetKeyUp(KeyCode.DownArrow)){
			currentColor = prevColor[currentColor];
			if(numGraphs > 0)drawNewGraph();
		}
		if(Input.GetKeyUp(KeyCode.Alpha1)){
			unitNum = 1;
			currentBumble.GetComponentInChildren<Renderer>().enabled = true;
			currentWorker.GetComponentInChildren<Renderer>().enabled = false;
			currentHornet.GetComponentInChildren<Renderer>().enabled = false;
		}
		if(Input.GetKeyUp(KeyCode.Alpha2)){
			unitNum = 2;
			currentBumble.GetComponentInChildren<Renderer>().enabled = false;
			currentWorker.GetComponentInChildren<Renderer>().enabled = true;
			currentHornet.GetComponentInChildren<Renderer>().enabled = false;
		}
		if(Input.GetKeyUp(KeyCode.Alpha3)){
			unitNum = 3;
			currentBumble.GetComponentInChildren<Renderer>().enabled = false;
			currentWorker.GetComponentInChildren<Renderer>().enabled = false;
			currentHornet.GetComponentInChildren<Renderer>().enabled = true;
		}
		}
	}

	void showUnit(int num){
		if(num == 1){
			currentBumble.GetComponentInChildren<Renderer>().enabled = true;
			currentWorker.GetComponentInChildren<Renderer>().enabled = false;
			currentHornet.GetComponentInChildren<Renderer>().enabled = false;
		}
		if(num == 2){
			currentBumble.GetComponentInChildren<Renderer>().enabled = false;
			currentWorker.GetComponentInChildren<Renderer>().enabled = true;
			currentHornet.GetComponentInChildren<Renderer>().enabled = false;
		}
		if(num == 3){
			currentBumble.GetComponentInChildren<Renderer>().enabled = false;
			currentWorker.GetComponentInChildren<Renderer>().enabled = false;
			currentHornet.GetComponentInChildren<Renderer>().enabled = true;
		}
		if(num == 0){
			currentBumble.GetComponentInChildren<Renderer>().enabled = false;
			currentWorker.GetComponentInChildren<Renderer>().enabled = false;
			currentHornet.GetComponentInChildren<Renderer>().enabled = false;
		}
	}
				
	void OnGUI(){
		if(unitsCreated < armySize){
		GUI.contentColor = Color.black;
		GUI.Label(new Rect(Screen.width/10, Screen.height-130, 200, 50), "Current Graph #"+(currentGraph+1));
		GUI.Label(new Rect(Screen.width/10-10, Screen.height-115, 300, 50), "Current Color Distribution #"+(currentColor+1));
		
		GUI.Label(new Rect(Screen.width/10, Screen.height-100, 200, 100), "Avg. Attack: "+averageSkill(Color.red));
		GUI.Label(new Rect(Screen.width/10+200, Screen.height-100, 200, 100), "Max Attack: "+maxSkill(Color.red));
		GUI.Label(new Rect(Screen.width/10, Screen.height-85, 200, 100), "Avg. Shield: "+averageSkill(Color.green));
		GUI.Label(new Rect(Screen.width/10+200, Screen.height-85, 200, 100), "Max Shield: "+maxSkill(Color.green));
		GUI.Label(new Rect(Screen.width/10, Screen.height-70, 200, 100), "Avg. Special: "+averageSkill(Color.yellow));
		GUI.Label(new Rect(Screen.width/10+200, Screen.height-70, 200, 100), "Max Special: "+maxSkill(Color.yellow));
		
		UnitLabels();

			GUI.skin = skin;
			if (GUI.Button (new Rect (Screen.width/2-75,Screen.height/2-200,150,100), "Create Unit", GUI.skin.GetStyle("button"))) {
				int [] attack = findArray(Color.red);
				int [] shield = findArray(Color.green);
				int [] special = findArray(Color.yellow);
				manager.addUnit(unitNum, attack, shield, special);
				unitsCreated++;
				eraseGraph();
				removeCombination();
				if(unitsCreated == armySize){
					currentBumble.GetComponentInChildren<Renderer>().enabled = false;
					currentWorker.GetComponentInChildren<Renderer>().enabled = false;
					currentHornet.GetComponentInChildren<Renderer>().enabled = false;
				}
			}

			if (GUI.Button (new Rect (Screen.width/2-75,Screen.height/2-100,150,100), "Randomize", GUI.skin.GetStyle("button"))) {
				RandomizeRemainingUnits();
				currentBumble.GetComponentInChildren<Renderer>().enabled = false;
				currentWorker.GetComponentInChildren<Renderer>().enabled = false;
				currentHornet.GetComponentInChildren<Renderer>().enabled = false;
			}
		}else{
			GUI.skin = skin;
			if (GUI.Button (new Rect (Screen.width/2-75,Screen.height/2-200,150,100), "Confirm", GUI.skin.GetStyle("button"))) {
				manager.teamsBuilt++;
				Application.LoadLevel(0);
			}

		}
	}

	public IEnumerator AIcreateNewUnit(int unitNo){
		//Random selection
		if(personalityType == 0){
			Debug.Log("Type 0");
			showUnit(unitNo);
			int rand = Random.Range(1, 4);
			for(int j = 0; j < rand; j++){
				currentGraph = nextGraph[currentGraph];
				currentColor = prevColor[currentColor];
				drawNewGraph();
				yield return new WaitForSeconds(2f);
			}
			int [] attack = findArray(Color.red);
			int [] shield = findArray(Color.green);
			int [] special = findArray(Color.yellow);
			manager.addUnit(unitNo, attack, shield, special);
			unitsCreated++;
			eraseGraph();
			removeCombination();
		}
		//Max attack focus
		if(personalityType == 1){
			Debug.Log("Type 1");
			showUnit(unitNo);
			float bestAttack = 0;
			int bestGraph = 0;
			int bestColor = 0;
			for(int i = 0; i < numGraphs; i++){
				currentGraph = nextGraph[currentGraph];
				for(int j = 0; j < numColors; j++){
					currentColor = nextColor[currentColor];
					drawNewGraph();
					yield return new WaitForSeconds(.2f);
					float att = maxSkill(Color.red);
					if(att > bestAttack){
						bestAttack = att;
						bestGraph = currentGraph;
						bestColor = currentColor;
					}
				}
			}
			currentGraph = bestGraph;
			currentColor = bestColor;
			drawNewGraph();
			int [] attack = findArray(Color.red);
			int [] shield = findArray(Color.green);
			int [] special = findArray(Color.yellow);
			manager.addUnit(unitNo, attack, shield, special);
			unitsCreated++;
			removeCombination();
			eraseGraph();
		}
		showUnit(0);
		yield return new WaitForSeconds(2f);
	}

	void RandomizeRemainingUnits(){
		for(int i = unitsCreated; i < armySize; i++){
			int rand = Random.Range(1, 4);
			for(int j = 0; j < rand; j++){
				currentGraph = nextGraph[currentGraph];
				currentColor = prevColor[currentColor];
				unitNum = rand;
			}
			int [] attack = findArray(Color.red);
			int [] shield = findArray(Color.green);
			int [] special = findArray(Color.yellow);
			manager.addUnit(unitNum, attack, shield, special);
			unitsCreated++;
			removeCombination();
			eraseGraph();
		}
	}

	void UnitLabels(){
		if(currentBumble.GetComponentInChildren<Renderer>().enabled){
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-130, 200, 100), "Unit Type: Bumblebee");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-115, 200, 100), "Hit Points: 60");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-100, 200, 100), "Attack Range: 1");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-85, 200, 100), "Movement: 4");
		}
		if(currentWorker.GetComponentInChildren<Renderer>().enabled){
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-130, 200, 100), "Unit Type: Worker");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-115, 200, 100), "Hit Points: 40");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-100, 200, 100), "Attack Range: 3");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-85, 200, 100), "Movement: 5");
		}
		if(currentHornet.GetComponentInChildren<Renderer>().enabled){
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-130, 200, 100), "Unit Type: Hornet");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-115, 200, 100), "Hit Points: 20");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-100, 200, 100), "Attack Range: 6");
			GUI.Label(new Rect(Screen.width*7/10, Screen.height-85, 200, 100), "Movement: 8");
		}
	}
	
	void removeCombination(){
		numGraphs--;
		nextGraph[prevGraph[currentGraph]] = nextGraph[currentGraph];
		prevGraph[nextGraph[currentGraph]] = prevGraph[currentGraph];
		nextColor[prevColor[currentColor]] = nextColor[currentColor];
		prevColor[nextColor[currentColor]] = prevColor[currentColor];
		if(numGraphs > 0){
			currentGraph = nextGraph[currentGraph];
			currentColor = nextColor[currentColor];
			eraseGraph();
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
		int index = 0;
		for(int j = 0; j < numBars; j++){
			if(GetColorMix(j).Equals(skill)){
				ret[index] = values[currentGraph,j];
				index++;
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

	void eraseGraph(){
		for(int i = 0; i < numBars; i++){
			Transform bar = bars[currentGraph, i];
			bar.gameObject.renderer.enabled = false;
		}
	}
	/*erases current graph and draws a new one with either new histogram or new color distro (or both)*/	
	void drawNewGraph(){
		for(int i = 0; i < numBars; i++){
			Transform oldBar;
			if(currentGraph != 0){
				oldBar = bars[currentGraph-1, i];
			}else{
				oldBar = bars[numGraphs-1, i];	
			}
			oldBar.gameObject.renderer.enabled = false;
		}
		for(int j = 0; j < numBars; j++){
			Transform newBar = bars[currentGraph, j];
			newBar.gameObject.renderer.material.color = GetColorMix(j);
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
