using OpenSkill.NET.Types;

namespace OpenSkill.NET.Models;

public class BradleyTerryFull : OpenSkillModel
{
    public override List<Team> Rate(List<Team> teams, Options options)
    {
        var utils = new Utils(options);
        var epsilon = options.Epsilon;
        var twoBetaSquared = options.TwoBetaSq;
        var teamRatings = utils.TeamRating(teams);

        var result = new List<Team>();
        for (int i = 0; i < teamRatings.Count; i++)
        {
            var iRating = teamRatings[i];
            var omega = 0d;
            var delta = 0d;

            for (int q = 0; q < teamRatings.Count; q++)
            {
                if (q == i) continue;

                var qRating = teamRatings[q];

                var ciq = Math.Sqrt(iRating.SigmaSq + qRating.SigmaSq + twoBetaSquared);
                var piq = 1d / (1 + Math.Exp((qRating.Mu - iRating.Mu) / ciq));
                var sigmaSquaredToCiq = iRating.SigmaSq / ciq;

                var s = 0d;
                if (qRating.Rank > iRating.Rank)
                {
                    s = 1;
                }
                else if (Math.Abs(qRating.Rank - iRating.Rank) < epsilon)
                {
                    s = 0.5;
                }

                omega += sigmaSquaredToCiq * (s - piq);
                var gamma = options.Gamma(new Gamma
                {
                    C = ciq,
                    K = teamRatings.Count,
                    Mu = iRating.Mu,
                    SigmaSq = iRating.SigmaSq,
                    Team = iRating.Team,
                    QRank = iRating.Rank
                });

                delta += gamma * sigmaSquaredToCiq / ciq * piq * (1 - piq);
            }

            var intermediateResults = new List<Rating>();
            foreach (var jRating in iRating.Team.Ratings)
            {
                var mu = jRating.Mu;
                var sigma = jRating.Sigma;
                mu += Math.Pow(sigma, 2) / iRating.SigmaSq * omega;
                sigma *= Math.Sqrt(Math.Max(1 - Math.Pow(sigma, 2) / iRating.SigmaSq * delta, epsilon));
                intermediateResults.Add(new Rating(mu, sigma));
            }

            result.Add(new Team(intermediateResults));
        }

        return result;
    }
}