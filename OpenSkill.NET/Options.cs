using OpenSkill.NET.Models;
using OpenSkill.NET.Types;

namespace OpenSkill.NET;

public class Options
{
    public static Options Default => new()
    {
        Z = Constants.Z,
        Mu = Constants.MU,
        Epsilon = Constants.EPSILON,
        Beta = Constants.BETA,
        Model = new PlackettLuce(),
        PreventSigmaIncrease = false
    };

    public double Z { get; set; } = Constants.Z;
    public double Mu { get; set; } = Constants.MU;

    private double? sigma;

    public double Sigma
    {
        get => sigma ?? Mu / Z;
        set => sigma = value;
    }

    public double? Tau { get; set; } = null;
    public double Epsilon { get; set; } = Constants.EPSILON;

    private double? beta;

    public double Beta
    {
        get => beta ?? Sigma / 2;
        set => beta = value;
    }

    public double BetaSq => Math.Pow(Beta, 2);
    public double TwoBetaSq => 2 * BetaSq;

    public Func<Gamma, double> Gamma { get; set; } = g => Math.Sqrt(g.SigmaSq) / g.C;
    public Model Model { get; set; } = new PlackettLuce();
    public List<double>? Rank { get; set; }
    public List<double>? Score { get; set; }

    public List<double> Tenet(int teamSize) =>
        Rank ?? Score?.Select(x => -x).ToList() ?? Enumerable.Range(0, teamSize).Select(x => (double)x).ToList();

    public bool PreventSigmaIncrease { get; set; }

    public Rating DefaultRating()
    {
        return new Rating(Mu, Sigma);
    }
}