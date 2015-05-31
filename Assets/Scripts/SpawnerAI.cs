using System.Collections;
using System.Linq;
using UnityEngine;

public class SpawnerAI : MonoBehaviour
{
    [Range(1, 500)]
    public int mobsPerWave = 3;
    [Range(10, 500)]
    public float waveCooldown = 10;
    public Transform[] mobPrefabs;
    public GameObject mobsFolder;

    private float waveDelayTimer = 0;
    private int waveNumber = 0;
    private GameObject[] spawnPoints;

    private void Awake()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        if (mobPrefabs.Length == 0)
            Debug.LogError("Add mob prefabs to " + typeof(SpawnerAI));
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

            if (spawnPoints != null)
            {
                foreach (var spawnPoint in spawnPoints)
                {
                    var pos = spawnPoint.transform.position;
                    for (int i = 0; i < mobsPerWave; i++)
                    {
                        var prefab = mobPrefabs[Random.Range(0, mobPrefabs.Length)];
                        var mob = Instantiate(prefab, new Vector3(pos.x + i * 2,
                              pos.y,
                              pos.z), Quaternion.identity) as GameObject;
                        // mob.transform.parent = mobsFolder.transform;
                    }
                    Debug.LogFormat("create {0} mobs", mobsPerWave);
                }
            }

            waveNumber++;
            if (waveNumber >= 50)
            {
                mobsPerWave = 10;
            }
            StartCoroutine(WaitNextWave(waveCooldown));
        }
    }

    private IEnumerator WaitNextWave(float cooldown)
    {
        for (waveDelayTimer = cooldown; waveDelayTimer > 0; waveDelayTimer -= Time.deltaTime)
        {
            yield return null;
        }
    }
}