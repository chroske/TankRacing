using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Characters.FirstPerson
{
	public class CanonRotate : MonoBehaviour {

		[SerializeField] private MouseLook m_MouseLook;

		private Camera m_Camera;

		// Use this for initialization
		void Start () {
			m_Camera = GameObject.FindWithTag("FPSCamera").GetComponent<Camera>();
			m_MouseLook.Init(transform , m_Camera.transform);
		}
		
		// Update is called once per frame
		void Update () {
			RotateView();
		}

		private void RotateView()
		{
			m_MouseLook.LookRotation (transform, m_Camera.transform);
		}
	}
}
