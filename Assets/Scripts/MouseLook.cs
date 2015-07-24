using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseX;
    public float sensitivityX = 15f;
    public float sensitivityY = 15f;

    public float minimumY = -90f;
    public float maximumY = 90f;

    float rotationY = 0f;

    void Update ()
    {
        if (axes == RotationAxes.MouseX) {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        } else {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }

    void Start ()
    {
        if (GetComponent<Rigidbody>()) {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
}
