using UnityEngine;
using System.Collections;

public class gyroCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Quaternion gyro = Input.gyro.attitude;
		
		//float pitch = Mathf.Asin (2 * gyro.w * gyro.y - 2 * gyro.x * gyro.z);
		//float roll = Mathf.Atan2 (2 * gyro.y * gyro.z + 2 * gyro.w * gyro.x, -gyro.w*gyro.w + gyro.x*gyro.x + gyro.y*gyro.y - gyro.z*gyro.z);
		
		//Vector3 angle = new Vector3(roll*Mathf.Rad2Deg,0,0);
		
		//transform.localRotation = Quaternion.Euler(angle) * Quaternion.Euler(270f, 0, 0);

		Quaternion q = Input.gyro.attitude;
		Quaternion qq = Quaternion.AngleAxis(-90.0f, Vector3.left);
		q.x *= -1.0f;
		q.y *= -1.0f;
		transform.localRotation = qq * q;
	}
}
