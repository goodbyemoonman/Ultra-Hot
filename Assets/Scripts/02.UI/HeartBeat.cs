using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeat : MonoBehaviour {
    Color origin;
    SpriteRenderer sr;
    int target;
    float alpha;
    int start;
    float t;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        origin = sr.color;
        target = 0;
        alpha = 1;
        start = 1;
        t = 0;
    }
    
    void Update () {
        alpha = Mathf.Lerp(start, target, t);
        Color newC = origin;
        newC.a = alpha;
        if(t > 1)
        {
            if (target == 0)
            {
                start = 0;
                target = 1;
            }
            else
            {
                start = 1;
                target = 0;
            }
            t -= 1;
        }
        sr.color = newC;
        t += (Time.deltaTime * 3);
	}
}
