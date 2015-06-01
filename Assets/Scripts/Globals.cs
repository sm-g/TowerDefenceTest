using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Globals : SingletonMB<Globals>
{
    [Range(60, 10 * 60)]
    public float totalTime = 3 * 60;

    [Range(1, 50)]
    public int mobsPassedToLose = 3;

    public GameObject[] mobPrefabs;
    public GameObject[] turretPrefabs;
    public GameObject projectilePrefab;
    internal float finishX;
    internal float goalTime;
    private Dictionary<TurretAI, GameObject> _turrets = new Dictionary<TurretAI, GameObject>();

    public Dictionary<TurretAI, GameObject> Turrets { get { return _turrets; } }
    public void Awake()
    {
        goalTime = totalTime;

        if (projectilePrefab == null)
            Debug.LogError("Add projectile prefab to " + typeof(Globals));
        if (mobPrefabs.Length == 0)
            Debug.LogError("Add mob prefabs to " + typeof(Globals));
        if (turretPrefabs.Length == 0)
            Debug.LogError("Add turret prefabs to " + typeof(Globals));

        if (projectilePrefab != null && projectilePrefab.GetComponent<ProjectileAI>() == null)
            Debug.LogErrorFormat("Add {0} to projectile prefab", typeof(ProjectileAI));

        var f = GameObject.FindGameObjectWithTag("Finish");
        if (f == null)
            Debug.LogError("Add tag 'Finish' to finish line.");
        else
            finishX = f.transform.position.x;

        foreach (var prefab in turretPrefabs)
        {
            var ai = prefab.GetComponent<TurretAI>();
            if (ai == null)
                Debug.LogError("No " + typeof(TurretAI) + " in prefab.");
            else
                _turrets.Add(ai, prefab);
        }
    }

    public void Start()
    {
        StartCoroutine(DoChecks());
        StartCoroutine(WaitRound());
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

    private IEnumerator WaitRound()
    {
        yield return new WaitForSeconds(Globals.instance.goalTime - Time.time);
    }

    private void OnLose()
    {
        StopAllCoroutines();
    }
}