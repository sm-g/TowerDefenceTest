using UnityEngine;

public class MobAI : MonoBehaviour
{
    public float speed = 1f;

    private Transform mob;

    private void Awake()
    {
        mob = transform;
    }

    private void Update()
    {
        mob.Translate(Vector3.left * speed * Time.deltaTime);
        if (mob.position.x < Globals.instance.finishX)
        {
            gameObject.GetComponent<MobHP>().curHP = 0;
            Globals.instance.passedMobs++;
        }
    }

}