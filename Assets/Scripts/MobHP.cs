using UnityEngine;

namespace Assets.Scripts
{
    public class MobHP : MonoBehaviour
    {
        [Range(0, 100)]
        public int maxHP = 30;

        [Range(0, 100)]
        public int curHP = 30;

        public Color maxDamageColor = Color.red;
        public Color minDamageColor = Color.blue;

        private void Awake()
        {
            GameManager.Instance.Register(gameObject);
            curHP = Mathf.Min(curHP, maxHP); // curHP <= maxHP
        }

        public void TakeDamage(int damage)
        {
            if (damage > 0)
                curHP -= damage;
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
}