using MathNet.Numerics.Distributions;

namespace OpenSkill;

public static class Statistics
{
    private static readonly Normal normal = new(0, 1);

    public static double PhiMajor(double x)
    {
        return normal.CumulativeDistribution(x);
    }

    public static double PhiMajorInverse(double x) => normal.InverseCumulativeDistribution(x);
    public static double PhiMinor(double x) => normal.Density(x);

    public static double V(double x, double t)
    {
        var xt = x - t;
        var denom = PhiMajor(xt);
        return denom < double.Epsilon ? -xt : PhiMinor(xt) / denom;
    }

    public static double W(double x, double t)
    {
        var xt = x - t;
        var denom = PhiMajor(xt);
        if (denom < double.Epsilon)
        {
            return x < 0 ? 1 : 0;
        }

        var vxt = V(x, t);
        return vxt * (vxt + xt);
    }

    public static double Vt(double x, double t)
    {
        var xx = Math.Abs(x);
        var b = PhiMajor(t - xx) - PhiMajor(-t - xx);
        if (b < 1e-5)
        {
            if (x < 0) return -x - t;
            return -x + t;
        }

        var a = PhiMinor(-t - xx) - PhiMinor(t - xx);
        return (x < 0 ? -a : a) / b;
    }

    public static double Wt(double x, double t)
    {
        var xx = Math.Abs(x);
        var b = PhiMajor(t - xx) - PhiMajor(-t - xx);
        var vtxt = Vt(x, t);

        return b < double.Epsilon
            ? 1.0d
            : ((t - xx) * PhiMinor(t - xx) + (t + xx) * PhiMinor(-t - xx)) / b + vtxt * vtxt;
    }
}
