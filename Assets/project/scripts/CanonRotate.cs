using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CanonRotate : MonoBehaviour {
	
	[SerializeField] private GameObject gameManager;
	
	public float XSensitivity = 2f;
	public float YSensitivity = 2f;
	public bool clampVerticalRotation = true;
	public float MinimumX = -90F;
	public float MaximumX = 90F;
	public bool smooth;
	public float smoothTime = 5f;
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	
	private Camera m_Camera;
	private GameManager m_GameManager;
	private float rotationX;
	private float rotationY;
	
	// Use this for initialization
	void Start () {
		m_GameManager = gameManager.GetComponent<GameManager>();
		m_Camera = GameObject.FindWithTag("CanonCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		RotateView();
	}
	
	private void RotateView()
	{
		if(m_GameManager.battleMode == "Canon"){
			#if UNITY_EDITOR
			LookRotation (transform, m_Camera.transform);
			#elif UNITY_IPHONE || UNITY_ANDROID
			LookRotationOnSmartPhone (transform, m_Camera.transform);
			#endif
		}
	}
	
	
	private void LookRotationOnSmartPhone(Transform character, Transform camera)
	{
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved){
			if (Input.GetButton ("Fire1")) {
				if (axes == RotationAxes.MouseXAndY) {
					float rotationX = character.transform.localEulerAngles.y + CrossPlatformInputManager.GetAxis ("Mouse X") * XSensitivity;
					
					rotationY += CrossPlatformInputManager.GetAxis ("Mouse Y") * YSensitivity;
					rotationY = Mathf.Clamp (rotationY, MinimumX, MaximumX);
					
					camera.transform.localEulerAngles = new Vector3 (-rotationY, 0, 0);
					character.transform.localEulerAngles = new Vector3 (0, rotationX, 0);
				} else if (axes == RotationAxes.MouseX) {
					camera.transform.Rotate (0, Input.GetAxis ("Mouse X") * XSensitivity, 0);
				} else {
					rotationY += Input.GetAxis ("Mouse Y") * YSensitivity;
					rotationY = Mathf.Clamp (rotationY, MinimumX, MaximumX);
					
					camera.transform.localEulerAngles = new Vector3 (-rotationY, camera.transform.localEulerAngles.y, 0);
				}
			}
		}
		
	}
	
	private void LookRotation(Transform character, Transform camera)
	{
		if (Input.GetButton ("Fire1")) {
			if (axes == RotationAxes.MouseXAndY) {
				float rotationX = character.transform.localEulerAngles.y + Input.GetAxis ("Mouse X") * XSensitivity;
				
				rotationY += Input.GetAxis ("Mouse Y") * YSensitivity;
				rotationY = Mathf.Clamp (rotationY, MinimumX, MaximumX);
				
				camera.transform.localEulerAngles = new Vector3 (-rotationY, 0, 0);
				character.transform.localEulerAngles = new Vector3 (0, rotationX, 0);
			} else if (axes == RotationAxes.MouseX) {
				camera.transform.Rotate (0, Input.GetAxis ("Mouse X") * XSensitivity, 0);
			} else {
				rotationY += Input.GetAxis ("Mouse Y") * YSensitivity;
				rotationY = Mathf.Clamp (rotationY, MinimumX, MaximumX);
				
				camera.transform.localEulerAngles = new Vector3 (-rotationY, camera.transform.localEulerAngles.y, 0);
			}
		}
	}
}