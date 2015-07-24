using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimplexNoise;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour {

	public static int size = 16;
    public static int height = 64;

    static float frequency {
        get { return World.world.settings.generation.frequency; }
    }
    static float amplitude {
        get { return World.world.settings.generation.amplitude; }
    }
    static float baseHeight {
        get { return World.world.settings.generation.baseHeight; }
    }

    public static List<Chunk> chunks = new List<Chunk> ();
    public static List<Chunk> unInitializedChunks = new List<Chunk> ();

    bool initialized = false;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    int[,,] map;

    static Vector3 offset0, offset1, offset2;

    void Start() {
        chunks.Add(this);
        unInitializedChunks.Add(this);

        meshFilter = GetComponent<MeshFilter> ();
        meshCollider = GetComponent<MeshCollider> ();

        Random.seed = World.world.settings.seed;
        offset0 = new Vector3(Random.value * 100000f, 0, Random.value * 100000f);
        offset1 = new Vector3(Random.value * 100000f, 0, Random.value * 100000f);
        offset2 = new Vector3(Random.value * 100000f, 0, Random.value * 100000f);

        if (unInitializedChunks[0] == this) {
            Generate();
        }
	}

    public IEnumerator Regenerate() {
        //Debug.Log("Regenerating chunk " + transform.position);

        List<Vector3> vertices = new List<Vector3> ();
        List<int> triangles = new List<int> ();
        List<Vector2> uv = new List<Vector2> ();

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < size; x++) {
                for (int z = 0; z < size; z++) {
                    int id = map[x,y,z];
                    if (id != 0)
                        Block.AddToMesh(this, x, y, z, id, vertices, triangles, uv);
                }
            }
        }

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        yield return null;
    }


    public void Generate() {
        unInitializedChunks.Remove(this);

        map = new int[size, height, size];

        List<Vector3> vertices = new List<Vector3> ();
        List<int> triangles = new List<int> ();
        List<Vector2> uv = new List<Vector2> ();

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < size; x++) {
                for (int z = 0; z < size; z++) {

                    int id = CalculateId(new Vector3(x,y,z) + transform.position);
                    map[x,y,z] = id;

                    if (id != 0)
                        Block.AddToMesh(this, x, y, z, id, vertices, triangles, uv);

                }
            }
        }

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        initialized = true;
        if (unInitializedChunks.Count > 0) {
            unInitializedChunks[0].Generate();
        }
    }


    private static int CalculateId(Vector3 position) {

        if (position.y < 0 || position.y > height - 1) {
            return 0;
        }

        float x0 = (position.x + offset0.x) * frequency;
        float z0 = (position.z + offset0.z) * frequency;

        float x1 = (position.x + offset1.x) * frequency * 2f;
        float z1 = (position.z + offset1.z) * frequency * 2f;

        float x2 = (position.x + offset2.x) * frequency * 4f;
        float z2 = (position.z + offset2.z) * frequency * 4f;

        float noise0 = Noise.Generate(x0, z0) * amplitude;
        float noise1 = Noise.Generate(x1, z1) * amplitude / 4f;
        float noise2 = Noise.Generate(x2, z2) * amplitude / 8f;

        float noise = noise0 + noise1 + noise2;

        if (noise + baseHeight > position.y) {
            return 1;
        } else {
            return 0;
        }
    }

    public int GetId(int x, int y, int z) {

        if (y < 0 || y > height - 1) {
            return 0;
        }

        int id = 0;
        if (x < 0 || x > size - 1 || z < 0 || z > size - 1) {
            id = InspectId(new Vector3(x, y, z) + transform.position);
            return id;
        }

        if (initialized) {
            id = map[x,y,z];
        } else {
            id = CalculateId(new Vector3(x, y, z) + transform.position);
        }
        return id;
    }

    public static int InspectId(Vector3 position) {
        if (position.y < 0f || position.y > height - 1) {
            return 0;
        }
        int id = 0;
        Chunk chunk = Find(position);
        if (chunk != null && chunk.initialized) {
            Vector3 chunkPosition = chunk.transform.position;

            int x = Mathf.FloorToInt(position.x - chunkPosition.x);
            int y = Mathf.FloorToInt(position.y - chunkPosition.y);
            int z = Mathf.FloorToInt(position.z - chunkPosition.z);

            id = chunk.GetId(x, y, z);
        } else {
            id = CalculateId(position);
        }
        return id;
    }

    public void SetId(int x, int y, int z, int id) {
        if (y < 0 || y > height - 1 || x < 0 || x > size - 1 || z < 0 || z > size - 1) return;

        if (initialized) map[x,y,z] = id;

        StartCoroutine(Regenerate());

        Chunk chunk = null;
        if (x == 0) {
            chunk = Find(new Vector3(x-1, y, z) + transform.position);
        } else if (x == size - 1) {
            chunk = Find(new Vector3(x+1, y, z) + transform.position);
        }
        if (chunk != null && chunk.initialized) StartCoroutine(chunk.Regenerate());

        if (z == 0) {
            chunk = Find(new Vector3(x, y, z-1) + transform.position);
        } else if (z == size - 1) {
            chunk = Find(new Vector3(x, y, z+1) + transform.position);
        }
        if (chunk != null && chunk.initialized) StartCoroutine(chunk.Regenerate());
    }

    public static Chunk Find(Vector3 position) {
        foreach (Chunk chunk in chunks) {
            Vector3 chunkPosition = chunk.transform.position;
            if (position.x >= chunkPosition.x && position.x < chunkPosition.x + size &&
                position.y >= chunkPosition.y && position.y < chunkPosition.y + height &&
                position.z >= chunkPosition.z && position.z < chunkPosition.z + size) {
                return chunk;
            }
        }
        return null;
    }

    public bool IsTransparent(int x, int y, int z) {
        if (y < 0) {
            return false;
        }
        int id = GetId(x,y,z);
        return id == 0;
    }
}

