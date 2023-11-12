namespace HikVisionConverter.Communication;

internal class UnitConverter
{
    public static double DegreeToMils(double degree)
    {
        return degree * (6400 / 360);
    }

    public static double RadianToDegree(double rad)
    {
        return (rad / Math.PI * 180 + 360) % 360;
    }

    public static double MilsToDegree(double mils)
    {
        return 360 * mils / 6400;
    }

    public static double DegreeToRadian(double degree)
    {
        return degree * Math.PI / 180;
    }

    public static double RadianToMils(double rad)
    {
        return rad * (6400 / (2 * Math.PI));
    }
}
