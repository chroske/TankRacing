using UnityEngine;
using System.Collections;

public class gyroCamera : MonoBehaviour {

	private Quaternion gyro;
	private Camera m_Camera;

	// Use this for initialization
	void Start () {
		if (SystemInfo.supportsGyroscope) {
			Input.gyro.enabled = true;
		}
	}

	void OnGUI()
	{
		GUILayout.Label ("Gyroscope attitude : " + Input.gyro.attitude);
		GUILayout.Label("Gyroscope gravity : " + Input.gyro.gravity);
		GUILayout.Label("Gyroscope rotationRate : " + Input.gyro.rotationRate);
		GUILayout.Label("Gyroscope rotationRateUnbiased : " + Input.gyro.rotationRateUnbiased);
		GUILayout.Label("Gyroscope updateInterval : " + Input.gyro.updateInterval);
		GUILayout.Label ("Gyroscope userAcceleration : " + Input.gyro.userAcceleration);
		GUILayout.Label ("gyro : " + gyro);
	}
	
	// Update is called once per frame
	void Update () {
		gyro = Input.gyro.attitude;
		gyro = Quaternion.Euler(90, 0, -90) * (new Quaternion(-gyro.x,-gyro.y, gyro.z, gyro.w));
		transform.localRotation = gyro;



		//Quaternion gyro = Input.gyro.attitude;
		
		//float pitch = Mathf.Asin (2 * gyro.w * gyro.y - 2 * gyro.x * gyro.z);
		//float roll = Mathf.Atan2 (2 * gyro.y * gyro.z + 2 * gyro.w * gyro.x, -gyro.w*gyro.w + gyro.x*gyro.x + gyro.y*gyro.y - gyro.z*gyro.z);
		
		//Vector3 angle = new Vector3(roll*Mathf.Rad2Deg,0,0);
		
		//transform.localRotation = Quaternion.Euler(angle) * Quaternion.Euler(270f, 0, 0);

		//Quaternion q = Input.gyro.attitude;
		//Quaternion qq = Quaternion.AngleAxis(-90.0f, Vector3.left);
		//q.x *= -1.0f;
		//q.y *= -1.0f;
		//transform.localRotation = qq * gyro;
	}
}
