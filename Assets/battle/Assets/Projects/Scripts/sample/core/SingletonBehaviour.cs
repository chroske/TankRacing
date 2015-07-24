using UnityEngine;
using System.Collections;

public class SingletonBehaviour<Type> : MonoBehaviour where Type : MonoBehaviour {
	private static Type instance;
	
	public static Type Instance {
		get {
			if ( null == instance ){
				instance = (Type)FindObjectOfType( typeof( Type ) );
			}
			return instance;
		}
	}
}
