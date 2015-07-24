using UnityEngine;
using System.Collections;

public class Decay : MonoBehaviour {

    public float lifetime = 10f;
    float remaining;

    void Start () {
	    remaining = lifetime;
	}

	void Update () {
	    remaining -= Time.deltaTime;
        if (remaining < 0.0f) {
            Destroy(gameObject);
        }
	}
}
