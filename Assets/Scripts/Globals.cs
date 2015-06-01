using System.Collections;
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

        var f = GameObject.Find("Finish");
        if (f == null)
            Debug.LogError("Place Finish object to game scene.");
        else
            finishX = f.transform.position.x;
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