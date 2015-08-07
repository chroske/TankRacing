using UnityEngine;
using System.Collections;

public class SteerController : MonoBehaviour {

	public float steerAngle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) || Input.GetMouseButton (0)) {
			float dx = Input.mousePosition.x - transform.position.x;
			float dy = Input.mousePosition.y - transform.position.y;
			 
			float rad = Mathf.Atan2 (dx, -dy);
			steerAngle = rad * Mathf.Rad2Deg;
		} else if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {
			steerAngle = 0;
		}
	}
}
