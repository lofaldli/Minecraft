using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimplexNoise;

namespace Minecraft {
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk : MonoBehaviour {

    	public static int size = 16;
        public static int height = 64;

        static float frequency {
            get { return World.instance.frequency; }
        }
        static float amplitude {
            get { return World.instance.amplitude; }
        }
        static float baseHeight {
            get { return World.instance.baseHeight; }
        }

        public static List<Chunk> chunks = new List<Chunk> ();
        public static List<Chunk> unInitializedChunks = new List<Chunk> ();

        bool initialized = false;

        Mesh mesh;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        int[,,] map;
        int[,] heightMap;

        static Vector3 offset0, offset1, offset2;

        void Start() {
            chunks.Add(this);
            unInitializedChunks.Add(this);

            meshFilter = GetComponent<MeshFilter> ();
            meshCollider = GetComponent<MeshCollider> ();

            Random.seed = World.instance.seed;
            offset0 = new Vector3(Random.value * 100000f, 0, Random.value * 100000f);
            offset1 = new Vector3(Random.value * 100000f, 0, Random.value * 100000f);
            offset2 = new Vector3(Random.value * 100000f, 0, Random.value * 100000f);

            if (unInitializedChunks[0] == this) {
                Generate();
            }
    	}

        public void Generate() {
            unInitializedChunks.Remove(this);

            map = new int[size, height, size];
            heightMap = new int[size,size];

            GenerateTerrain();
            //AddTrees();

            initialized = true;
            StartCoroutine(BuildMesh());
        }


        public IEnumerator BuildMesh() {

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

            if (unInitializedChunks.Count > 0) {
                unInitializedChunks[0].Generate();
            }
        }

        void GenerateTerrain() {
            for (int x = 0; x < size; x++) {
                for (int z = 0; z < size; z++) {
                    heightMap[x,z] = CalculateHeightMapValue(new Vector3(x,0f,z) + transform.position);
                    for (int y = 0; y < height; y++) {

                        int id = CalculateId(new Vector3(x,y,z) + transform.position);
                        map[x,y,z] = id;
                    }
                }
            }
        }

        void AddTrees(){
            Tree.AddToArray(map, new Vector3(8,heightMap[8,8]+1,8));
        }

        private static int CalculateHeightMapValue(Vector3 position) {
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

            return Mathf.FloorToInt(noise + baseHeight);
        }

        private static int CalculateId(Vector3 position) {

            if (position.y < 0 || position.y > height - 1) {
                return 0;
            }

            float heightMapValue = CalculateHeightMapValue(position);

            if (position.y < 5) {
                return 5; // bedrock
            }

            if (position.y == heightMapValue) {
                return 1; // grass
            } else if (position.y < heightMapValue && position.y > heightMapValue - 5) {
                return 2; // dirt
            } else if (position.y > heightMapValue) {
                return 0; // air
            } else {
                return 3; // stone
            }
        }

        public int GetLocalId(int x, int y, int z) {

            if (y < 0 || y > height - 1) {
                return 0;
            }

            int id = 0;
            if (x < 0 || x > size - 1 || z < 0 || z > size - 1) {
                id = GetWorldId(new Vector3(x, y, z) + transform.position);
                return id;
            }

            if (initialized) {
                id = map[x,y,z];
            } else {
                id = CalculateId(new Vector3(x, y, z) + transform.position);
            }
            return id;
        }

        public static int GetWorldId(Vector3 position) {
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

                id = chunk.GetLocalId(x, y, z);
            } else {
                id = CalculateId(position);
            }
            return id;
        }

        public void SetLocalId(int x, int y, int z, int id) {
            if (y < 0 || y > height - 1 || x < 0 || x > size - 1 || z < 0 || z > size - 1) return;

            if (initialized) map[x,y,z] = id;

            StartCoroutine(BuildMesh());

            Chunk chunk = null;
            if (x == 0) {
                chunk = Find(new Vector3(x-1, y, z) + transform.position);
            } else if (x == size - 1) {
                chunk = Find(new Vector3(x+1, y, z) + transform.position);
            }
            if (chunk != null && chunk.initialized) StartCoroutine(chunk.BuildMesh());

            if (z == 0) {
                chunk = Find(new Vector3(x, y, z-1) + transform.position);
            } else if (z == size - 1) {
                chunk = Find(new Vector3(x, y, z+1) + transform.position);
            }
            if (chunk != null && chunk.initialized) StartCoroutine(chunk.BuildMesh());
        }

