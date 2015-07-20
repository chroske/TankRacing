using UnityEngine;
using System.Collections;

public class gyroCamera : MonoBehaviour {

	private Quaternion gyro;

	// Use this for initialization
	void Start () {
		if (SystemInfo.supportsGyroscope) {
			//gyro = Input.gyro;
			//gyro.enabled = true;
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
		//デバイスの傾きを取得     
		gyro = Input.gyro.attitude;
		
		//回転の向きの調整     
		gyro.x *= -1.0f;
		gyro.y *= -1.0f;

		Quaternion qq = Quaternion.AngleAxis(-90.0f, -90.0f,0f);
		
		//自分の傾きとして適用
		transform.localRotation = gyro * qq;



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
