using UnityEngine;
using System.Collections;

public class TankDriveController : MonoBehaviour {

	public WheelCollider wheelRight1;
	public WheelCollider wheelRight2;
	public WheelCollider wheelLeft1;
	public WheelCollider wheelLeft2;


	float speed = 10.0f;
	public float maxMotorTorque; 

	void Start () {
		
	}

	void Update () {

		float motor = maxMotorTorque * Input.GetAxis("Vertical") * speed;

		wheelRight1.motorTorque = motor;
		wheelRight2.motorTorque = motor;

		wheelLeft1.motorTorque = motor;
		wheelLeft2.motorTorque = motor;
	}
}
