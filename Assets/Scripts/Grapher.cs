using UnityEngine;
using System.Collections;

public class Grapher : MonoBehaviour {
	public Transform bar;
	private int[] values;
	private Transform[] bars;
	public int[] colorIndex;
	public Material mat;
	public Color[] colorMix;
	
	private int numColors = 5;
	private float prev = 0;
	public int numBars = 100;
	// Use this for initialization
	void Start () {
		colorMix = new Color[numColors];
		resetColorMix();
		bars = new Transform[numBars];
		values = new int[numBars];
		Vector3 position = Vector3.up;
		SetColors();
		for(int i = 0; i<numBars; i++){
			float randomNumber = Random.Range(max(prev-10, 0f), min(prev+20, 50f));
			float multiplier = Random.Range(0.1f, 1.0f);
			values[i] = Mathf.CeilToInt(randomNumber*multiplier);
			Transform t = (Transform)Instantiate(bar, position, Quaternion.identity);
			t.transform.localScale = new Vector3(.5f, randomNumber*multiplier+1, .5f);
			t.transform.Translate(0, randomNumber*multiplier/2f, 0);
			t.gameObject.renderer.material.color = GetColorMix(i);
			bars[i] = t;
			position = position + Vector3.right*.55f;
		}
		
	}
	void resetColorMix(){
		for(int i = 0; i < numColors; i++){
			colorMix[i] = Color.clear;
		}
	}
	
	void SetColors(){
		colorIndex = new int[numColors];
		int prevIndex = -1;
		for(int i = 0; i < numColors; i++){
			int index = 0;
			int slice = (numBars/numColors)+2;
			for(int j = 0; j < 2; j++){
				index = (int)Mathf.Floor(max(index, Random.Range(prevIndex+1, slice*(i+1))));
			}
			colorIndex[i] = ((i == numColors-1)) ? numBars : index;
			prevIndex = index;
		}
		ScrambleColors();
	}
	
	void ScrambleColors(){
		for(int i = 0; i < numColors; i++){
			int count = 0;
			while(count < 200){
				int index = Random.Range(0, numColors);
				if(colorMix[index] == Color.clear){
					colorMix[index] = GetColor(i);
					Debug.Log(colorMix[index]);
					break;
				}
				count++;
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
		if(i<= colorIndex[0]) return colorMix[0];
		if(i<= colorIndex[1]) return colorMix[1];
		if(i<= colorIndex[2]) return colorMix[2];
		if(i<= colorIndex[3]) return colorMix[3];
		if(i<= colorIndex[4]) return colorMix[4];
		return Color.black;
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	float max(float a, float b){
		 return a > b ? a : b;
	}
	float min(float a, float b){
		return a < b ? a : b;
	}
}
