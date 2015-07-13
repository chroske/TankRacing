using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float timer = 3;
	public GameObject bulletHole;


	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		Destroy(gameObject, timer);
	}

	void OnTriggerEnter (Collider col) {
		if(col.gameObject.tag == "Enemy"){
			col.gameObject.SendMessage("Damage");
		}

		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Enemy") {
			collision.gameObject.SendMessage("Damage");
		} else {
			Debug.Log(collision.contacts[0].point);

			GameObject obj = Instantiate(
				bulletHole,
				collision.contacts[0].point + collision.contacts[0].normal*0.01f,
				Quaternion.LookRotation(collision.contacts[0].normal)
				//Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal)
				)as GameObject;
			obj.transform.Rotate(180,0,/*Random.Range(0,360)*/0);
		}
		Destroy(gameObject);
	}
}
