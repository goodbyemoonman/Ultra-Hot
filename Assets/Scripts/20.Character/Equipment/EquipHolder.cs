using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipHolder : MonoBehaviour {
    float boundary;
    BoundaryCheckAlgorithm bca;
    AttackBase punch;
    FixedJoint2D fxjt2d;
    [SerializeField]
    GameObject EquipObj;
    AttackBase atkNow;
    Equipment equipNow;
    public Transform equipPos;
    public BulletReminder br;
    bool canAct;

    private void Awake()
    {
        GetComponent<HealthManager>().CharaStateTeller += CharacterStateObserver;
        GetComponent<HealthManager>().DeadTeller += DeadObserver;
        punch = GetComponent<PunchAttack>();
        bca = new BoundaryCheckAlgorithm();
        fxjt2d = GetComponent<FixedJoint2D>();
        boundary = 1.5f;
        atkNow = punch;
        canAct = true;
    }

    private void OnEnable()
    {
        canAct = true;
        UnEquip();
        StageManager.Instance.GameStateTeller += GameStateObserver;
    }

    void CharacterStateObserver(HealthManager.CharacterState state)
    {
        if (state == HealthManager.CharacterState.Sturn)
        {   //맞으면 무기를 떨어트려
            Drop();
            canAct = false;
        }
        else
            canAct = true;

    }

    void DeadObserver()
    {
        StageManager.Instance.GameStateTeller -= GameStateObserver;
    }

    void GameStateObserver(GameStateList state)
    {
        switch (state)
        {
            case GameStateList.StageStart:
                canAct = true;
                break;
            case GameStateList.Win:
            case GameStateList.Defeat:
                UnEquip();
                canAct = false;
                break;
            default:
                canAct = false;
                break;
        }
    }

    public bool IsEquipSomethig()
    {
        if (EquipObj == null)
            return false;
        else
            return true;
    }
    
    public void TryEquip()
    {
        if (IsEquipSomethig())
            return;
        if(bca.CheckObjListInSight(gameObject, boundary, Utility.EquipmentLayer))
            Equip(bca.GetObjList()[0]);
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
        EquipObj.SendMessage(msg);
        UnEquip();
    }

    void UnEquip()
    {
        fxjt2d.connectedBody = null;
        fxjt2d.enabled = false;
        if (equipNow == null)
            return;
        if (br != null)
            equipNow.BulletReminder -= br.Remind;
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
    
    void Drop()
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
            TryAttack();
        else
        {
            if (EquipObj == null)
                return;
            UnEquip("DestroyThis");
        }
    }
}
