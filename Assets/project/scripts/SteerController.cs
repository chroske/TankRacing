using UnityEngine;
using System.Collections;

public class SteerController : MonoBehaviour {
	[SerializeField] private GameObject pullBar;

	public float steerAngle { get; private set; }
	public float nevaConDistance { get; private set; }

	private float dx;
	private float dy;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) || Input.GetMouseButton (0)) {
			dx = Input.mousePosition.x - transform.position.x;
			dy = Input.mousePosition.y - transform.position.y;
			 
			float rad = Mathf.Atan2 (dx, -dy);
			steerAngle = rad * Mathf.Rad2Deg;

			nevaConDistance = Vector2.Distance (new Vector2 (dx, dy), new Vector2 (0, 0));
		} else if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {
			steerAngle = 0;
			nevaConDistance = 0;
		}

		//neva con
		pullBar.transform.localScale = new Vector3(1, nevaConDistance*4, 1);
		pullBar.transform.localRotation = Quaternion.Euler(0, 0, steerAngle+180);

	}


}
