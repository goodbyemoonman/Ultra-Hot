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
    public GameObject gameStartButton;
    public GameObject clickToStartText;
    public GameObject toNextStageButton;
    public TextMeshProUGUI NotificationText;
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
        GameStateTeller += GameStateObserver;
    }

    void Start()
    {
        GameStateTeller(GameStateList.GameTitle);
    }

    void AtGameTitle()
    {
        clickToStartText.SetActive(true);
        gameStartButton.SetActive(true);
        openingCeremony = OpeningCeremony(2);
        StartCoroutine(openingCeremony);
    }

    void GameStateObserver(GameStateList state)
    {
        switch (state)
        {
            case GameStateList.GameTitle:
                AtGameTitle();
                break;
            case GameStateList.StageReady:
                AtStageReady();
                break;
            case GameStateList.StageStart:
                StopAllCoroutines();
                StartCoroutine(EnemyRespawn(numberOfEnemy));
                break;
            case GameStateList.Defeat:
                StopAllCoroutines();
                NotifyMsg("DEFEAT");
                StartCoroutine(AfterDefeat());
                break;
            case GameStateList.Win:
                StopAllCoroutines();
                NotifyMsg("STAGE CLEAR");
                StartCoroutine(AfterWin());
                break;
        }
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
    //GameStartButton GameObject 에서 이벤트 트리거로 클릭시 발동.
    public void GameStartButton()
    {
        StopAllCoroutines();
        gameTitleText.SetActive(false);
        gameStartButton.SetActive(false);
        clickToStartText.SetActive(false);

        StartCoroutine(AfterWin());
    }

    //ToNextStageButton GameObject 에서 이벤트 트리거로 클릭시 발동.
    public void ToNextStageButton()
    {
        StopAllCoroutines();
        toNextStageButton.SetActive(false);
        StartCoroutine(ToNextStage());
    }
    
    #region Using by WorldMaker Script
    public void AddEnemySpawner(GameObject spawner)
    {
        enemySpawners.Add(spawner);
    }

    public void SetNumberOfEnemy(int num)
    {
        numberOfEnemy = num;
    }
    #endregion

    void BeforeStageReady()
    {
        enemySpawners.Clear();
        deadEnemyCount = 0;
    }

    public void DefeatStage()
    {
        GameStateTeller(GameStateList.Defeat);
    }

    void EnemyDeadObserver()
    {
        deadEnemyCount++;
        if (deadEnemyCount == numberOfEnemy)
        {
            if (GameStateTeller != null)
                GameStateTeller(GameStateList.Win);
        }
    }

    void AtStageReady()
    {
        for(int i = 0; i < ActiveOnStageReady.Count; i++)
        {
            ActiveOnStageReady[i].SetActive(true);
        }
        clickToStartText.SetActive(true);
        toNextStageButton.SetActive(true);
    }

    IEnumerator AfterDefeat()
    {
        openingCeremony = OpeningCeremony(8);
        StartCoroutine(openingCeremony);

        BeforeStageReady();

        yield return StartCoroutine(screen.ChangeScreen(true));
        clickToStartText.SetActive(true);

        if (GameStateTeller != null)
            GameStateTeller(GameStateList.StageReady);

    }

    IEnumerator AfterWin()
    {
        BeforeStageReady();

        yield return StartCoroutine(screen.ChangeScreen(true));
        clickToStartText.SetActive(true);

        if (GameStateTeller != null)
            GameStateTeller(GameStateList.StageReady);
    }

    IEnumerator ToNextStage()
    {
        NotifyMsg("");
        clickToStartText.SetActive(false);
        sightManager.RefreshSight();
        yield return StartCoroutine(screen.ChangeScreen(false));
        if (GameStateTeller != null)
            GameStateTeller(GameStateList.StageStart);
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
    
    void NotifyMsg(string msg)
    {
        if (msg == "")
            NotificationText.gameObject.SetActive(false);
        else
        {
            NotificationText.text = msg;
            NotificationText.gameObject.SetActive(true);
        }
    }
}
