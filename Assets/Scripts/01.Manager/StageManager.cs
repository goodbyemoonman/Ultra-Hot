using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameStateList { GameTitle, StageReady, StageStart, Win, Defeat }

public class StageManager : Singleton<StageManager>
{
    public delegate void GameStateDelegate(GameStateList state);
    public event GameStateDelegate GameStateTeller;
    Screen screen;
    SightManager sightManager;

    public List<GameObject> ActiveOnStageReady;
    public GameObject gameTitleText;
    public GameObject clickButton;
    public GameObject clickText;
    public TextMeshProUGUI text;
    List<GameObject> enemySpawners;
    int deadEnemyCount;
    int numberOfEnemy;
    IEnumerator openingCeremony;

    private void Awake()
    {
        sightManager = GetComponent<SightManager>();
        screen = GetComponent<Screen>();
        enemySpawners = new List<GameObject>();
        HealthManager.DeadTellerStatic += EnemyDeadObserver;
    }

    void Start()
    {
        GameStateTeller(GameStateList.GameTitle);
        clickText.SetActive(true);
        clickButton.SetActive(true);
        openingCeremony = OpeningCeremony(2);
        StartCoroutine(openingCeremony);
    }

    IEnumerator OpeningCeremony(float t)
    {
        while (true)
        {
            GameObject effect = ObjPoolManager.Instance.GetObject(ObjectPoolList.OpeningEffect);
            effect.transform.SetParent(Camera.main.transform);
            effect.transform.localPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f));
            effect.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0, t));
        }
    }

    [ContextMenu("Stage Start")]
    //메인 윈도우에서 이벤트 트리거로 클릭시 발동.
    public void StartStage()
    {
        StopAllCoroutines();
        screen.Initialize();
        gameTitleText.SetActive(false);
        clickButton.SetActive(false);
        clickText.SetActive(false);
        StartCoroutine(AfterWin());
    }

    public void AddEnemySpawner(GameObject spawner)
    {
        enemySpawners.Add(spawner);
    }

    public void SetNumberOfEnemy(int num)
    {
        numberOfEnemy = num;
    }
    
    IEnumerator EnemyRespawn(int max)
    {
        yield return new WaitForSeconds(0.5f);
        float t = 0;
        for (int i = 0; i < enemySpawners.Count && i < max; i++)
        {
            GameObject newEnemy = ObjPoolManager.Instance.GetObject(ObjectPoolList.Enemy);
            newEnemy.transform.position = enemySpawners[i].transform.position;
            newEnemy.SetActive(true);
        }

        for (int i = enemySpawners.Count; i < max; i++)
        {
            t = Random.Range(0, 4) + Random.Range(0, 4);
            yield return new WaitForSeconds(t);
            GameObject newEnemy = ObjPoolManager.Instance.GetObject(ObjectPoolList.Enemy);
            newEnemy.transform.position = enemySpawners[Random.Range(0, enemySpawners.Count)].transform.position;
            newEnemy.SetActive(true);
        }
    }

    public void DefeatStage()
    {
        StopAllCoroutines();
        GameStateTeller(GameStateList.Defeat);
        StartCoroutine(AfterDefeat());
    }

    void EnemyDeadObserver()
    {
        deadEnemyCount++;
        if (deadEnemyCount == numberOfEnemy)
        {
            TextMsg("Stage Clear");
            if (GameStateTeller != null)
                GameStateTeller(GameStateList.Win);
            StartCoroutine(AfterWin());
        }
    }

    IEnumerator AfterDefeat()
    {
        openingCeremony = OpeningCeremony(8);
        StartCoroutine(openingCeremony);
        TextMsg("Stage Fail");
        yield return StartCoroutine(screen.ChangeScreen(true));
        enemySpawners.Clear();
        deadEnemyCount = 0;

        if (GameStateTeller != null)
            GameStateTeller(GameStateList.StageReady);
        clickText.SetActive(true);

        while (!Input.GetMouseButtonDown(0))
            yield return null;
        StopCoroutine(openingCeremony);
        StartCoroutine(StageBegin());
    }

    IEnumerator AfterWin()
    {
        yield return StartCoroutine(screen.ChangeScreen(true));
        enemySpawners.Clear();
        deadEnemyCount = 0;
        if (GameStateTeller != null)
            GameStateTeller(GameStateList.StageReady);
        for(int i = 0; i < ActiveOnStageReady.Count; i++)
        {
            ActiveOnStageReady[i].SetActive(true);
        }
        clickText.SetActive(true);
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        StartCoroutine(StageBegin());
    }

    IEnumerator StageBegin()
    {
        TextMsg("");
        clickText.SetActive(false);
        sightManager.RefreshSight();
        yield return StartCoroutine(screen.ChangeScreen(false));
        if (GameStateTeller != null)
            GameStateTeller(GameStateList.StageStart);

        StartCoroutine(EnemyRespawn(numberOfEnemy));
    }

    void TextMsg(string msg)
    {
         text.text = msg;
    }
}
