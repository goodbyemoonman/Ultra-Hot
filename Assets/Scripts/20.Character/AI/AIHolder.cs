using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHolder : MonoBehaviour {
    AIBase seek;

    private void Awake()
    {
        seek = gameObject.AddComponent<SeekAI>();
        seek.SetHoler(this);
    }

    private void OnEnable()
    {
        seek.Do(gameObject);
    }

    IEnumerator Dumb()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            yield return StartCoroutine(
                RotateHelper(gameObject, 90));
        }
    }
    public IEnumerator RotateHelper(GameObject who, float angle)
    {
        float originAngle = who.transform.eulerAngles.z;
        for (float t = 0; t < 0.4f; t += Time.deltaTime)
        {
            who.transform.eulerAngles =
                new Vector3(0, 0,
                Mathf.Lerp(originAngle, originAngle + angle, t * 2.5f));
            SendMessage("MoveToSelfDir", Vector2.right);

            yield return null;
        }
        who.transform.eulerAngles = new Vector3(0, 0, originAngle + angle);
    }
}
