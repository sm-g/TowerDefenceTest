using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class ProjectileAI : MonoBehaviour
    {
        private float attackDistance = 0.1f;
        private float speed = 10;
        private TurretAI turret;
        private Transform projectile;

        public int Damage { get; private set; }

        /// <summary>
        /// Цель снаряда. Снаряд без цели уничтожается.
        /// </summary>
        public GameObject Target { get; private set; }

        /// <summary>
        /// Прицеливает снаряд.
        /// </summary>
        /// <param name="target">Цель снаряда.</param>
        /// <param name="turret">Башня, выпустившая снаряд.</param>
        /// <param name="damage">Урон</param>
        public void Initialize(GameObject target, TurretAI turret, int damage)
        {
            this.turret = turret;
            Target = target;
            Damage = damage;
        }

        private void Start()
        {
            projectile = transform;
        }

        private void Update()
        {
            if (Target != null && Target.activeInHierarchy)
            {
                if (Target.InRadialArea(projectile, 0, attackDistance))
                    AttackTarget();
                else
                    FollowTarget();
            }
            else
            {
                // снаряд без цели
                Destroy(gameObject);
            }
        }

        private void FollowTarget()
        {
            var step = speed * Time.deltaTime;
            projectile.position = Vector3.MoveTowards(transform.position, Target.transform.position, step);
        }

        private void AttackTarget()
        {
            MobHP mhp = Target.GetComponent<MobHP>();
            if (mhp != null)
            {
                mhp.TakeDamage(Damage);
                turret.AddXp(mhp.maxHP);
            }

            // атака только на одну цель
            Target = null;
        }
    }
}