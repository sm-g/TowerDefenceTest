using System;
using System.Linq;

namespace Assets.Scripts
{
    [Serializable]
    public class IntCount
    {
        public int minimum;
        public int maximum;

        public IntCount(int min, int max)
        {
            minimum = min;
            maximum = max;
        }

        public bool IsValid() { return minimum <= maximum; }
    }
    [Serializable]
    public class FloatCount
    {
        public float minimum;
        public float maximum;

        public FloatCount(float min, float max)
        {
            minimum = min;
            maximum = max;
        }

        public bool IsValid() { return minimum <= maximum; }
    }
}