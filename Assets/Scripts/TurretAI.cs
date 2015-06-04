using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurretAI : MonoBehaviour
    {
        public FloatCount attackDistance = new FloatCount(0.1f, 6.0f);

        [Range(1, 100)]
        public int attackDamage = 10;

        [Range(0, 5)]
        public int shotsAtOnce = 1;

        [Range(0.1f, 50f)]
        public float reloadTimer = 2.5f;

        public Color noTargetColor = Color.grey;
        public Color attackingColor = Color.magenta;

        private static GameObject projectilesFolder;
        private static float maxAttackDistance = 15;
        private float reloadCooldown;
        private int xp;
        private GameObject _target;
        private Material material;
        private Transform turret;

        public GameObject Target
        {
            get { return _target; }
            private set
            {
                if (ReferenceEquals(_target, value)) return;

                Debug.LogFormat("{0} targets '{1}'", this, value);
                _target = value;
                material.color = Target == null ? noTargetColor : attackingColor;
            }
        }

        private void Awake()
        {
            GameManager.Instance.Register(gameObject);

            if (!attackDistance.IsValid())
                Debug.LogWarning("Attack max distance less than min distance.");

            projectilesFolder = projectilesFolder ?? new GameObject(Generated.Projectiles);
            reloadCooldown = reloadTimer;
            material = gameObject.GetComponent<Renderer>().material;
            material.color = noTargetColor;
        }

        private void Start()
        {
            turret = transform;
            // высота по дальности атаки
            var yScale = Mathf.Lerp(1, 3, attackDistance.maximum / maxAttackDistance);
            turret.localScale = new Vector3(1, yScale, 1);
        }

        private void Update()
        {
            reloadTimer -= Time.deltaTime;

            if (Target != null && Target.activeInHierarchy &&
                Target.InRadialArea(turret, attackDistance.minimum, attackDistance.maximum))
            {
                // есть цель в допустимых пределах

                if (reloadTimer <= 0)
                {
                    // и пора стрелять
                    reloadTimer = reloadCooldown;

                    // выпускаем снаряды
                    for (int i = 0; i < shotsAtOnce; i++)
                        Shoot(Target);
                }
            }
            else
            {
                // выбираем новую цель
                Target = GetNearestTarget();
            }
        }

        /// <summary>
        /// Выпускает снаряд по цели.
        /// </summary>
        private void Shoot(GameObject target)
        {
            var lookAtTarget = Quaternion.LookRotation(target.transform.position - turret.position);
            var proj = Instantiate(Builder.Instance.projectilePrefab, turret.position, lookAtTarget) as GameObject;
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
                if (target.InRadialArea(turret, attackDistance.minimum, closestMobDistance))
                {
                    var d = Vector3.Distance(target.transform.position, turret.position);
                    if (d < closestMobDistance)
                    {
                        closestMobDistance = d;
                        nearestmob = target;
                    }
                }
            }

            var res = closestMobDistance > attackDistance.maximum ? null : nearestmob;
            //if (res != null)
            //    Debug.LogFormat("nearest to {0} in {1} m", this, Mathf.FloorToInt(closestMobDistance));
            return res;
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
                        reloadCooldown,
                        attackDistance.maximum);
        }
    }
}