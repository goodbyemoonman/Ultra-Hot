using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectSound : MonoBehaviour {
    private void OnEnable()
    {
        SoundManager.Instance.PlaySE(gameObject, AudioClipList.BodyImpact1, AudioClipList.BodyImpact2, AudioClipList.BodyImpact3);
    }
}
