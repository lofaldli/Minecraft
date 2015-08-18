using UnityEngine;
using System.Collections;

namespace Minecraft {
    public class MouseLook : MonoBehaviour {

        float _sensitivityX = 15f;
        float _sensitivityY = 15f;

        float _minimumY = -90f;
        float _maximumY = 90f;

        float _rotationY = 0f;

        Camera _camera;

    	void Start () {
            if (GetComponent<Rigidbody>()) {
                GetComponent<Rigidbody>().freezeRotation = true;
            }
            _camera = GetComponentInChildren<Camera> ();
    	}

    	void Update () {
            transform.Rotate(0, Input.GetAxis("Mouse X") * _sensitivityX, 0);

            _rotationY += Input.GetAxis("Mouse Y") * _sensitivityY;
            _rotationY = Mathf.Clamp (_rotationY, _minimumY, _maximumY);
            _camera.transform.localEulerAngles = new Vector3(-_rotationY, _camera.transform.localEulerAngles.y, 0);
    	}
    }
}