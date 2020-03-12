using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EKeyMove : MonoBehaviour {
    Vector3 newPos;
    private void OnEnable()
    {
        newPos = transform.localPosition;
        StartCoroutine(Move());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Move()
    {
        float adder = 0.01f;
        while (true)
        {
            if (transform.localPosition.y > 0.65f)
                adder = -0.01f;

            if (transform.localPosition.y < 0.5f)
                adder = 0.01f;
            
            newPos.y += adder;

            transform.localPosition = newPos;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
