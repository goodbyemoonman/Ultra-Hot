using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipHolder : MonoBehaviour {
    float boundary;
    BoundaryCheckAlgorithm bca;
    AttackBase punch;
    FixedJoint2D fxjt2d;
    MoveHandler mh;
    [SerializeField]
    GameObject EquipObj;
    AttackBase atkNow;
    Equipment equipNow;
    public Transform equipPos;
    public BulletReminder br;
    bool canAct;

    private void Awake()
    {
        mh = GetComponent<MoveHandler>();
        punch = GetComponent<PunchAttack>();
        bca = new BoundaryCheckAlgorithm();
        fxjt2d = GetComponent<FixedJoint2D>();
        boundary = 1.5f;
        atkNow = punch;
        canAct = true;
    }

    public void StateObserver(HealthManager.CharacterState state)
    {
        if (state == HealthManager.CharacterState.Sturn)
            canAct = false;
        else
            canAct = true;

    }

    public bool IsEquipSomethig()
    {
        if (EquipObj == null)
            return false;
        else
            return true;
    }
    
    public void EKeyDown()
    {
        if (IsEquipSomethig())
            return;
        List<GameObject> candidates = bca.GetObjListInSight(gameObject, boundary, Utility.EquipmentLayer);
        if (candidates.Count == 0)
            return;

        Equip(candidates[0]);
    }
    
    public void TryAttack()
    {
        atkNow.TryExecute();
    }

    void Equip(GameObject equipObj)
    {
        if (!canAct)
            return;

        this.EquipObj = equipObj;
        fxjt2d.enabled = true;
        fxjt2d.connectedBody = equipObj.GetComponent<Rigidbody2D>();
        equipNow = equipObj.GetComponent<Equipment>();
        equipNow.EquipTo(equipPos);
        atkNow = equipObj.GetComponent<AttackBase>();
        if(br != null)
        {
            equipNow.BulletReminder += br.Remind;
        }
    }

    void UnEquip(string msg)
    {
        fxjt2d.connectedBody = null;
        fxjt2d.enabled = false;
        if (br != null)
            equipNow.BulletReminder -= br.Remind;
        EquipObj.SendMessage(msg);
        atkNow = punch;
        equipNow = null;
        EquipObj = null;
    }

    public void Throw()
    {
        if (EquipObj == null)
            return;
        UnEquip("ThrowThisObj");
    }

    public void Drop()
    {
        if (EquipObj == null)
            return;
        UnEquip("Drop");
    }
    
    public float GetAtkRange()
    {
        return atkNow.AtkRange;
    }

    public void EnemyAtk()
    {
        if (atkNow.EnoughBullet())
        {
            TryAttack();
        }
        else
            Drop();
    }

    public Vector2 GetShootPoint()
    {
        if (equipNow != null)
        {
            return EquipObj.GetComponent<PistolAttack>().firePos.position;
        }
        else
            return transform.position;
    }
    
}
