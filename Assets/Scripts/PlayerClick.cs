using UnityEngine;
using System.Collections;

public class PlayerClick : MonoBehaviour {

    Camera cam;
    public GameObject item;

	void Start () {
        cam = GetComponent<Camera> ();
        if (item == null) {
            Debug.LogError("item gameobject not assigned");
        }
	}


	void Update () {

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, cam.transform.forward, out hit, 10f);
        Vector3 normal = hit.normal;
        Vector3 point = hit.point;

        if (hitSomething) {
            if (Input.GetMouseButtonDown(0)) {
                BreakBlock(point + (-0.1f * normal));
            }
            if (Input.GetMouseButtonDown(1)) {
                PlaceBlock(point + (0.1f * normal));
            }

            // TODO: tool tips
        }
	}

    void PlaceBlock(Vector3 position) {
        Chunk chunk = Chunk.Find(position);

        if (chunk == null) return;

        int x = Mathf.FloorToInt(position.x - chunk.transform.position.x);
        int y = Mathf.FloorToInt(position.y - chunk.transform.position.y);
        int z = Mathf.FloorToInt(position.z - chunk.transform.position.z);

        chunk.SetId(x, y, z, 1);
    }

    void BreakBlock(Vector3 position) {
        Chunk chunk = Chunk.Find(position);

        if (chunk == null) return;

        int x = Mathf.FloorToInt(position.x - chunk.transform.position.x);
        int y = Mathf.FloorToInt(position.y - chunk.transform.position.y);
        int z = Mathf.FloorToInt(position.z - chunk.transform.position.z);

        chunk.SetId(x, y, z, 0);

        if (Item.Count < Item.MAX_COUNT) {
            Instantiate(item, position, Quaternion.identity);
        }
    }
}
