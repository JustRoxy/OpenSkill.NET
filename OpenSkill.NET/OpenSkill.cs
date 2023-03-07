using OpenSkill.NET.Types;

namespace OpenSkill.NET;

public class OpenSkill
{
    private readonly Options _options;
    private readonly Utils _utils;

    public OpenSkill(Options options)
    {
        _options = options;
        _utils = new Utils(options);
    }

    public List<Team> Rate(List<Team> teams)
    {
        var model = _options.Model;
        if (_options.Tau is not null)
        {
            var tauSquared = _options.Tau.Value * _options.Tau.Value;
            teams.ForEach(x => x.Ratings.ForEach(v => { v.Sigma = Math.Sqrt(v.Sigma * v.Sigma + tauSquared); }));
        }

        var result = model.Rate(teams, _options);
        return result;

        if (_options.Tau is not null && _options.PreventSigmaIncrease)
        {
            for (var i = 0; i < teams.Count; i++)
            {
                for (var j = 0; j < teams[i].Ratings.Count; j++)
                {
                }
            }
        }
    }

    public IEnumerable<double> PredictWin(List<Team> teams)
    {
        var ratings = _utils.TeamRating(teams);
        var n = ratings.Count;
        var denom = n * ((double)n - 1) / 2d;
        var betasq = _options.BetaSq;

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