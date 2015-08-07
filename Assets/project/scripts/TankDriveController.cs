using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class TankDriveController : MonoBehaviour {

	public WheelCollider wheelRightFront;
	public WheelCollider wheelRightBack;
	public WheelCollider wheelLeftFront;
	public WheelCollider wheelLeftBack;

	public GameObject steerObj;


	float speed = 100.0f;
	float brake = 60.0f;
	float Turning = 20.0f;

	public float maxMotorTorque;
	public float maxBrakeTorque;

	private float motor = 0;

	void Update () {
		if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButton (0)) {
			motor = speed * maxMotorTorque;
			motor = 1000;

			wheelRightFront.brakeTorque = 0;
			wheelRightBack.brakeTorque = 0;
			wheelLeftFront.brakeTorque = 0;
			wheelLeftBack.brakeTorque = 0;

			//wheelRightFront.motorTorque = motor;
			wheelRightBack.motorTorque = motor;
			//wheelLeftFront.motorTorque = motor;
			wheelLeftBack.motorTorque = motor;



		} else if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {
			wheelRightFront.brakeTorque = brake * maxBrakeTorque;
			wheelRightBack.brakeTorque = brake * maxBrakeTorque;
			wheelLeftFront.brakeTorque = brake * maxBrakeTorque;
			wheelLeftBack.brakeTorque = brake * maxBrakeTorque;
		}

		SwipeSteering ();
	}

	private void SwipeSteering(){
		float wheelSteerAngle = steerObj.GetComponent<SteerController>().steerAngle;
		float steering = Mathf.Clamp(wheelSteerAngle, -1, 1);
		float m_MaximumSteerAngle = 45f;

		float m_SteerAngle = steering*m_MaximumSteerAngle;

		if(wheelSteerAngle < 0){
			wheelRightFront.steerAngle = Mathf.Clamp(m_SteerAngle, wheelSteerAngle, 0);
			wheelLeftFront.steerAngle = Mathf.Clamp(m_SteerAngle, wheelSteerAngle, 0);
		} else if(wheelSteerAngle > 0) {
			wheelRightFront.steerAngle = Mathf.Clamp(m_SteerAngle, 0, wheelSteerAngle);
			wheelLeftFront.steerAngle = Mathf.Clamp(m_SteerAngle, 0, wheelSteerAngle);
		}


		Debug.Log("kakudo=" + wheelSteerAngle);
	}


	private float m_FullTorqueOverAllWheels = 2500f;
	private float m_TractionControl = 0;
	private float m_CurrentTorque;


	void Start () {
		//m_CurrentTorque = m_FullTorqueOverAllWheels;
		m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);
	}

	private void FixedUpdate()
	{
		// pass the input to the car!
		float steering = Input.GetAxis("Horizontal");
		float accel = Input.GetAxis("Vertical");


		accel = Mathf.Clamp(accel, 0, 1);

		float thrustTorque = accel * (m_CurrentTorque / 2f);
		wheelRightBack.motorTorque = thrustTorque;
		wheelLeftBack.motorTorque = thrustTorque;

		steering = Mathf.Clamp(steering, -1, 1);
		float m_MaximumSteerAngle = 25f;
		float m_SteerAngle = steering*m_MaximumSteerAngle;

		m_SteerAngle = steering*m_MaximumSteerAngle;
		wheelRightFront.steerAngle = m_SteerAngle;
		wheelLeftFront.steerAngle = m_SteerAngle;
	}
}