        public static void SetWorldId(Vector3 position, int id) {
            if (position.y < 0f || position.y > height - 1) {
                return;
            }

            Chunk chunk = Find(position);
            if (chunk != null && chunk.initialized) {
                Vector3 chunkPosition = chunk.transform.position;

                int x = Mathf.FloorToInt(position.x - chunkPosition.x);
                int y = Mathf.FloorToInt(position.y - chunkPosition.y);
                int z = Mathf.FloorToInt(position.z - chunkPosition.z);

                chunk.SetLocalId(x, y, z, id);
            } else {
                Debug.LogWarning("This chunk has not been initialized yet!");
            }
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
            int id = GetLocalId(x,y,z);
            return id == 0 || id == 7;
        }
    }

    public class Block {

        public static void AddToMesh(Chunk chunk, int x, int y, int z, int id, List<Vector3> vertices, List<int> triangles, List<Vector2> uv) {
            // top
            if (chunk == null || chunk.IsTransparent(x, y + 1, z))
                AddFace(new Vector3(x, y + 1, z), id, Vector3.up, vertices, triangles, uv);
            // bottom
            if (chunk == null || chunk.IsTransparent(x,y - 1,z))
                AddFace(new Vector3(x, y, z), id, Vector3.down, vertices, triangles, uv);
            // left
            if (chunk == null || chunk.IsTransparent(x - 1, y, z))
                AddFace(new Vector3(x, y, z + 1), id, Vector3.left, vertices, triangles, uv);
            // right
            if (chunk == null || chunk.IsTransparent(x + 1, y, z))
                AddFace(new Vector3(x + 1, y, z + 1), id, Vector3.right, vertices, triangles, uv);
            // front
            if (chunk == null || chunk.IsTransparent(x, y, z + 1))
                AddFace(new Vector3(x, y, z + 1), id, Vector3.forward, vertices, triangles, uv);
            // back
            if (chunk == null || chunk.IsTransparent(x, y, z - 1))
                AddFace(new Vector3(x, y, z), id, Vector3.back, vertices, triangles, uv);
        }

        public static void AddFace(Vector3 corner, int id, Vector3 normal, List<Vector3> vertices, List<int> triangles, List<Vector2> uv) {
            Vector3 right = Vector3.zero;
            Vector3 up = Vector3.zero;
            bool reversed = false;

            if (normal == Vector3.down || normal == Vector3.right || normal == Vector3.forward) {
                reversed = true;
            }

            int index = vertices.Count;

            if (normal == Vector3.up || normal == Vector3.down) {
                up = Vector3.forward;
                right = Vector3.right;
            } else if (normal == Vector3.left || normal == Vector3.right) {
                up = Vector3.up;
                right = Vector3.back;
            } else if (normal == Vector3.forward || normal == Vector3.back) {
                up = Vector3.up;
                right = Vector3.right;
            }

            vertices.Add(corner);
            vertices.Add(corner + up);
            vertices.Add(corner + up + right);
            vertices.Add(corner + right);

            if (reversed) {
                triangles.Add(index + 0);
                triangles.Add(index + 2);
                triangles.Add(index + 1);

                triangles.Add(index + 0);
                triangles.Add(index + 3);
                triangles.Add(index + 2);
            } else {
                triangles.Add(index + 0);
                triangles.Add(index + 1);
                triangles.Add(index + 2);

                triangles.Add(index + 0);
                triangles.Add(index + 2);
                triangles.Add(index + 3);
            }

            Vector2 offset = Vector3.zero;
            float resolution = 0.0625f;

            Vector2 _00 = new Vector2(0,0) * resolution;
            Vector2 _01 = new Vector2(0,1) * resolution;
            Vector2 _11 = new Vector2(1,1) * resolution;
            Vector2 _10 = new Vector2(1,0) * resolution;

            ItemTexture texture = ItemDatabase.itemDatabase.GetItemById(id).texture;
            if (normal == Vector3.up) {
                offset = texture.top;
            } else if (normal == Vector3.down) {
                offset = texture.bottom;
            } else if (normal == Vector3.left) {
                offset = texture.left;
            } else if (normal == Vector3.right) {
                offset = texture.right;
            } else if (normal == Vector3.forward) {
                offset = texture.front;
            } else if (normal == Vector3.back) {
                offset = texture.back;
            }

            uv.Add(_00 + offset);
            uv.Add(_01 + offset);
            uv.Add(_11 + offset);
            uv.Add(_10 + offset);
        }
    }

    public class Tree : MonoBehaviour {

        static int minHeight = 5;
        static int maxHeight = 10;

        static int trunkId = 6;
        static int leafId = 7;

        public static void AddToArray(int[,,] map, Vector3 position) {
            int x0 = Mathf.FloorToInt(position.x);
            int y0 = Mathf.FloorToInt(position.y);
            int z0 = Mathf.FloorToInt(position.z);

            int height = Random.Range(minHeight, Mathf.Min(maxHeight, Chunk.height - y0));

            // trunk
            for (int y = 0; y < height - 1; y++) {
                    map[x0, y + y0, z0] = trunkId;
            }

            // top leaves
            map[x0, y0 + height - 1, z0] = leafId;
            map[x0 + 1, y0 + height - 1, z0] = leafId;
            map[x0 - 1, y0 + height - 1, z0] = leafId;
            map[x0, y0 + height - 1, z0 + 1] = leafId;
            map[x0, y0 + height - 1, z0 - 1] = leafId;

            // top - 1 leaves
            for (int x = -1; x <= 1; x++) {
                for (int z = -1; z <= 1; z++) {
                    if (!(x == 0 && z == 0)) {
                        map[x0 + x, y0 + height - 2, z0 + z] = leafId;
                    }
                }
            }
            for (int x = -2; x <= 2; x++) {
                for (int z = -2; z <= 2; z++) {
                    if (!(x == 0 && z == 0)) {
                        map[x0 + x, y0 + height - 3, z0 + z] = leafId;
                        map[x0 + x, y0 + height - 4, z0 + z] = leafId;
                    }
                }
            }
        }
    }
}