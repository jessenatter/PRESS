using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WaveManager : BaseClass
{
    public Vector2[] possibleSpawnPoints = {
        new Vector2(8, 0),
        new Vector2(-8, 0),
        new Vector2(5, 3),
        new Vector2(-5, 3),
        new Vector2(-5, -3),
        new Vector2(5, -3),
    };

    List<Vector2> unusedSpawnPoints = new List<Vector2>();
    Queue<Enemy> createdEnemies = new Queue<Enemy>();

    public int enemyCount;
    public int currentEnemyCount;
    public int wave;

    float spawnDelay = 120;
    float spawnDelay_t;
    float inBetweenWaveTime = 120;
    float inBetweenWaveTime_t;

    bool waveLoaded;
    bool startedWaveLoad;

    public TextMeshProUGUI waveUI, scoreUI;

    public override void Start(Manager _manager)
    {
        base.Start(_manager);
        waveUI = manager.waveUI;
        scoreUI = manager.scoreUI;

        //load saved wave and score
        wave = GameDataManager.savedWave;
        manager.score = GameDataManager.savedScore;
    }

    public override void Update()
    {
        base.Update();

        if (!waveLoaded)
        {
            SpawnWave();
        }

        EndWave();
    }

    void StartWave()
    {
        //activate UI 
        waveUI.enabled = true;
        wave += 1;
        waveUI.text = "WAVE: " + wave.ToString();
        GameDataManager.savedWave = wave;
        enemyCount = Mathf.RoundToInt(wave * Random.Range(1, 1.75f));
        currentEnemyCount = enemyCount;

        unusedSpawnPoints.Clear();
        foreach(Vector2 spawnPoint in possibleSpawnPoints)
        {
            unusedSpawnPoints.Add(spawnPoint);
        }

        CreateEnemyClasses();

        spawnDelay_t = spawnDelay;
        inBetweenWaveTime_t = inBetweenWaveTime;
    }

    void SpawnWave()
    {
        if (!startedWaveLoad)
        {
            StartWave();
            startedWaveLoad = true;
        }

        if (inBetweenWaveTime_t == 0)
        {
            if (spawnDelay_t > 0)
            {
                spawnDelay_t--;
            }
            else if (createdEnemies.Count > 0)
            {
                spawnDelay_t = spawnDelay;
                SpawnEnemy();
            }
            else
            {
                waveLoaded = true;
            }
        }

        if (inBetweenWaveTime_t > 0)
        {
            inBetweenWaveTime_t--;
        }
        else if (waveUI.enabled)
        {
            spawnDelay_t = spawnDelay;
            waveUI.enabled = false;
        }
    }

    void CreateEnemyClasses()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Enemy enemy = new Enemy();
            createdEnemies.Enqueue(enemy);
        }
    }

    void SpawnEnemy()
    {
        Enemy enemy = createdEnemies.Dequeue();

        if (unusedSpawnPoints.Count == 0)
        {
            foreach (Vector2 spawnPoint in possibleSpawnPoints)
            {
                unusedSpawnPoints.Add(spawnPoint);
            }
        }

        int randomSpawnPoint = Random.Range(0, unusedSpawnPoints.Count - 1);

        enemy.spawnPoint = unusedSpawnPoints[Random.Range(0, unusedSpawnPoints.Count - 1)];
        unusedSpawnPoints.RemoveAt(randomSpawnPoint);

        manager.Characters.Add(enemy);
        manager.storedVelocities = new Vector2[manager.rbs.Count];
        enemy.Start(manager);
    }

    void EndWave()
    {
        if (wave != 0)
        {
            if (currentEnemyCount == 0)
            {
                Upgrades.OfferNewUpgrade();

                if (Upgrades.upgradeSelected)
                {
                    waveLoaded = false;
                    startedWaveLoad = false;
                }
            }
            else
                Upgrades.upgradeOffered = false;
        }
        else
        {
            waveLoaded = false;
            startedWaveLoad = false;
        }
    }
}
