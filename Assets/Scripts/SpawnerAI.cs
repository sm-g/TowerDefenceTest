using System.Collections;
using System.Linq;
using UnityEngine;

public class SpawnerAI : MonoBehaviour
{
    private static GameObject mobsFolder;
    private float waveDelayTimer = 0;
    private int waveNumber = 0;
    private GameObject[] spawnPoints;
    private float waveCooldown;
    private int mobsPerWave;

    public void Initialize(float waveCooldown, int mobsPerWave, GameObject[] spawnPoints)
    {
        this.waveCooldown = waveCooldown;
        this.mobsPerWave = mobsPerWave;
        this.spawnPoints = spawnPoints;

        mobsFolder = mobsFolder ?? new GameObject("Mobs");
    }

    private void Update()
    {
        if (GameManager.Instance.Mobs.Count() == 0)
            waveDelayTimer = 0;

        if (waveDelayTimer <= 0)
        {
            // убиты все мобы или пришло время
            MakeNewWave();
            StartCoroutine(WaitNextWave(waveCooldown));
        }
    }

    private void MakeNewWave()
    {
        Debug.LogFormat("wave {0}", waveNumber);

        waveDelayTimer = waveCooldown;
        foreach (var spawnPoint in spawnPoints)
        {
            SpawnMobs(spawnPoint);
        }

        waveNumber++;
    }

    /// <summary>
    /// Создает ряд случайных мобов через 1 клетку в точке респауна.
    /// </summary>
    private void SpawnMobs(GameObject spawnPoint)
    {
        var spawnPos = spawnPoint.transform.position;
        for (int i = 0; i < mobsPerWave; i++)
        {
            var prefab = Globals.instance.mobPrefabs[Random.Range(0, Globals.instance.mobPrefabs.Length)];
            var pos = new Vector3(spawnPos.x + i * 2, spawnPos.y, spawnPos.z);

            var mob = GameObject.Instantiate(prefab, pos, Quaternion.identity) as GameObject;
            mobsFolder.AddChild(mob);
        }
        Debug.LogFormat("spawned {0} mobs", mobsPerWave);
    }

    private IEnumerator WaitNextWave(float cooldown)
    {
        for (waveDelayTimer = cooldown; waveDelayTimer > 0; waveDelayTimer -= Time.deltaTime)
        {
            yield return null;
        }
    }
}