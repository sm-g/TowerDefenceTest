using UnityEngine;

public class MobHP : MonoBehaviour
{
    [Range(0, 100)]
    public float maxHP = 30;
    [Range(0, 100)]
    public float curHP = 30;
    public Color maxDamageColor = Color.red;
    public Color minDamageColor = Color.blue;

    private void Awake()
    {
        GameManager.Instance.Register(gameObject);
        curHP = Mathf.Min(curHP, maxHP); // curHP <= maxHP
    }

    public void ChangeHP(float adjust)
    {
        curHP += adjust;
        curHP = Mathf.Min(curHP, maxHP);
    }

    private void Update()
    {
        // цвет моба зависит от hp
        gameObject.GetComponent<Renderer>().material.color = Color.Lerp(maxDamageColor, minDamageColor, curHP / maxHP);
        if (curHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.Unregister(gameObject);
    }
}