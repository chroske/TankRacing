using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;


        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

        public void LookRotation(Transform character, Transform camera)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

            if(clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

            if(smooth)
            {
                character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }


		enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
		RotationAxes axes = RotationAxes.MouseXAndY;
		float rotationX;
		float rotationY;
		public void LookRotationOnSmartPhone(Transform character, Transform camera)
		{
			if (Input.GetButton ("Fire1")) {
				if (axes == RotationAxes.MouseXAndY) {
					float rotationX = camera.transform.localEulerAngles.y + Input.GetAxis ("Mouse X") * XSensitivity;
					
					rotationY += Input.GetAxis ("Mouse Y") * YSensitivity;
					rotationY = Mathf.Clamp (rotationY, MinimumX, MaximumX);
					
					camera.transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
				} else if (axes == RotationAxes.MouseX) {
					camera.transform.Rotate (0, Input.GetAxis ("Mouse X") * XSensitivity, 0);
				} else {
					rotationY += Input.GetAxis ("Mouse Y") * YSensitivity;
					rotationY = Mathf.Clamp (rotationY, MinimumX, MaximumX);
					
					camera.transform.localEulerAngles = new Vector3 (-rotationY, camera.transform.localEulerAngles.y, 0);
				}
		



				
//			if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) || Input.GetMouseButton (0)) {
//				float yRot = /*character.transform.localEulerAngles.y + */CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
//				float xRot = /*camera.transform.localEulerAngles.x + */CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
//				
//				m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
//				m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
//				
//				if (clampVerticalRotation)
//					m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
//				
//				if (smooth) {
//					character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
//					                                            smoothTime * Time.deltaTime);
//					camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
//					                                         smoothTime * Time.deltaTime);
//				} else {
//					character.localRotation = m_CharacterTargetRot;
//					camera.localRotation = m_CameraTargetRot;
//				}
//			}
			}
		}




        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
