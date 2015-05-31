using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [Range(0, 500)]
    public float attackMaxDistance = 50.0f;
    [Range(0, 5)]
    public float attackMinDistance = 0.1f;
    public float attackDamage = 10.0f;
    [Range(0, 5)]
    public int shotsAtOnce = 1;
    [Range(0.1f, 50f)]
    public float reloadTimer = 2.5f;

    float reloadCooldown;
    private GameObject curTarget;
    private Transform turret;

    private void Awake()
    {
        Globals.instance.Register(gameObject);

        if (attackMaxDistance < attackMinDistance)
            Debug.LogWarning("Attack max distance less than min distance.");

        reloadCooldown = reloadTimer;
    }

    private void Start()
    {
        turret = transform;
    }

    private void Update()
    {
        if (curTarget != null && curTarget.activeInHierarchy)
        {
            float distance = Vector3.Distance(turret.position, curTarget.transform.position);
            if (attackMinDistance < distance && distance < attackMaxDistance)
            {
                if (reloadTimer > 0) reloadTimer -= Time.deltaTime;
                if (reloadTimer < 0) reloadTimer = 0;
                if (reloadTimer == 0)
                {
                    MobHP mhp = curTarget.GetComponent<MobHP>();
                    if (mhp != null)
                        mhp.ChangeHP(-attackDamage);
                    reloadTimer = reloadCooldown;
                }
                return;
            }
        }

        curTarget = GetNearestTarget();
    }

    public GameObject GetNearestTarget()
    {
        float closestMobDistance = float.MaxValue;
        GameObject nearestmob = null;
        List<GameObject> sortingMobs = GameObject.FindGameObjectsWithTag("Mob").ToList();

        foreach (var target in sortingMobs)
        {
            if ((Vector3.Distance(target.transform.position, turret.position) < closestMobDistance))
            {
                closestMobDistance = Vector3.Distance(target.transform.position, turret.position);
                nearestmob = target;
            }
        }
        return closestMobDistance > attackMaxDistance ? null : nearestmob;
    }
}