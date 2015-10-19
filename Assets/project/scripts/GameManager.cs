using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public string battleMode;
	public int playerId = 1;

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

		if(playerId == 1){
			playerId = 2;
		} else if(playerId == 2){
			playerId = 1;
		}
	}
}
