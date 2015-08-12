using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	[SerializeField] private GameObject FpsCamera;
	[SerializeField] private GameObject CarCamera;
	[SerializeField] private GameObject gameManager;

	private GameManager m_GameManager;
	private Camera m_FpsCamera;
	private Camera m_CarCamera;

	// Use this for initialization
	void Start () {
		m_GameManager = gameManager.GetComponent<GameManager>();
		m_FpsCamera = FpsCamera.GetComponent<Camera>();
		m_CarCamera = CarCamera.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if(m_FpsCamera.enabled && m_GameManager.battleMode == "Car"){
			m_FpsCamera.enabled = false;
			m_CarCamera.enabled = true;
		} else if(!m_FpsCamera.enabled && m_GameManager.battleMode == "Canon") {
			m_FpsCamera.enabled = true;
			m_CarCamera.enabled = false;
		}

//		if(m_CarCamera.enabled && m_GameManager.battleMode == "Canon"){
//			m_CarCamera.enabled = false;
//		} else {
//			m_CarCamera.enabled = true;
//		}
	}
}
