using OpenSkill.NET.Types;

namespace OpenSkill.NET;

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
                        .Select(v => new Rating(v.Mu, Math.Sqrt(v.Sigma * v.Sigma + tauSquared)))
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