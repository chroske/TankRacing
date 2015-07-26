using UnityEngine;
using System.Collections;

public class TankController : MonoBehaviour {

	public float  sokudo ; //前進速度
	public float kaitenSokudo;//回転速度

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//前進
		float zenshin = Input.GetAxis("Vertical") * sokudo;
		transform.position += transform.forward * zenshin * Time.deltaTime;
		//回転
		float kaiten = Input.GetAxis("Horizontal") * kaitenSokudo;
		transform.Rotate(0, kaiten, 0);
	}
}
