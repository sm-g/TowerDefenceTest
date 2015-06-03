using UnityEngine;

namespace Assets.Scripts
{
    public class MobHP : MonoBehaviour
    {
        [Range(0, 500)]
        public int maxHP = 30;

        [Range(0, 500)]
        public int curHP = 30;

        public Color minHpColor = Color.red;
        public Color maxHpColor = Color.blue;

        private Material material;

        private void Awake()
        {
            //Debug.Log("Awake " + this);

            GameManager.Instance.Register(gameObject);
            curHP = Mathf.Min(curHP, maxHP); // curHP <= maxHP
            material = gameObject.GetComponent<Renderer>().material;
        }

        private void Update()
        {
            material.color = Color.Lerp(minHpColor, maxHpColor, (float)curHP / maxHP);
            if (curHP <= 0)
                Destroy(gameObject);
        }

        public void TakeDamage(int damage)
        {
            if (damage > 0)
                curHP -= damage;
        }

        private void OnDestroy()
        {
            GameManager.Instance.Unregister(gameObject);
        }
    }
}