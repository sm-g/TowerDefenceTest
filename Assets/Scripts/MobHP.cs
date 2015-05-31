using UnityEngine;

public class MobHP : MonoBehaviour
{
    [Range(0, 500)]
    public float maxHP = 30;
    [Range(0, 500)]
    public float curHP = 30;
    public Color MaxDamageColor = Color.red;
    public Color MinDamageColor = Color.blue;

    private void Awake()
    {
        Globals.instance.Register(gameObject);
        curHP = Mathf.Min(curHP, maxHP); // curHP <= maxHP
    }

    public void ChangeHP(float adjust)
    {
        curHP += adjust;
        curHP = Mathf.Min(curHP, maxHP);
    }

    private void Update()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.Lerp(MaxDamageColor, MinDamageColor, curHP / maxHP);
        if (curHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Globals.instance.Unregister(gameObject);
    }
}