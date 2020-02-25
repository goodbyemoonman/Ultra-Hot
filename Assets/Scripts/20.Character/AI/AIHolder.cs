using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHolder : MonoBehaviour {
    AIBase seek;

    private void Awake()
    {
        seek = gameObject.AddComponent<SeekAI>();
    }

    private void OnEnable()
    {
        StartCoroutine(Dumb());
    }

    IEnumerator Dumb()
    {
        while (true)
        {
            seek.Do(gameObject);

            yield return new WaitForSeconds(1);
        }
    }
}
