using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectCallback : MonoBehaviour {
    public void OnParticleSystemStopped()
    {
        ObjPoolManager.Instance.ReturnObject(gameObject);
    }
}
