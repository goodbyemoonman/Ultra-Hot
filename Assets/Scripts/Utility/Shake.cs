using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {
    Vector3 originPos;
    float t = 0;
    private void Awake()
    {
        originPos = transform.position;
    }

    // Update is called once per frame
    void Update () {
        if (t > 0.25f)
        {
            t -= 0.25f;
            transform.position = originPos + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        }

        t += Time.deltaTime;
	}
}
