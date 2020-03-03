using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulletReminder : MonoBehaviour {
    TextMeshProUGUI tmp;
    public float t;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.color = Color.clear;
    }

    public void Remind(int number)
    {
        tmp.text = number.ToString();
        StopAllCoroutines();
        StartCoroutine(FadeOut(t));
    }

    IEnumerator FadeOut(float t)
    {
        float time = 0;
        while(time < t)
        {
            Color newC = Color.Lerp(Color.black, Color.clear, time / t);
            tmp.color = newC;
            time += 0.05f;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        tmp.color = Color.clear;
    }
}
