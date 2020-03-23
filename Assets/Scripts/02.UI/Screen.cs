using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen : MonoBehaviour
{
    Transform cameraTf;
    readonly int minX = -10;
    readonly int maxX = 10;
    readonly int minY = -6;
    readonly int maxY = 6;

    List<Animator> screens;

    [ContextMenu("Initialze")]
    
    private void Start()
    {
        if (screens != null)
            return;

        cameraTf = Camera.main.transform;
        GameObject parent = new GameObject("Screen Parent");
        parent.transform.SetParent(cameraTf);
        screens = new List<Animator>();

        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject newGo = ObjPoolManager.Instance.GetObject(ObjectPoolList.BlackScreen);
                newGo.transform.SetParent(parent.transform);
                newGo.transform.position = pos;
                screens.Add(newGo.GetComponent<Animator>());
            }
        }
    }
    
    public IEnumerator ChangeScreen(bool toBlack)
    {
        for (int i = 0; i < screens.Count; i++)
        {
            if (toBlack)
            {
                if (screens[i].gameObject.activeInHierarchy == false)
                {
                    screens[i].gameObject.SetActive(true);
                    screens[i].ResetTrigger("ToOrigin");
                }
            }
            else
            {
                if (screens[i].gameObject.activeInHierarchy)
                    screens[i].SetTrigger("ToOrigin");
            }
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }

}
