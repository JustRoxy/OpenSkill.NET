using OpenSkill.Types;

namespace OpenSkill;

public static class Constants
{
    public const double Z = 3d;
    public const double MU = 25d;
    public static double Tau(Options options) => options.Mu / 300;
    public const double SIGMA = MU / Z;
    public const double EPSILON = 0.0001d;
    public const double BETA = SIGMA / 2;
}
