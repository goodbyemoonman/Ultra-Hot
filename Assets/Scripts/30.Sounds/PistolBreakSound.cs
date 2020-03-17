using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBreakSound : MonoBehaviour {
    private void OnEnable()
    {
        SoundManager.Instance.PlaySE(gameObject, AudioClipList.break1);
    }
}
