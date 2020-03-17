using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour {
    public enum CharacterState { Idle, Sturn }
    CharacterState state;
    public CharacterState State
    {
        get
        {
            return state;
        }
        private set {
            if (state == value)
                return;
            else
            {
                CharaStateTeller(value);
                state = value;
            }
        }
    }
    public delegate void DeadDelegate();
    public static event DeadDelegate DeadTellerStatic;
    public event DeadDelegate DeadTeller;
    public delegate void CharaStateDelegate(CharacterState state);
    public event CharaStateDelegate CharaStateTeller;
    SpriteRenderer sr;

    [SerializeField]
    int hp = 3;

    protected void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        sr.color = Color.white;
        State = CharacterState.Idle;
        hp = 3;
    }

    virtual public void GetDamaged(int damage)
    {
        State = CharacterState.Sturn;
        hp -= damage;
        StopAllCoroutines();
        StartCoroutine(Timer(1f));
        GameObject effect = ObjPoolManager.Instance.GetObject(ObjectPoolList.BloodEffect);
        effect.transform.position = transform.position;
        effect.SetActive(true);
    }


    IEnumerator Timer(float time)
    {
        if (hp < 1)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            GameObject effect = ObjPoolManager.Instance.GetObject(ObjectPoolList.BloodEffect);
            effect.transform.position = transform.position;
            effect.SetActive(true);
            if(DeadTellerStatic != null)
                DeadTellerStatic();
            if (DeadTeller != null)
                DeadTeller();
            ObjPoolManager.Instance.ReturnObject(gameObject);
        }
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            sr.color = Color.Lerp(Color.red, Color.white, t / time);
            yield return null;
        }
        sr.color = Color.white;
        State = CharacterState.Idle;
    }
}
