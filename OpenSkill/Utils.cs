using OpenSkill.Types;

namespace OpenSkill;

public class Utils
{
    private readonly Options _options;

    public Utils(Options options)
    {
        _options = options;
    }

    public static double Score(double q, double i)
    {
        if (q < i) return 0.0d;
        return q > i ? 1.0d : 0.5;
    }

    public int[] Rankings(List<Team> teams)
    {
        var teamScores = _options.Tenet(teams.Count);
        var outRank = new int[teamScores.Count];
        var s = 0;
        for (var i = 0; i < teams.Count; i++)
        {
            if (i > 0 && teamScores[i - 1] < teamScores[i])
            {
                s = i;
            }

            outRank[i] = s;
        }

        return outRank;
    }

    public double Ordinal(Rating rating)
    {
        return rating.Mu - _options.Z * rating.Sigma;
    }

    public List<TeamRating> TeamRating(List<Team> game)
    {
        var rank = Rankings(game);

        return game.Select((team, i) =>
        {
            return new TeamRating
            {
                Mu = team.Ratings.Select(v => v.Mu).Aggregate(0d, (x, y) => x + y),
                SigmaSq = team.Ratings.Select(x => x.Sigma * x.Sigma).Aggregate(0d, (x, y) => x + y),
                Team = team,
                Rank = rank[i]
            };
        }).ToList();
    }

    public double UtilC(IEnumerable<TeamRating> ratings)
    {
        var rr = ratings.Select(x => x.SigmaSq + _options.BetaSq).Aggregate(0d, (x, y) => x + y);

        return Math.Sqrt(rr);
    }

    public (List<T> teams, List<double> tenet) Unwind<T>(List<double> tenet, IEnumerable<T> teams)
    {
        List<(double, int, T)> result = teams.Select((t, i) => (tenet[i], i, t))
            .OrderBy(x => x.Item1)
            .ToList();

        var dest = new List<T>();
        var derank = new List<double>();
        foreach (var (_, i, o) in result)
        {
            dest.Add(o);
            derank.Add(i);
        }

        return (dest, derank);
    }

    public double[] UtilSumQ(List<TeamRating> ratings, double c)
    {
        return ratings.Select(q =>
                ratings
                    .Where(i => i.Rank >= q.Rank)
                    .Select(i => Math.Exp(i.Mu / c))
                    .Aggregate(0d, (x, y) => x + y))
            .ToArray();
    }

    public int[] UtilA(List<TeamRating> ratings)
    {
        return ratings.Select(i => ratings.Count(q => Math.Abs(i.Rank - q.Rank) < double.Epsilon)).ToArray();
    }

    public double Gamma(double sigmaSq, double c) => Math.Sqrt(sigmaSq) / c;
}