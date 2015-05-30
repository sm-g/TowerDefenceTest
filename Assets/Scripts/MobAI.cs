using UnityEngine;

public class MobAI : MonoBehaviour
{
    public float mobMinSpeed = 0.5f;
    public float mobMaxSpeed = 2.0f;

    private float currentSpeed;
    private Transform mob;

    private void Awake()
    {
        mob = transform;
        currentSpeed = 1;// Random.Range(mobMinSpeed, mobMaxSpeed);
    }

    private void Update()
    {
        mob.Translate(-currentSpeed * Time.deltaTime, 0, 0, Space.World);
        if (mob.position.x < 40)
        {
            gameObject.GetComponent<MobHP>().curHP = 0;
            Globals.instance.passedMobs++;
        }
    }
}