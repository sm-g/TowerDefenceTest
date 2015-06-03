using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Globals : SingletonMB<Globals>
    {
        public GameObject[] mobPrefabs;
        public GameObject[] turretPrefabs;
        public GameObject projectilePrefab;

        private void Awake()
        {
            CheckPrefabs();

            var scripts = GameObject.Find("Scripts");
            if (scripts == null)
                Debug.LogError("No Scripts on scene.");
            else
            {
                var spawner = scripts.GetComponent<SpawnerAI>();
                if (spawner == null)
                    Debug.LogError("Add SpawnerAI to Scripts.");
                else
                    spawner.Initialize(mobPrefabs);
            }
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


    }
}