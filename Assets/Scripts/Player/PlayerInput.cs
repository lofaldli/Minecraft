using UnityEngine;
using System.Collections;

namespace Minecraft {
    [RequireComponent(typeof(CharacterMovement))]
    public class PlayerInput : MonoBehaviour {
        CharacterMovement _characterMovement;
        Camera _camera;

        void Start () {
            _characterMovement = GetComponent<CharacterMovement> ();
            _camera = GetComponentInChildren<Camera> ();
        }

        void Update () {

            Vector3 movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            bool jump = Input.GetButton("Jump");
            bool crouch = Input.GetButton("Crouch");

            _characterMovement.Move(movementDirection, jump, crouch);

            RaycastHit hit;
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, 10f)) {
                if (Input.GetButtonDown("Fire1")) {
                    BreakBlock(hit.point + (-0.1f * hit.normal));
                }
                if (Input.GetButtonDown("Fire2")) {
                    PlaceBlock(hit.point + (0.1f * hit.normal));
                }

                TooltipUI.SetActive(true);
                int id = Chunk.GetWorldId(hit.point + (-0.1f * hit.normal));
                Item item = ItemDatabase.GetItemById(id);
                if (item != null) {
                    TooltipUI.text.text = "" + item.name;
                }
            } else {
                TooltipUI.SetActive(false);
            }
        }

        void PlaceBlock(Vector3 position) {
            Chunk.SetWorldId(position, 1);
        }

        void BreakBlock(Vector3 position) {
            Chunk.SetWorldId(position, 0);
        }
    }
}
