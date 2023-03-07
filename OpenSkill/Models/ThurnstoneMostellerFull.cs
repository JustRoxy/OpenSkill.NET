using OpenSkill.Types;

namespace OpenSkill.Models;

public class ThurnstoneMostellerFull : OpenSkillModel
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
                var delta_mu = (iRating.Mu - qRating.Mu) / ciq;
                var sigmaSquaredToCiq = iRating.SigmaSq / ciq;
                var gamma = options.Gamma(new Gamma
                {
                    C = ciq,
                    K = teamRatings.Count,
                    Mu = iRating.Mu,
                    SigmaSq = iRating.SigmaSq,
                    Team = iRating.Team,
                    QRank = iRating.Rank
                });

                if (qRating.Rank > iRating.Rank)
                {
                    omega += sigmaSquaredToCiq * Statistics.V(delta_mu, epsilon / ciq);
                    delta += gamma * sigmaSquaredToCiq / ciq * Statistics.W(delta_mu, epsilon / ciq);
                }
                else if (qRating.Rank < iRating.Rank)
                {
                    omega += -sigmaSquaredToCiq * Statistics.V(-delta_mu, epsilon / ciq);
                    delta += gamma * sigmaSquaredToCiq / ciq * Statistics.W(-delta_mu, epsilon / ciq);
                }
                else
                {
                    omega += sigmaSquaredToCiq * Statistics.Vt(delta_mu, epsilon / ciq);
                    delta += gamma * sigmaSquaredToCiq / ciq * Statistics.Wt(delta_mu, epsilon / ciq);
                }
            }

            var intermediateResult = new List<Rating>();
            foreach (var jRating in iRating.Team.Ratings)
            {
                var mu = jRating.Mu;
                var sigma = jRating.Sigma;
                mu += Math.Pow(sigma, 2) / iRating.SigmaSq * omega;
                sigma *= Math.Sqrt(Math.Max(1 - Math.Pow(sigma, 2) / iRating.SigmaSq * delta, epsilon));
                intermediateResult.Add(new Rating(mu, sigma, jRating.Reference));
            }

            result.Add(new Team(intermediateResult));
        }

        return result;
    }
}