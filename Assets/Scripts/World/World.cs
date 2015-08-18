using UnityEngine;
using System.Collections;

namespace Minecraft {
    public class World : MonoBehaviour {

        public static World instance;

        public GameObject chunkPrefab;

        public int size = 0;
        public int seed = 0;
        [Range(0.01f,0.1f)] public float frequency = 0.02f;
        [Range(0f,32f)] public float amplitude = 16.0f;
        [Range(0f,64f)] public float baseHeight = 32.0f;

    	void Start () {
            if (chunkPrefab == null) {
                Debug.LogError("chunkPrefab not assigned");
                return;
            }
            instance = this;

            if (seed == 0) {
                seed = Random.Range(-100000000, 100000000);
            }

            StartCoroutine(Generate());
    	}

        IEnumerator Generate () {
            GameObject chunkGO;
            for (int x = -size; x < size; x++) {
                for (int z = -size; z < size; z++) {
                    chunkGO = (GameObject)Instantiate(chunkPrefab, new Vector3(x,0,z) * Chunk.size, Quaternion.identity);
                    chunkGO.transform.SetParent(transform);
                    yield return null;
                }
            }
        }
    }
}