using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float speed = 10f;

	void Update () {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));


        if (Input.GetKey(KeyCode.Space)) {
            direction.y = 1f;
        } else if (Input.GetKey(KeyCode.LeftShift)) {
            direction.y = -1f;
        }

        float movementLength = direction.magnitude;
        if (movementLength != 0) {
            direction /= movementLength;

            transform.position += transform.rotation * direction * speed * Time.deltaTime;
        }
	}


}
