using Engine;


public static class Rand
{
    static Random rand;
    public static void _Start()
    {
        rand = new();
    }
    public static int Randint(int min, int max) => rand.Next(min, max);
    public static float Randint(float min, float max) => min + rand.NextSingle() * (max - min);
    public static T Choose<T>(List<T> list)
    {
        return list[Randint(0,list.Count)];
    }
    //public static int SingleInt => rand.Next(2)-1;
    //public static float SingleFloat => rand.NextSingle() * rand.Next(1, 3) - 1;

}