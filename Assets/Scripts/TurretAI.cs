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

    public GameObject projectilePrefab;

    private float reloadCooldown;
    private GameObject curTarget;
    private Transform turret;
    private GameObject projectilesFolder;

    private void Awake()
    {
        Globals.instance.Register(gameObject);

        if (projectilePrefab == null)
            Debug.LogError("Add projectile prefab to " + typeof(TurretAI));
        if (projectilePrefab != null && projectilePrefab.GetComponent<ProjectileAI>() == null)
            Debug.LogErrorFormat("Add {0} to projectile prefab", typeof(ProjectileAI));

        if (attackMaxDistance < attackMinDistance)
            Debug.LogWarning("Attack max distance less than min distance.");

        projectilesFolder = GameObject.Find("Projectiles");
        if (projectilesFolder == null)
            projectilesFolder = new GameObject("Projectiles");

        reloadCooldown = reloadTimer;
    }

    private void Start()
    {
        turret = transform;
    }

    private void Update()
    {
        if (curTarget != null && curTarget.activeInHierarchy &&
            curTarget.InRadialArea(turret, attackMinDistance, attackMaxDistance))
        {
            // есть цель в допустимых пределах
            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0)
            {
                // и пора стрелять
                reloadTimer = reloadCooldown;

                // выпускаем снаряды
                for (int i = 0; i < shotsAtOnce; i++)
                    Shoot(curTarget);
            }
        }
        else
        {
            // выбираем новую цель
            curTarget = GetNearestTarget();
        }
    }

    private void Shoot(GameObject curTarget)
    {
        var proj = Instantiate(projectilePrefab, turret.position, projectilePrefab.transform.rotation) as GameObject;
        proj.transform.parent = projectilesFolder.transform;

        var ai = proj.GetComponent<ProjectileAI>();
        if (ai != null)
        {
            // прицеливаем снаряд
            ai.target = curTarget;
            ai.damage = attackDamage;
        }
        else
            GameObject.Destroy(proj);
    }

    /// <summary>
    /// Возвращает ближайшего моба в области атаки.
    /// </summary>
    public GameObject GetNearestTarget()
    {
        float closestMobDistance = float.MaxValue;
        GameObject nearestmob = null;

        foreach (var target in Globals.instance.Mobs)
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