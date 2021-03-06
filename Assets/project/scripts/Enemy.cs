﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int hitPoint;
	public GameObject DestroyEffect;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Damage (int damagePoint) {
		hitPoint -= damagePoint;
		if(hitPoint <= 0){
			//effect
			GameObject effect = Instantiate(DestroyEffect ,
			                                transform.position,
			                                Quaternion.identity
			                                ) as GameObject;		// エフェクト発生
			Destroy(effect , 0.2f);

			Destroy(gameObject);    //自身を消去
		}

	}
}
