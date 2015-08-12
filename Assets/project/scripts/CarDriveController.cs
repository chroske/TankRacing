using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CarDriveController : MonoBehaviour
{
	[SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
	[SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
	[SerializeField] private Vector3 m_CentreOfMassOffset;
	[SerializeField] private float m_MaximumSteerAngle;
	[Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
	[Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
	[SerializeField] private float m_FullTorqueOverAllWheels;
	[SerializeField] private float m_ReverseTorque;
	[SerializeField] private float m_MaxHandbrakeTorque;
	[SerializeField] private float m_Downforce = 100f;
	[SerializeField] private float m_Topspeed = 200;
	[SerializeField] private static int NoOfGears = 5;
	[SerializeField] private float m_RevRangeBoundary = 1f;
	[SerializeField] private float m_SlipLimit;
	[SerializeField] private float m_BrakeTorque;
	[SerializeField] private GameObject steerObj;
	[SerializeField] private GameObject body;
	[SerializeField] private GameObject cameraArm;
	[SerializeField] private GameObject gameManager;
	
	private Quaternion[] m_WheelMeshLocalRotations;
	private Vector3 m_Prevpos, m_Pos;
	private float m_SteerAngle;
	private int m_GearNum;
	private float m_GearFactor;
	private float m_OldRotation;
	private float m_CurrentTorque;
	private Rigidbody m_Rigidbody;
	private const float k_ReversingThreshold = 0.01f;
	private GameManager m_GameManager;
	
	public bool Skidding { get; private set; }
	public float BrakeInput { get; private set; }
	public float CurrentSteerAngle{ get { return m_SteerAngle; }}
	public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude*2.23693629f; }}
	public float MaxSpeed{get { return m_Topspeed; }}
	public float Revs { get; private set; }
	public float AccelInput { get; private set; }
		

	
	// Use this for initialization
	private void Start()
	{
		m_GameManager = gameManager.GetComponent<GameManager>();

		m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
		
		m_MaxHandbrakeTorque = float.MaxValue;
		
		m_Rigidbody = GetComponent<Rigidbody>();
		m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);
	}
	
	
	private void GearChanging()
	{
		float f = Mathf.Abs(CurrentSpeed/MaxSpeed);
		float upgearlimit = (1/(float) NoOfGears)*(m_GearNum + 1);
		float downgearlimit = (1/(float) NoOfGears)*m_GearNum;
		
		if (m_GearNum > 0 && f < downgearlimit)
		{
			m_GearNum--;
		}
		
		if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
		{
			m_GearNum++;
		}
	}
	
	
	// simple function to add a curved bias towards 1 for a value in the 0-1 range
	private static float CurveFactor(float factor)
	{
		return 1 - (1 - factor)*(1 - factor);
	}
	
	
	// unclamped version of Lerp, to allow value to exceed the from-to range
	private static float ULerp(float from, float to, float value)
	{
		return (1.0f - value)*from + value*to;
	}
	
	
	private void CalculateGearFactor()
	{
		float f = (1/(float) NoOfGears);
		// gear factor is a normalised representation of the current speed within the current gear's range of speeds.
		// We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
		var targetGearFactor = Mathf.InverseLerp(f*m_GearNum, f*(m_GearNum + 1), Mathf.Abs(CurrentSpeed/MaxSpeed));
		m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime*5f);
	}
	
	
	private void CalculateRevs()
	{
		// calculate engine revs (for display / sound)
		// (this is done in retrospect - revs are not used in force/power calculations)
		CalculateGearFactor();
		var gearNumFactor = m_GearNum/(float) NoOfGears;
		var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
		var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
		Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
	}
	
	
	public void Move(float steering, float accel, float footbrake, float handbrake)
	{
		//clamp input values
		steering = Mathf.Clamp(steering, -1, 1);
		AccelInput = accel = Mathf.Clamp(accel, 0, 1);
		BrakeInput = footbrake = -1*Mathf.Clamp(footbrake, -1, 0);
		handbrake = Mathf.Clamp(handbrake, 0, 1);
		
		SteerHelper();
		ApplyDrive(accel, footbrake);
		CapSpeed();
		
		//Set the handbrake.
		//Assuming that wheels 2 and 3 are the rear wheels.
		if (handbrake > 0f)
		{
			var hbTorque = handbrake*m_MaxHandbrakeTorque;
			m_WheelColliders[2].brakeTorque = hbTorque;
			m_WheelColliders[3].brakeTorque = hbTorque;
			
		}
		
		
		CalculateRevs();
		GearChanging();
		
		AddDownForce();
		CheckForWheelSpin();
		//TractionControl();
	}
	
	
	private void CapSpeed()
	{
		float speed = m_Rigidbody.velocity.magnitude;
		speed *= 2.23693629f;
		if (speed > m_Topspeed)
			m_Rigidbody.velocity = (m_Topspeed/2.23693629f) * m_Rigidbody.velocity.normalized;
//		switch (m_SpeedType)
//		{
//		case SpeedType.MPH:
//			
//			speed *= 2.23693629f;
//			if (speed > m_Topspeed)
//				m_Rigidbody.velocity = (m_Topspeed/2.23693629f) * m_Rigidbody.velocity.normalized;
//			break;
//			
//		case SpeedType.KPH:
//			speed *= 3.6f;
//			if (speed > m_Topspeed)
//				m_Rigidbody.velocity = (m_Topspeed/3.6f) * m_Rigidbody.velocity.normalized;
//			break;
//		}
	}
	
	
	private void ApplyDrive(float accel, float footbrake)
	{
		
		float thrustTorque;
		thrustTorque = accel * (m_CurrentTorque / 4f);
		for (int i = 0; i < 4; i++)
		{
			m_WheelColliders[i].motorTorque = thrustTorque;
		}
		
		for (int i = 0; i < 4; i++)
		{
			if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
			{
				m_WheelColliders[i].brakeTorque = m_BrakeTorque*footbrake;
			}
			else if (footbrake > 0)
			{
				m_WheelColliders[i].brakeTorque = 0f;
				m_WheelColliders[i].motorTorque = -m_ReverseTorque*footbrake;
			}
		}
	}
	
	
	private void SteerHelper()
	{
		for (int i = 0; i < 4; i++)
		{
			WheelHit wheelhit;
			m_WheelColliders[i].GetGroundHit(out wheelhit);
			if (wheelhit.normal == Vector3.zero)
				return; // wheels arent on the ground so dont realign the rigidbody velocity
		}
		
		// this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
		if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
		{
			var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
			Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
			m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
		}
		m_OldRotation = transform.eulerAngles.y;
	}
	
	
	// this is used to add more grip in relation to speed
	private void AddDownForce()
	{
		m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up*m_Downforce*m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
	}
	
	
	// checks if the wheels are spinning and is so does three things
	// 1) emits particles
	// 2) plays tiure skidding sounds
	// 3) leaves skidmarks on the ground
	// these effects are controlled through the WheelEffects class
	private void CheckForWheelSpin()
	{
		// loop through all wheels
		for (int i = 0; i < 4; i++)
		{
			WheelHit wheelHit;
			m_WheelColliders[i].GetGroundHit(out wheelHit);
		}
	}
	
	// crude traction control that reduces the power to wheel if the car is wheel spinning too much
	private void TractionControl()
	{
		WheelHit wheelHit;

		for (int i = 0; i < 4; i++)
		{
			m_WheelColliders[i].GetGroundHit(out wheelHit);
			
			AdjustTorque(wheelHit.forwardSlip);
		}
	}
	
	
	private void AdjustTorque(float forwardSlip)
	{
		if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
		{
			m_CurrentTorque -= 10 * m_TractionControl;
		}
		else
		{
			m_CurrentTorque += 10 * m_TractionControl;
			if (m_CurrentTorque > m_FullTorqueOverAllWheels)
			{
				m_CurrentTorque = m_FullTorqueOverAllWheels;
			}
		}
	}

	private void FixedUpdate()
	{
		if(m_GameManager.battleMode == "Car"){
			SwipeSteering();
		}
	}
	
	private void SwipeSteering(){
		//touch
		if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) || Input.GetMouseButton (0)) {
			DisplayTouchingEvent();
		// un touch
		} else if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || !Input.GetMouseButton (0)) {
			DisplayUnTouchingEvent();

			//reset steer angle
			ReturnDefaultSteerAngle();
		}
	}

	private void DisplayTouchingEvent(){
		float wheelSteerAngle = steerObj.GetComponent<SteerController>().steerAngle;
		float steering = Mathf.Clamp(wheelSteerAngle, -1, 1);
		float m_SteerAngle = steering*m_MaximumSteerAngle;

		for (int i = 0; i < 4; i++)
		{
			m_WheelColliders[i].brakeTorque = 0;
		}
		
		//drive
		Move(1, 1, 1, 0f);
		
		//steering
		if (wheelSteerAngle < 0) {
			transform.Rotate (new Vector3 (0, wheelSteerAngle * 0.5f, 0) * Time.deltaTime);
			
			m_WheelColliders[0].steerAngle = Mathf.Clamp(m_SteerAngle, wheelSteerAngle * 0.4f, 0);
			m_WheelColliders[1].steerAngle = Mathf.Clamp(m_SteerAngle, wheelSteerAngle * 0.4f, 0);
		} else if (wheelSteerAngle > 0) {
			transform.Rotate (new Vector3 (0, wheelSteerAngle * 0.5f, 0) * Time.deltaTime);
			
			m_WheelColliders[0].steerAngle = Mathf.Clamp(m_SteerAngle, 0, wheelSteerAngle * 0.4f);
			m_WheelColliders[1].steerAngle = Mathf.Clamp(m_SteerAngle, 0, wheelSteerAngle * 0.4f);
		}
		//ドリフト演出のためcameraを見た目のみ傾ける
		DriftRotateCamera(wheelSteerAngle);
	}

	private void DisplayUnTouchingEvent(){
		//brake
		for (int i = 0; i < 4; i++)
		{
			m_WheelColliders[i].brakeTorque = m_BrakeTorque;
		}
		
		ReturnDefaultCameraAngle();
	}

	private void DriftRotateCamera(float wheelSteerAngle){
		float angle = Mathf.LerpAngle (cameraArm.transform.localEulerAngles.y, -wheelSteerAngle*0.2f, Time.deltaTime*10);
		cameraArm.transform.localEulerAngles = new Vector3 (0, angle, 0);
	}


	private void ReturnDefaultCameraAngle(){
		//machin rotate reset
		if(cameraArm.transform.localEulerAngles.y != 0){
			float angle = Mathf.LerpAngle (cameraArm.transform.localEulerAngles.y, 0, Time.deltaTime*5);
			cameraArm.transform.localEulerAngles = new Vector3 (0, angle, 0);
		}
	}

	private void ReturnDefaultSteerAngle(){
		for (int i = 0; i < 2; i++) {
			float angle = Mathf.LerpAngle (m_WheelColliders[i].steerAngle, 0, Time.deltaTime*20);
			m_WheelColliders[i].steerAngle = angle;
		}

	}

}
