using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class SpawnerAI : MonoBehaviour
    {
        private static GameObject mobsFolder;
        private int waveNumber = 0;
        private GameObject[] spawnPoints;
        private float waveCooldown;
        private int mobsPerWave;
        private GameObject[] mobPrefabs;

        public void Initialize(float waveCooldown, int mobsPerWave, GameObject[] spawnPoints, GameObject[] mobPrefabs)
        {
            this.waveCooldown = waveCooldown;
            this.mobsPerWave = mobsPerWave;
            this.spawnPoints = spawnPoints;
            this.mobPrefabs = mobPrefabs;

            mobsFolder = mobsFolder ?? new GameObject("Mobs");
        }

        private void Start()
        {
            // новая волна - в начале раунда, когда убиты все мобы или пришло время
            GameManager.Instance.RoundStarted += (s, e) =>
            {
                StartWave();
                StartCoroutine(OneSecondTimer(() =>
                {
                    if (GameManager.Instance.Mobs.Count() == 0)
                        StartWave();
                }));
            };
            GameManager.Instance.Won += (s, e) =>
            {
                // stop spawn
                StopAllCoroutines();
            };
            // on lost, mobs continue to arrive
        }

        /// <summary>
        /// Начинает новую волну.
        /// </summary>
        private void StartWave()
        {
            StopCoroutine(WaitNextWave());

            Debug.LogFormat("=== wave {0}", waveNumber);

            foreach (var spawnPoint in spawnPoints)
            {
                SpawnMobs(spawnPoint);
            }

            waveNumber++;

            StartCoroutine(WaitNextWave());
        }

        /// <summary>
        /// Создает ряд случайных мобов через 1 клетку в точке респауна.
        /// </summary>
        private void SpawnMobs(GameObject spawnPoint)
        {
            var spawnPos = spawnPoint.transform.position;
            for (int i = 0; i < mobsPerWave; i++)
            {
                var prefab = mobPrefabs[UnityEngine.Random.Range(0, mobPrefabs.Length)];
                var pos = new Vector3(spawnPos.x + i * 2, spawnPos.y, spawnPos.z);

                var mob = GameObject.Instantiate(prefab, pos, Quaternion.identity) as GameObject;
                mobsFolder.AddChild(mob);
            }
            Debug.LogFormat("spawned {0} mobs", mobsPerWave);
        }

        private IEnumerator WaitNextWave()
        {
            yield return new WaitForSeconds(waveCooldown);
            StartWave();
        }

        private IEnumerator OneSecondTimer(Action act)
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                act();
            }
        }
    }
}