using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class SpawnerAI : MonoBehaviour
    {
        /// <summary>
        /// Задержка между волнами в секундах.
        /// </summary>
        [Range(10, 500)]
        public float waveCooldown = 10;

        public IntCount mobsPerWave = new IntCount(3, 10);

        public GameObject[] mobPrefabs;

        private static GameObject mobsFolder;
        private int waveNumber;
        private GameObject[] spawnPoints;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (mobPrefabs.Length == 0)
                Debug.LogError("Add mob prefabs to " + this.GetType());

            spawnPoints = GameObject.FindGameObjectsWithTag(Tags.Respawn);
            if (spawnPoints.Length == 0)
                Debug.LogWarning("No spawn points on scene.");

            if (!mobsPerWave.IsValid())
                Debug.LogWarning("Mobs Per Wave count invalid.");

            mobsFolder = mobsFolder ?? new GameObject(Generated.Mobs);
        }

        private void Start()
        {
            GameManager.Instance.StateChanged += (s, e) =>
            {
                switch (e.arg)
                {
                    case GameState.Start:
                        break;

                    case GameState.Playing:
                        // новая волна - в начале раунда, когда убиты все мобы или пришло время
                        StopAllCoroutines();
                        waveNumber = 0;

                        StartWave();

                        StartCoroutine(OneSecondTimer(() =>
                        {
                            if (CanStartOutOfTurnWave())
                            {
                                Debug.Log("Out Of Turn Wave");
                                StartWave();
                            }
                        }));
                        break;

                    case GameState.Won:
                        // stop spawn
                        StopAllCoroutines();
                        break;

                    case GameState.Lost:
                        break;
                }
            };
        }

        /// <summary>
        /// Можно ли начать волну до завершения времени на текущую.
        /// </summary>
        private static bool CanStartOutOfTurnWave()
        {
            // lost - продолжаем спаунить мобов
            return GameManager.Instance.Mobs.Count() == 0 &&
                (GameManager.Instance.State == GameState.Playing ||
                 GameManager.Instance.State == GameState.Lost);
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

            Debug.Log("total mobs = " + GameManager.Instance.Mobs.Count());

            waveNumber++;

            StartCoroutine(WaitNextWave());
        }

        /// <summary>
        /// Создает ряд случайных мобов через 1 клетку в точке респауна.
        /// </summary>
        private void SpawnMobs(GameObject spawnPoint)
        {
            var spawnPos = spawnPoint.transform.position;
            var count = UnityEngine.Random.Range(mobsPerWave.minimum, mobsPerWave.maximum + 1);
            for (int i = 0; i < count; i++)
            {
                var prefab = mobPrefabs[UnityEngine.Random.Range(0, mobPrefabs.Length)];
                var pos = new Vector3(spawnPos.x + i * 2, spawnPos.y, spawnPos.z);

                var mob = GameObject.Instantiate(prefab, pos, Quaternion.identity) as GameObject;
                mobsFolder.AddChild(mob);
            }
            Debug.LogFormat("spawned {0} mobs", count);
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