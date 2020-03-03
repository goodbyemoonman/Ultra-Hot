using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour {
    public int num;
    private void OnEnable()
    {
        SendMessage("Remind", num);
    }
}
