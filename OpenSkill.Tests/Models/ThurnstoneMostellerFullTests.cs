using OpenSkill.Models;
using OpenSkill.Types;

namespace OpenSkill.Tests.Models;

public class ThurnstoneMostellerFullTests
{
    private readonly Rating r = Rating.Default;
    private Team team1;
    private Team team2;
    private Team team3;

    private readonly OpenSkill _openSkill = new(new Options
    {
        Model = new ThurnstoneMostellerFull()
    });

    public ThurnstoneMostellerFullTests()
    {
        team1 = Team.With(r);
        team2 = Team.With(r, r);
        team3 = Team.With(r, r, r);
    }

    [Fact]
    public void TwoTeams()
    {
        var result = _openSkill.Rate(new List<Team>
        {
            team1,
            team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(29.205246334857588, 7.632833420130952) },
            new[] { new Rating(20.794753665142412, 7.632833420130952) }
        });
    }

    [Fact]
    public void ThreeTeams()
    {
        var result = _openSkill.Rate(new List<Team>()
        {
            team1, team1, team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(33.410492669715175, 6.861184124806115) },
            new[] { new Rating(25.0, 6.861184124806115) },
            new[] { new Rating(16.589507330284825, 6.861184124806115) }
        });
    }

    [Fact]
    public void FourTeams()
    {
        var result = _openSkill.Rate(new List<Team>()
        {
            team1, team1, team1, team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(37.61573900457276, 5.990955614049813) },
            new[] { new Rating(29.205246334857588, 5.990955614049813) },
            new[] { new Rating(20.794753665142412, 5.990955614049813) },
            new[] { new Rating(12.384260995427237, 5.990955614049813) }
        });
    }

    [Fact]
    public void ThreeAsymmetricTeams()
    {
        var result = _openSkill.Rate(new List<Team>()
        {
            team3,
            team1,
            team2
        });

        result.Equivalent(new[]
        {
            new[]
            {
                new Rating(25.72407717049428, 8.154234193613432),
                new Rating(25.72407717049428, 8.154234193613432),
                new Rating(25.72407717049428, 8.154234193613432)
            },
            new[]
            {
                new Rating(34.00108396884494, 7.757937033019593)
            },
            new[]
            {
                new Rating(15.274838860660779, 7.3733815675445085),
                new Rating(15.274838860660779, 7.3733815675445085)
            }
        });
    }

    [Fact]
    public void CustomGammaK2()
    {
        _openSkill.Options.Gamma = gamma => 1 / gamma.K;
        var result = _openSkill.Rate(new List<Team>()
        {
            team1,
            team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(29.205246334857588, 7.784759481252749) },
            new[] { new Rating(20.794753665142412, 7.784759481252749) }
        });
    }

    [Fact]
    public void CustomGammaK5()
    {
        _openSkill.Options.Gamma = gamma => 1 / gamma.K;
        var result = _openSkill.Rate(new List<Team>()
        {
            team1,
            team1,
            team1,
            team1,
            team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(41.82098533943035, 7.436215544405679) },
            new[] { new Rating(33.410492669715175, 7.436215544405679) },
            new[] { new Rating(25.0, 7.436215544405679) },
            new[] { new Rating(16.589507330284828, 7.436215544405679) },
            new[] { new Rating(8.179014660569653, 7.436215544405679) }
        });
    }

    [Fact]
    public void TiersInRank()
    {
        _openSkill.Options.Rank = new List<double> { 1, 2, 2, 4, 5 };
        var result = _openSkill.Rate(new List<Team>()
        {
            team1,
            team1,
            team1,
            team1,
            team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(41.82098533943035, 4.970638866839803) },
            new[] { new Rating(29.20528633485759, 4.280577057550792) },
            new[] { new Rating(29.20528633485759, 4.280577057550792) },
            new[] { new Rating(16.589507330284825, 4.970638866839803) },
            new[] { new Rating(8.17901466056965, 4.970638866839803) }
        });
    }
}