using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public Transform target;
    public Text text;
    Rigidbody2D rgbd;
    Vector3 originPos;
    float t = 0;

    private void OnEnable()
    {
        rgbd = GetComponent<Rigidbody2D>();
        originPos = transform.position;
        t = 0;
        StopAllCoroutines();
        StartCoroutine(move());
    }

    IEnumerator move()
    {
        while(t <= 1)
        {
            Vector3 newPos = Vector3.Lerp(originPos, target.position, t += 0.01f * Time.timeScale);
            rgbd.MovePosition(newPos);
            text.text = t.ToString();
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}

