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
                StateTeller(value);
                state = value;
            }
        }
    }
    public delegate void StateDelegate(CharacterState state);
    public event StateDelegate StateTeller;
    SpriteRenderer sr;

    [SerializeField]
    int hp = 3;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        State = CharacterState.Idle;
        hp = 3;
    }

    virtual public void GetDamaged(int damage)
    {
        State = CharacterState.Sturn;
        Debug.Log(gameObject.name + " get Damaged " + damage + " points");
        hp -= damage;
        SendMessage("Drop");
        StartCoroutine(Timer(1f));
        GameObject effect = ObjPoolManager.Instance.GetObject(ObjectPoolList.BloodEffect);
        effect.transform.position = transform.position;
        effect.SetActive(true);
    }


    IEnumerator Timer(float time)
    {
        if (hp < 1)
        {
            GameObject effect = ObjPoolManager.Instance.GetObject(ObjectPoolList.BloodEffect);
            effect.transform.position = transform.position;
            effect.SetActive(true);
            ObjPoolManager.Instance.ReturnObject(gameObject);
        }
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            sr.color = Color.Lerp(Color.red, Color.white, t / time);
            yield return null;
        }

        State = CharacterState.Idle;
    }
}
