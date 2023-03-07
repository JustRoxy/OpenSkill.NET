using OpenSkill.NET.Types;

namespace OpenSkill.NET.Models;

public class PlackettLuce : OpenSkillModel
{
    public override List<Team> Rate(List<Team> teams, Options options)
    {
        var utils = new Utils(options);
        var epsilon = options.Epsilon;
        var ratings = utils.TeamRating(teams);
        var c = utils.UtilC(ratings);
        var sumQ = utils.UtilSumQ(ratings, c);
        var a = utils.UtilA(ratings);

        var result = new List<Team>();
        for (var i = 0; i < ratings.Count; i++)
        {
            var iRating = ratings[i];
            var omega = 0d;
            var delta = 0d;
            var iMuOverCe = Math.Exp(iRating.Mu / c);

            for (var q = 0; q < ratings.Count; q++)
            {
                var qRating = ratings[q];
                if (qRating.Rank > iRating.Rank) continue;

                var iMuCeOverSumQ = iMuOverCe / sumQ[q];
                delta += iMuCeOverSumQ * (1 - iMuCeOverSumQ) / a[q];
                if (q == i)
                {
                    omega += (1 - iMuCeOverSumQ) / a[q];
                }
                else
                {
                    omega -= iMuCeOverSumQ / a[q];
                }
            }

            omega *= iRating.SigmaSq / c;
            delta *= iRating.SigmaSq / Math.Pow(c, 2);

            var gamma = options.Gamma(new Gamma
            {
                C = c,
                K = ratings.Count,
                Mu = iRating.Mu,
                SigmaSq = iRating.SigmaSq,
                Team = iRating.Team,
                QRank = iRating.Rank
            });

            delta *= gamma;
            var intermediateResult = new List<Rating>();
            foreach (var jRating in iRating.Team.Ratings)
            {
                var mu = jRating.Mu;
                var sigma = jRating.Sigma;
                var lsigmaSq = sigma * sigma / iRating.SigmaSq;
                mu += lsigmaSq * omega;
                sigma *= Math.Sqrt(Math.Max(1 - lsigmaSq * delta, epsilon));
                intermediateResult.Add(new Rating(mu, sigma));
            }

            result.Add(new Team(intermediateResult));
        }


        return result;
    }
}