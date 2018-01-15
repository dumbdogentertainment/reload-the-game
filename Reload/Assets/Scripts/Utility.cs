namespace DumbDogEntertainment
{
    public static class Utility
    {
        public static bool IsBetweenInclusive(this float me, float min, float max)
        {
            return me >= min && me <= max;
        }

        public static bool IsBetweenExclusive(this float me, float min, float max)
        {
            return me > min && me < max;
        }
    }
}