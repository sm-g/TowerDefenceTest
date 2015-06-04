using UnityEngine;

namespace Assets.Scripts
{
    public class MobAI : MonoBehaviour
    {
        [Range(0, 5)]
        public float speed = 1f;

        private Transform mob;

        private void Awake()
        {
            mob = transform;
        }

        private void Update()
        {
            mob.Translate(Vector3.left * speed * Time.deltaTime);
        }
    }
}