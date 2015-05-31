using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretPlacer : Singleton<TurretPlacer>
{
    public Transform[] turretPrefabs;

    private List<TurretAI> _turrets = new List<TurretAI>();

    public IEnumerable<TurretAI> Turrets { get { return _turrets; } }

    private void Awake()
    {
        if (turretPrefabs.Length == 0)
            Debug.LogError("Add turret prefabs to " + typeof(SpawnerAI));

        foreach (var prefab in turretPrefabs)
        {
            var ai = prefab.GetComponent<TurretAI>();
            if (ai == null)
                Debug.LogError("No " + typeof(TurretAI) + " in prefab");
            else
                _turrets.Add(ai);
        }
    }
}