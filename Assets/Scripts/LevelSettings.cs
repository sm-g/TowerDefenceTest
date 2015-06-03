using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class LevelSettings : MonoBehaviour
    {
        /// <summary>
        /// Время до победы в секундах.
        /// </summary>
        [Range(00, 10 * 60)]
        public float totalTime = 3 * 60;

        [Range(1, 50)]
        public int livesAtStart = 5;

        internal float finishX;

        public void Awake()
        {
            var finish = GameObject.FindGameObjectWithTag("Finish");
            if (finish == null)
                Debug.LogError("Add tag 'Finish' to finish line.");
            else
                finishX = finish.transform.position.x;
        }
    }
}