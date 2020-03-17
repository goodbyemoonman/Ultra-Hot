using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningEffectSound : MonoBehaviour {
    private void OnEnable()
    {
        SoundManager.Instance.PlaySE(gameObject, 0.2f, AudioClipList.Shoot01, AudioClipList.Shoot02, AudioClipList.break1, AudioClipList.BodyImpact3);
    }
}
