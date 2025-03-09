using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("ENEMY CONFIGURATION")]
    [SerializeField] protected EnemyContainer[] enemyContainers;

    [System.Serializable]
    protected struct EnemyContainer
    {
        public Character_Enemy enemyPrefab;

        [HideInInspector]
        public int poolIndex;
    }

    private int currentEnemiesAlive = 0;

    [Header("ENEMY SPAWN CONFIGURATION")]
    [SerializeField] private int maxEnemyInScene = 10;
    [SerializeField] private bool canSpawnEnemies = false;
    [SerializeField] private bool isSpawnRateConstant = true;
    [SerializeField] private float maxSpawnRate = 10f;
    [SerializeField] private float minSpawnRate = 5f;
    [SerializeField] private float spawnRateDecreaseRate = 1f;
    [SerializeField] private Transform spawnPointParent;

    private Transform[] spawnPoints;
    
    private float currentSpawnRate;

    private float timer = 0;

    private void Awake()
    {
        EnemyEvent.OnEnemyDie += UpdateEnemyCount;

        BattleEvent.OnStartBattle += DisableEnemySpawner;
        BattleEvent.OnFinishBattle += EnableEnemySpawner;
    }

    private void OnDestroy()
    {
        EnemyEvent.OnEnemyDie -= UpdateEnemyCount;

        BattleEvent.OnStartBattle -= DisableEnemySpawner;
        BattleEvent.OnFinishBattle -= EnableEnemySpawner;
    }

    private void Start()
    {
        currentSpawnRate = maxSpawnRate;
        spawnPoints = spawnPointParent.GetComponentsInChildren<Transform>();

        for (int i = 0; i < enemyContainers.Length; i++)
        {
            enemyContainers[i].poolIndex = ObjectPooler.instance.AddObject(enemyContainers[i].enemyPrefab.gameObject, 1);
        }
    }

    private void Update()
    {

        if (!canSpawnEnemies) return;

        timer += Time.deltaTime;
        if (timer >= maxSpawnRate)
        {
            timer = 0;
            SpawnEnemy();

            if(!isSpawnRateConstant)
            {
                AdjustSpawnRate();
            }

        }
        
    }

    private void UpdateEnemyCount()
    {
        currentEnemiesAlive--;

        if (currentEnemiesAlive < maxEnemyInScene)
        {
            canSpawnEnemies = true;
        }
    }

    private void SpawnEnemy()
    {
        int randomEnemyIndex = UnityEngine.Random.Range(0, enemyContainers.Length);

        Character_Enemy obj = ObjectPooler.instance.GetPooledObject(enemyContainers[randomEnemyIndex].poolIndex).GetComponent<Character_Enemy>();
        int randomPosIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        obj.SetPosition(spawnPoints[randomPosIndex].position);
        obj.ReviveCharacter();

        obj.gameObject.SetActive(true);

        currentEnemiesAlive++;

        if (currentEnemiesAlive >= maxEnemyInScene)
        {
            canSpawnEnemies = false;
        }

    }

    private void AdjustSpawnRate()
    {
        currentSpawnRate -= spawnRateDecreaseRate;

        currentSpawnRate = Mathf.Max(minSpawnRate, currentSpawnRate);

    }

    private void EnableEnemySpawner()
    {
        canSpawnEnemies = true;
    }

    private void DisableEnemySpawner()
    {
        canSpawnEnemies = false;
    }

}


public static class EnemyEvent
{
    public static Action OnEnemyDie;
    public static Action OnAllEnemyDied;
}