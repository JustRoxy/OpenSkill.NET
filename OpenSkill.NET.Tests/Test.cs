using FluentAssertions;
using OpenSkill.NET.Types;

namespace OpenSkill.NET.Tests;

public static class Test
{
    public static void Equivalent(this List<Team> teams, Rating[][] ratings)
    {
        teams.Should().HaveCount(ratings.Length);
        for (int i = 0; i < teams.Count; i++)
        {
            var team = teams[i];
            var rating = ratings[i];

            team.Ratings.Should().HaveCount(rating.Length);
            for (int j = 0; j < rating.Length; j++)
            {
                var tr = team.Ratings[j];
                var r = rating[j];
                tr.Mu.Should().BeApproximately(r.Mu, 7);
                tr.Sigma.Should().BeApproximately(r.Sigma, 7);
            }
        }
    }
}