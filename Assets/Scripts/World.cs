using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {

    public WorldSettings settings;
    public static World world;
    public GameObject chunkPrefab;
    public int size = 3;
	void Start () {
        if (chunkPrefab == null) {
            Debug.LogError("chunkPrefab not assigned");
            return;
        }
        world = this;

        if (settings.seed == 0) {
            settings.seed = Random.Range(-100000000, 100000000);
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

[System.Serializable]
public class WorldSettings {
    public int seed = 0;
    public GenerationSettings generation;
}

[System.Serializable]
public class GenerationSettings {
    public float frequency = 0.02f;
    public float amplitude = 16.0f;
    public float baseHeight = 16.0f;
}

