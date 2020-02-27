using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHolder : MonoBehaviour {
    AIBase walkAroundAI;
    AIBase AINow;

    private void Awake()
    {
        walkAroundAI = new WalkAroundAI();
        walkAroundAI.Awake();
        walkAroundAI.Initialize(gameObject);
        walkAroundAI.SetHoler(this);
    }

    private void Update()
    {
        walkAroundAI.Do(gameObject);
    }
}
