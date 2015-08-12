using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public string battleMode;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void changeBattleFlag(){
		if(battleMode == "Car"){
			battleMode = "Canon";
		} else if(battleMode == "Canon"){
			battleMode = "Car";
		}
	}
}
