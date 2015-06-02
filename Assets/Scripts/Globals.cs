using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Globals : SingletonMB<Globals>
    {
        /// <summary>
        /// Время до победы в секундах.
        /// </summary>
        [Range(60, 10 * 60)]
        public float totalTime = 3 * 60;

        [Range(1, 50)]
        public int livesAtStart = 5;

        [Range(1, 10)]
        public int mobsPerWave = 3;

        /// <summary>
        /// Задержка между волнами в секундах.
        /// </summary>
        [Range(10, 500)]
        public float waveCooldown = 10;

        public GameObject[] mobPrefabs;
        public GameObject[] turretPrefabs;
        public GameObject projectilePrefab;
        internal float finishX;
        internal float goalTime;

        private void Awake()
        {
            goalTime = totalTime;

            CheckPrefabs();

            var finish = GameObject.FindGameObjectWithTag("Finish");
            if (finish == null)
                Debug.LogError("Add tag 'Finish' to finish line.");
            else
                finishX = finish.transform.position.x;

            var spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
            if (spawnPoints.Length == 0)
                Debug.LogWarning("No spawn points on scene.");

            var spawner = GameObject.Find("Spawner");
            if (spawner == null)
                Debug.LogError("No spawner on scene.");
            else
                spawner.GetComponent<SpawnerAI>()
                       .Initialize(waveCooldown, mobsPerWave, spawnPoints, mobPrefabs);

        }

        private void CheckPrefabs()
        {
            if (projectilePrefab == null)
                Debug.LogError("Add projectile prefab to " + typeof(Globals));
            if (mobPrefabs.Length == 0)
                Debug.LogError("Add mob prefabs to " + typeof(Globals));
            if (turretPrefabs.Length == 0)
                Debug.LogError("Add turret prefabs to " + typeof(Globals));

            if (projectilePrefab != null && projectilePrefab.GetComponent<ProjectileAI>() == null)
                Debug.LogErrorFormat("Add {0} to projectile prefab", typeof(ProjectileAI));

            foreach (var prefab in turretPrefabs)
            {
                if (prefab.GetComponent<TurretAI>() == null)
                    Debug.LogError("No " + typeof(TurretAI) + " in prefab.");
            }

        }

        private void Start()
        {
            StartCoroutine(DoChecks());
        }

        private IEnumerator DoChecks()
        {
            while (true)
            {
                GameManager.Instance.CheckPassedMobs();
                GameManager.Instance.CheckRound();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}