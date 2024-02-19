using MathNet.Numerics.Statistics;
using OpenSkill.Types;

namespace OpenSkill;

public class OpenSkill
{
    public Options Options { get; }
    private readonly Utils _utils;

    public OpenSkill(Options options)
    {
        Options = options;
        _utils = new Utils(options);
    }

    public List<Team> Rate(List<Team> teams)
    {
        var model = Options.Model;

        var tauScaled = teams;
        if (Options.Tau is not null)
        {
            var tauSquared = Options.Tau.Value * Options.Tau.Value;
            tauScaled = teams.Select(x =>
                    new Team(x.Ratings
                        .Select(v => new Rating(v.Mu, Math.Sqrt(v.Sigma * v.Sigma + tauSquared), v.Reference))
                        .ToList()))
                .ToList();
        }

        var (sortedTeams, tenet) = _utils.Unwind(Options.Tenet(tauScaled.Count), tauScaled);
        var result = model.Rate(sortedTeams, Options);
        (result, _) = _utils.Unwind(tenet, result);

        if (Options.Tau is not null && Options.PreventSigmaIncrease)
        {
            for (var i = 0; i < result.Count; i++)
            {
                for (var j = 0; j < result[i].Ratings.Count; j++)
                {
                    var rating = result[i].Ratings[j];
                    var originalRating = teams[i].Ratings[j];
                    if (rating.Sigma > originalRating.Sigma)
                    {
                        rating.Sigma = originalRating.Sigma;
                    }
                }
            }
        }

        return result;
    }

    private List<(int rank, double probability)> PredictRank(List<Team> pureTeams)
    {
        var n = pureTeams.Count;
        var totalPlayerCount = pureTeams.Sum(x => x.Ratings.Count);
        var denom = n * (n - 1d) / 2;
        var drawProbability = 1 / totalPlayerCount;
        var drawMargin = Math.Sqrt(totalPlayerCount) * Options.Beta *
                         Statistics.PhiMajorInverse((1 + drawProbability) / 2d);

        var teams = _utils.TeamRating(pureTeams);

        double[] pairwiseProbabilities = new double[teams.Count];

        for (int i = 0; i < teams.Count; i++)
        {
            var pairA = teams[i];

            foreach (var pairB in teams.Where(x => x != pairA))
            {
                var muA = pairA.Mu;
                var sigmaA = pairA.SigmaSq;
                var muB = pairB.Mu;
                var sigmaB = pairB.SigmaSq;

                var probability = Statistics.PhiMajor((muA - muB - drawMargin) /
                                                      Math.Sqrt(n * Options.BetaSq +
                                                                sigmaA + sigmaB));
                pairwiseProbabilities[i] += probability;
            }
        }

        var rankedProbability = pairwiseProbabilities.Select(x => Math.Abs(x / denom)).ToList();
        var ranks = rankedProbability.Ranks(RankDefinition.Min);
        var maxOrdinal = ranks.Max();

        return ranks.Select(x => (int)Math.Abs(x - maxOrdinal) + 1).ToArray()
            .Zip(rankedProbability, (i, d) => (i, d))
            .ToList();
    }

    public IEnumerable<double> PredictWin(List<Team> teams)
    {
        var ratings = _utils.TeamRating(teams);
        var n = ratings.Count;
        var denom = n * ((double)n - 1) / 2d;
        var betasq = Options.BetaSq;

        return ratings.Select((a, i) =>
        {
            return ratings.Where((_, q) => i != q)
                .Select(b => Statistics.PhiMajor((a.Mu - b.Mu) /
                                                 Math.Sqrt(n * betasq + Math.Pow(a.SigmaSq, 2) +
                                                           Math.Pow(b.SigmaSq, 2))))
                .Aggregate(0d, (x, y) => x + y) / denom;
        });
    }
}