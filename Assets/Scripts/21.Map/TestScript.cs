using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    public float scaler = 0.015f;
    public float timescale = 0;
    public float target = 1f;

    private void OnEnable()
    {
        StartCoroutine(scaleManage());
    }

    IEnumerator scaleManage()
    {
        while (true) {
            timescale = Mathf.Lerp(timescale, target, scaler);
            yield return new WaitForSeconds(0.01f);
        }
    }


    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 50), timescale.ToString());
    }
}
