using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    public float attackMaxDistance = 50.0f;
    public float attackMinDistance = 5.0f;
    public float attackDamage = 10.0f;
    public float reloadTimer = 2.5f;
    public const float reloadCooldown = 2.5f;

    private GameObject curTarget;
    private Transform turret;

    private void Awake()
    {
        Globals.instance.Register(gameObject);
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