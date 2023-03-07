using OpenSkill.Models;
using OpenSkill.Types;

namespace OpenSkill.Tests.Models;

public class BradleyTerryFullTests
{
    private Rating r;
    private Team team1;
    private Team team2;
    private Team team3;
    private OpenSkill _openSkill;

    public BradleyTerryFullTests()
    {
        r = Rating.Default;
        team1 = Team.With(r);
        team2 = Team.With(r, r);
        team3 = Team.With(r, r, r);

        _openSkill = new OpenSkill(new Options
        {
            Model = new BradleyTerryFull()
        });
    }

    [Fact]
    public void TwoTeams()
    {
        var result = _openSkill.Rate(new List<Team>()
        {
            team1,
            team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(27.63523138347365, 8.065506316323548) },
            new[] { new Rating(22.36476861652635, 8.065506316323548) }
        });
    }

    [Fact]
    public void ThreeTeams()
    {
        var result = _openSkill.Rate(new List<Team>()
        {
            team1,
            team1,
            team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(30.2704627669473, 7.788474807872566) },
            new[] { new Rating(25.0, 7.788474807872566) },
            new[] { new Rating(19.7295372330527, 7.788474807872566) }
        });
    }

    [Fact]
    public void FourTeams()
    {
        var result = _openSkill.Rate(new List<Team>()
        {
            team1,
            team1,
            team1,
            team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(32.90569415042095, 7.5012190693964005) },
            new[] { new Rating(27.63523138347365, 7.5012190693964005) },
            new[] { new Rating(22.36476861652635, 7.5012190693964005) },
            new[] { new Rating(17.09430584957905, 7.5012190693964005) }
        });
    }

    [Fact]
    public void FiveTeams()
    {
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
            new[] { new Rating(35.5409255338946, 7.202515895247076) },
            new[] { new Rating(30.2704627669473, 7.202515895247076) },
            new[] { new Rating(25.0, 7.202515895247076) },
            new[] { new Rating(19.729537233052703, 7.202515895247076) },
            new[] { new Rating(14.4590744661054, 7.202515895247076) }
        });
    }

    [Fact]
    public void ThreeAsymmetricTeams()
    {
        var result = _openSkill.Rate(new List<Team>
        {
            team3,
            team1,
            team2
        });

        result.Equivalent(new[]
        {
            new[]
            {
                new Rating(25.992743915179297, 8.19709997489984),
                new Rating(25.992743915179297, 8.19709997489984),
                new Rating(25.992743915179297, 8.19709997489984)
            },
            new[]
            {
                new Rating(28.48909130001799, 8.220848339985736)
            },
            new[]
            {
                new Rating(20.518164784802718, 8.127515465304823),
                new Rating(20.518164784802718, 8.127515465304823)
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
            new[] { new Rating(27.63523138347365, 8.122328620674137) },
            new[] { new Rating(22.36476861652635, 8.122328620674137) }
        });
    }

    [Fact]
    public void CustomGammaK5()
    {
        _openSkill.Options.Gamma = gamma => 1 / gamma.K;
        var result = _openSkill.Rate(new List<Team>
        {
            team1,
            team1,
            team1,
            team1,
            team1
        });

        result.Equivalent(new[]
        {
            new[] { new Rating(35.5409255338946, 7.993052538854532) },
            new[] { new Rating(30.2704627669473, 7.993052538854532) },
            new[] { new Rating(25.0, 7.993052538854532) },
            new[] { new Rating(19.729537233052703, 7.993052538854532) },
            new[] { new Rating(14.4590744661054, 7.993052538854532) }
        });
    }
}