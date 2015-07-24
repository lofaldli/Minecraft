using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour {

    public static int MAX_COUNT = 100;
    public static int Count = 0;

	public int id = 1;
    public float size = 0.25f;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    PlayerInfo[] players;

    void Start () {
        Count++;

        meshFilter = GetComponent<MeshFilter> ();
        meshCollider = GetComponent<MeshCollider> ();
        transform.localScale = Vector3.one * size;
        GenerateMesh();
    }

    void OnDestroy() {
        Count--;
    }

    void Update() {

        if (transform.position.y < 0) {
            Destroy(gameObject);
        }

        players = FindObjectsOfType<PlayerInfo> ();

        if (players.Length > 0) {
            Vector3 position = transform.position;
            float pickupDistance = 0.5f;
            float attractDistance = 4f;
            foreach (PlayerInfo player in players) {
                float distance = Vector3.Distance(position, player.transform.position);
                if (distance < pickupDistance) {
                    if (player.GetComponent<Inventory>().PickUp(id, 1)) {
                        Destroy(gameObject);
                        break;
                    }
                } else if (distance < attractDistance) {
                    transform.position = Vector3.Lerp(position, player.transform.position, Time.deltaTime * 2f / distance);
                }
            }


        }
    }

    void GenerateMesh () {
        List<Vector3> vertices = new List<Vector3> ();
        List<int> triangles = new List<int> ();
        List<Vector2> uv = new List<Vector2> ();

        Block.AddToMesh(null, 0, 0, 0, id, vertices, triangles, uv);

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
