using FluentAssertions;
using OpenSkill.NET.Types;

namespace OpenSkill.NET.Tests;

public class PredictWinTests
{
    private readonly OpenSkill _openSkill = new(new Options());

    private Rating a1 = Rating.Default;
    private Rating a2 = new(32.444, 5.123);

    private Rating b1 = new(73.381, 1.421);
    private Rating b2 = new(25.188, 6.211);

    [Fact]
    public void TwoTeams()
    {
        var result = _openSkill.PredictWin(new List<Team>()
        {
            new(new List<Rating> { a1, a2 }),
            new(new List<Rating> { b1, b2 })
        });

        result.Should().Equal(0.34641823958165474, 0.6535817604183453);
    }

    [Fact]
    public void IgnoresRankings()
    {
        _openSkill.Options.Rank = new List<double>() { 3, 2, 1 };
        var p1 = _openSkill.PredictWin(new List<Team>
        {
            Team.With(a2),
            Team.With(b1),
            Team.With(b2)
        });

        _openSkill.Options.Rank = new List<double> { 3, 2, 1 };
        var p2 = _openSkill.PredictWin(new List<Team>
        {
            Team.With(a2),
            Team.With(b1),
            Team.With(b2)
        });

        p2.Should().Equal(p1);
    }

    [Fact]
    public void MultipleAsymmetricTeams()
    {
        var result = _openSkill.PredictWin(
            new List<Team>
            {
                Team.With(a1, a2),
                Team.With(b1, b2),
                Team.With(a2),
                Team.With(b2)
            }).ToList();

        result.Should().HaveCount(4);
        result[0].Should().BeApproximately(0.2613515941642222, 7);
        result[1].Should().BeApproximately(0.41117430943389155, 7);
        result[2].Should().BeApproximately(0.1750905983112395, 7);
        result[3].Should().BeApproximately(0.15238349809064686, 7);
        
    }
}