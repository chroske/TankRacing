using UnityEngine;
using System.Collections;

public class RotateWheelObj : MonoBehaviour {

	public WheelCollider wheelCo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localEulerAngles = new Vector3(0, Mathf.Floor(wheelCo.steerAngle), 90);

		//transform.Rotate(new Vector3(wheelCo.steerAngle,0f,0f));
	}
}
