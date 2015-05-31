using System.Linq;
using UnityEngine;

public class ProjectileAI : MonoBehaviour
{
    private float attackDistance = 0.1f;
    private float speed = 10;

    private Transform projectile;

    public float damage { get; set; }

    public GameObject target { get; set; }

    private void Start()
    {
        projectile = transform;
    }

    private void Update()
    {
        if (target != null && target.activeInHierarchy)
        {
            if (target.InRadialArea(projectile, 0, attackDistance))
                AttackTarget();
            else
                FollowTarget();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FollowTarget()
    {
        projectile.rotation = Quaternion.Slerp(
            projectile.rotation,
            Quaternion.LookRotation(target.transform.position - projectile.position),
            1000);

        var step = speed * Time.deltaTime;
        projectile.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
    }

    private void AttackTarget()
    {
        MobHP mhp = target.GetComponent<MobHP>();
        if (mhp != null)
            mhp.ChangeHP(-damage);

        // атака только на одну цель
        target = null;
    }
}