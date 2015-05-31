using System.Collections;
using System.Linq;
using UnityEngine;

public class SpawnerAI : MonoBehaviour
{
    [Range(1, 500)]
    public int mobsPerWave = 3;

    [Range(10, 500)]
    public float waveCooldown = 10;

    public GameObject[] mobPrefabs;

    private float waveDelayTimer = 0;
    private int waveNumber = 0;
    private GameObject[] spawnPoints;
    private GameObject mobsFolder;

    private void Awake()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        if (mobPrefabs.Length == 0)
            Debug.LogError("Add mob prefabs to " + typeof(SpawnerAI));
        if (spawnPoints.Length == 0)
            Debug.LogWarning("No spawn points on scene.");

        mobsFolder = new GameObject("Mobs");
    }

    private void Update()
    {
        // новая волна - когда убиты все мобы или пришло время
        if (Globals.instance.Mobs.Count() == 0)
            waveDelayTimer = 0;

        if (waveDelayTimer <= 0)
        {
            Debug.LogFormat("wave {0}", waveNumber);

            waveDelayTimer = waveCooldown;
            foreach (var spawnPoint in spawnPoints)
            {
                SpawnMobs(spawnPoint);
            }

            waveNumber++;
            if (waveNumber >= 50)
            {
                mobsPerWave = 10;
            }
            StartCoroutine(WaitNextWave(waveCooldown));
        }
    }

    /// <summary>
    /// Создает ряд случайных мобов.
    /// </summary>
    private void SpawnMobs(GameObject spawnPoint)
    {
        var spawnPos = spawnPoint.transform.position;
        for (int i = 0; i < mobsPerWave; i++)
        {
            var prefab = mobPrefabs[Random.Range(0, mobPrefabs.Length)];
            var pos = new Vector3(spawnPos.x + i * 2, spawnPos.y, spawnPos.z);

            var mob = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
            mob.transform.parent = mobsFolder.transform;
        }
        Debug.LogFormat("spawn {0} mobs", mobsPerWave);
    }

    private IEnumerator WaitNextWave(float cooldown)
    {
        for (waveDelayTimer = cooldown; waveDelayTimer > 0; waveDelayTimer -= Time.deltaTime)
        {
            yield return null;
        }
    }
}