using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {
    [SerializeField]
    int hp = 3;

    private void OnEnable()
    {
        hp = 3;
    }

    public void GetDamaged(int damage)
    {
        Debug.Log(gameObject.name + " get Damaged " + damage + " points");
        hp -= damage;
        GameObject effect = ObjPoolManager.Instance.GetObject(ObjectPoolList.BloodEffect);
        effect.transform.position = transform.position;
        effect.SetActive(true);
        if(hp < 1)
        {
            //죽음의 처리.
            Debug.Log("Die");
        }
    }
}
