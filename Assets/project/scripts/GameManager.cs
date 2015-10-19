using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	[SerializeField] private GameObject TestSocket;
	public string battleMode;
	public int playerId = 1;

	private TestSocketIO m_TestSocket;

	// Use this for initialization
	void Start () {
		m_TestSocket = TestSocket.GetComponent<TestSocketIO>();
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

		m_TestSocket.SettingGameModeByParam ();
	}
}
