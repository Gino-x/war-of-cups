using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [Header("Round Settings")]
    [SerializeField] private int baseEnemiesPerRound = 6;
    [SerializeField] private int enemiesIncreasePerRound = 3;
    [SerializeField] private float timeBetweenSpawns = 1.5f;
    [SerializeField] private int maxAliveAtOnce = 10;
    [SerializeField] private float delayBeforeFirstRound = 3f;
    [SerializeField] private float delayBetweenRounds = 5f;
    [Header("Difficulty Scaling")]
    [SerializeField] private float speedIncreasePerRound = 0.1f;
    [SerializeField] private float maxSpeedMultiplier = 3f;
    [SerializeField] private int baseDamage = 2;
    [SerializeField] private int damageIncreasePerRound = 5;
    [Header("UI")]
    [SerializeField] private RoundUIController roundUI;

    private int currentRound = 0;
    private int enemiesToSpawnThisRound;
    private int enemiesSpawnedThisRound;
    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(RoundLoop());
    }

    private IEnumerator RoundLoop()
    {
        yield return new WaitForSeconds(delayBeforeFirstRound);
        while (true)
        {
            currentRound++;
            enemiesToSpawnThisRound = baseEnemiesPerRound + (currentRound - 1) * enemiesIncreasePerRound;
            enemiesSpawnedThisRound = 0;
            activeEnemies.Clear();

            if (roundUI != null)
            {
                roundUI.UpdateRoundText(currentRound);
                roundUI.ShowRoundAnnouncement(currentRound);
            }

            yield return StartCoroutine(SpawnRound());

            while (enemiesSpawnedThisRound < enemiesToSpawnThisRound || CountAlive() > 0)
                yield return null;

            // Round won: bring back any player who died this round
            if (LoadCharacter.Instance != null)
                LoadCharacter.Instance.RespawnMarkedPlayers();

            yield return new WaitForSeconds(delayBetweenRounds);
        }
    }

    private IEnumerator SpawnRound()
    {
        while (enemiesSpawnedThisRound < enemiesToSpawnThisRound)
        {
            if (CountAlive() < maxAliveAtOnce)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
            else yield return null;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
            return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);
        activeEnemies.Add(enemy);
        enemiesSpawnedThisRound++;

        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            float multiplier = Mathf.Min(1f + (currentRound - 1) * speedIncreasePerRound, maxSpeedMultiplier);
            ai.SetSpeedMultiplier(multiplier);
        }

        EnemyDamage dmg = enemy.GetComponent<EnemyDamage>();
        if (dmg != null)
        {
            int newDamage = baseDamage + (currentRound - 1) * damageIncreasePerRound;
            dmg.SetDamage(newDamage);
        }
    }

    private int CountAlive()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count;
    }
}