using UnityEngine;
using System.Collections;

public class shot : MonoBehaviour {

	[SerializeField] private GameObject gameManager;
	
	private GameManager m_GameManager;

	// Use this for initialization
	void Start () {
		m_GameManager = gameManager.GetComponent<GameManager>();
	}

	public GameObject bullet;
	public Transform spawn;
	public float speed = 1000;

	public Transform rifle; //修正箇所
	private float time = 0f;    //経過時間
	public float interval = 0.3f;   //何秒おきに発砲するか


	// Update is called once per frame
	void Update () {
		if(m_GameManager.battleMode == "Canon"){
			time += Time.deltaTime; //経過時間を加算
			
			if(Input.GetButton("Fire1")){
				if(time >= interval){
					Shoot();    //発砲
					time = 0f;  //初期化
				}
			}
		}
	}
	
	void Shoot () {
		GameObject obj = GameObject.Instantiate(bullet)as GameObject;
		obj.transform.position = spawn.position;
		Vector3 force;
		force = this.gameObject.transform.forward * speed;
		obj.GetComponent<Rigidbody>().AddForce (force);
	}
}
