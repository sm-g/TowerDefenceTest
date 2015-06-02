using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurretAI : MonoBehaviour
    {
        [Range(0, 50)]
        public float attackMaxDistance = 50.0f;

        [Range(0, 5)]
        public float attackMinDistance = 0.1f;

        [Range(1, 20)]
        public int attackDamage = 10;

        [Range(0, 5)]
        public int shotsAtOnce = 1;

        [Range(0.1f, 50f)]
        public float reloadTimer = 2.5f;

        private float reloadCooldown;
        private int xp;
        private GameObject curTarget;
        private Transform turret;
        private GameObject projectilesFolder;

        private void Awake()
        {
            GameManager.Instance.Register(gameObject);

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
            reloadTimer -= Time.deltaTime;

            if (curTarget != null && curTarget.activeInHierarchy &&
                curTarget.InRadialArea(turret, attackMinDistance, attackMaxDistance))
            {
                // есть цель в допустимых пределах

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

        /// <summary>
        /// Выпускает снаряд по цели.
        /// </summary>
        private void Shoot(GameObject target)
        {
            var lookAtTarget = Quaternion.LookRotation(target.transform.position - turret.position);
            var proj = Instantiate(Globals.instance.projectilePrefab, turret.position, lookAtTarget) as GameObject;
            projectilesFolder.AddChild(proj);

            // прицеливаем снаряд
            var ai = proj.GetComponent<ProjectileAI>();
            if (ai != null)
            {
                ai.Initialize(target, this, attackDamage);
            }
        }

        /// <summary>
        /// Возвращает ближайшего моба в области атаки.
        /// </summary>
        private GameObject GetNearestTarget()
        {
            float closestMobDistance = float.MaxValue;
            GameObject nearestmob = null;

            foreach (var target in GameManager.Instance.Mobs)
            {
                if (target.InRadialArea(turret, attackMinDistance, closestMobDistance))
                {
                    closestMobDistance = Vector3.Distance(target.transform.position, turret.position);
                    nearestmob = target;
                }
            }

            return closestMobDistance > attackMaxDistance ? null : nearestmob;
        }

        public void AddXp(int points)
        {
            if (points > 0)
                xp += points;
        }

        public override string ToString()
        {
            return "{0}*{1}/{2} hp/s, {3} m".FormatStr(
                        attackDamage,
                        shotsAtOnce,
                        reloadTimer,
                        attackMaxDistance);
        }
    }
}