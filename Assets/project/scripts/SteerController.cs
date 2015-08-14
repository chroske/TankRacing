using UnityEngine;
using System.Collections;

public class SteerController : MonoBehaviour {
	[SerializeField] private GameObject pullBar;

	public float steerAngle { get; private set; }
	public float valConDistance { get; private set; }

	public float maxDistance;

	private float dx;
	private float dy;
	private float valConUiDistance;



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

			valConDistance = Vector2.Distance (new Vector2 (dx, dy), new Vector2 (0, 0));
		} else if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {
			steerAngle = 0;
			valConDistance = 0;
			valConUiDistance = 0;
		}

		//neva con
		if(valConDistance > 0){
			//add finger position
			valConUiDistance = valConDistance + 130;
		}

		pullBar.transform.localScale = new Vector3(1, Mathf.Clamp(valConUiDistance,0,maxDistance), 1);
		pullBar.transform.localRotation = Quaternion.Euler(0, 0, steerAngle+180);
		this.transform.localScale = new Vector3 (1 - (Mathf.Clamp (valConDistance, 0, maxDistance) / maxDistance), 1 - (Mathf.Clamp (valConDistance, 0, maxDistance) / maxDistance), 1);


	}


}
