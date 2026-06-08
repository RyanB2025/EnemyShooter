using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines how many of a specific enemy type to spawn
[System.Serializable]
public class EnemySpawnConfig
{
    [Tooltip("The enemy prefab to spawn (e.g., your EnemyShooter prefab)")]
    public GameObject enemyPrefab;

    [Tooltip("How many of this specific enemy to spawn in this wave")]
    public int count;
}

// Defines a single wave containing multiple enemy configurations
[System.Serializable]
public class Wave
{
    public string waveName;

    [Tooltip("List of different enemies and amounts to spawn in this wave")]
    public List<EnemySpawnConfig> enemiesToSpawn;

    [Tooltip("Time to wait between spawning individual enemies")]
    public float timeBetweenSpawns = 1f;

    [Tooltip("Time to wait after this wave ends before the next wave begins")]
    public float timeBeforeNextWave = 5f;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Location")]
    [Tooltip("The transform where enemies will appear")]
    [SerializeField] private Transform spawnPoint;

    [Header("Wave Configuration")]
    [SerializeField] private List<Wave> waves;
    [SerializeField] private bool autoStartSpawning = true;

    private int currentWaveIndex = 0;

    private void Start()
    {
        if (autoStartSpawning)
        {
            StartCoroutine(SpawnWavesRoutine());
        }
    }

    private IEnumerator SpawnWavesRoutine()
    {
        // Loop through all the waves configured in the inspector
        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log($"Starting Wave: {currentWave.waveName}");

            // Loop through each enemy configuration within the current wave
            foreach (EnemySpawnConfig enemyConfig in currentWave.enemiesToSpawn)
            {
                // Spawn the specified number of this particular enemy
                for (int i = 0; i < enemyConfig.count; i++)
                {
                    SpawnEnemy(enemyConfig.enemyPrefab);
                    yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
                }
            }

            Debug.Log($"Wave {currentWave.waveName} completed!");
            currentWaveIndex++;

            // Wait before starting the next wave (if there are more waves left)
            if (currentWaveIndex < waves.Count)
            {
                yield return new WaitForSeconds(currentWave.timeBeforeNextWave);
            }
        }

        Debug.Log("All waves have been completed!");
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab != null && spawnPoint != null)
        {
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("EnemySpawner is missing an Enemy Prefab or Spawn Point reference!");
        }
    }

    // Public method in case you want to trigger the spawner from a UI button or GameManager later
    public void StartSpawning()
    {
        if (currentWaveIndex == 0) // Prevents restarting if already running
        {
            StartCoroutine(SpawnWavesRoutine());
        }
    }
}
