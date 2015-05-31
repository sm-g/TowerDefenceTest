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

    public Transform projectilePrefab;

    private float reloadCooldown;
    private GameObject curTarget;
    private Transform turret;

    private void Awake()
    {
        Globals.instance.Register(gameObject);

        if (projectilePrefab == null)
            Debug.LogError("Add projectile prefab to " + typeof(TurretAI));

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
            // если цель в допустимых пределах
            if (curTarget.InRadialArea(turret, attackMinDistance, attackMaxDistance))
            {
                reloadTimer -= Time.deltaTime;

                // и пора стрелять
                if (reloadTimer <= 0)
                {
                    reloadTimer = reloadCooldown;

                    // выпускаем снаряды
                    for (int i = 0; i < shotsAtOnce; i++)
                        Shoot(curTarget);
                }
                return;
            }

            // выбираем новую цель
        }

        curTarget = GetNearestTarget();
    }

    private void Shoot(GameObject curTarget)
    {
        var proj = Instantiate(projectilePrefab, turret.position, projectilePrefab.rotation) as Transform;
        var ai = proj.GetComponent<ProjectileAI>();
        if (ai != null)
        {
            ai.target = curTarget;
            ai.damage = attackDamage;
        }
        else
            GameObject.Destroy(proj);
    }

    public GameObject GetNearestTarget()
    {
        float closestMobDistance = float.MaxValue;
        GameObject nearestmob = null;
        var mobs = GameObject.FindGameObjectsWithTag("Mob").ToList();

        foreach (var target in mobs)
        {
            if (target.InRadialArea(turret, attackMinDistance, closestMobDistance))
            {
                closestMobDistance = Vector3.Distance(target.transform.position, turret.position);
                nearestmob = target;
            }
        }

        return closestMobDistance > attackMaxDistance ? null : nearestmob;
    }

}