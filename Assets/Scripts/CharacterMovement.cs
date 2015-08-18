using UnityEngine;
using System.Collections;

namespace Minecraft {
    public class CharacterMovement : MonoBehaviour {

        public float speed = 10f;

        Vector3 direction;

        bool isFlying = true;

    	void Update () {

    	}

        public void Move(Vector3 direction, bool jump, bool crouch) {
            if (isFlying) {
                HandleFlyingMovement(direction, jump, crouch);
            }
        }

        public void HandleFlyingMovement(Vector3 direction, bool jump, bool crouch) {
            float magnitude = direction.magnitude;
            if (magnitude > 0.01f) {
                direction = direction / magnitude;

                transform.position += transform.rotation * direction * speed * Time.deltaTime;
            }

            if (jump) {
                transform.position += Vector3.up * speed * Time.deltaTime;
            } else if (crouch) {
                transform.position += Vector3.down * speed * Time.deltaTime;
            }
        }
    }
}