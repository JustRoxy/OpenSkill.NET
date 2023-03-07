using OpenSkill.NET.Types;

namespace OpenSkill.NET.Tests;

public class PlackettLuceTests
{
    private readonly Team team1;
    private readonly Team team2;
    private readonly Team team3;
    private readonly Options _options = new Options();
    private readonly OpenSkill _openSkill;

    public PlackettLuceTests()
    {
        var r = Rating.Default;
        team1 = new Team(new List<Rating> { r });
        team2 = new Team(new List<Rating> { r, r });
        team3 = new Team(new List<Rating> { r, r, r });
        _openSkill = new OpenSkill(_options);
    }

    [Fact]
    public void Team1_Team1()
    {
        var result = _openSkill.Rate(new List<Team> { team1, team1 });
        result.Equivalent(new[]
        {
            new[] { new Rating(27.63523138347365d, 8.065506316323548d) },
            new[] { new Rating(22.36476861652635d, 8.065506316323548d) }
        });
    }

    [Fact]
    void Team1_Team1_Team1()
    {
        var result = _openSkill.Rate(new List<Team> { team1, team1, team1 });
        result.Equivalent(new[]
        {
            new[] { new Rating(27.868876552746237, 8.204837030780652) },
            new[] { new Rating(25.717219138186557, 8.057829747583874) },
            new[] { new Rating(21.413904309067206, 8.057829747583874) }
        });
    }

    [Fact]
    void Team1_Team1_Team1_Team1()
    {
        var result = _openSkill.Rate(new List<Team> { team1, team1, team1, team1 });
        result.Equivalent(new[]
        {
            new[] { new Rating(27.795084971874736, 8.263160757613477) },
            new[] { new Rating(26.552824984374855, 8.179213704945203) },
            new[] { new Rating(24.68943500312503, 8.083731307186588) },
            new[] { new Rating(20.96265504062538, 8.083731307186588) }
        });
    }

    [Fact]
    void Team1_Team1_Team1_Team1_Team1()
    {
        var result = _openSkill.Rate(new List<Team> { team1, team1, team1, team1, team1 });
        result.Equivalent(new[]
        {
            new[] { new Rating(27.666666666666668, 8.290556877154474) },
            new[] { new Rating(26.833333333333332, 8.240145629781066) },
            new[] { new Rating(25.72222222222222, 8.179996679645559) },
            new[] { new Rating(24.055555555555557, 8.111796013701358) },
            new[] { new Rating(20.72222222222222, 8.111796013701358) }
        });
    }

    [Fact]
    void Team3_Team1_Team2()
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
                new Rating(25.939870821784513, 8.247641552260456),
                new Rating(25.939870821784513, 8.247641552260456),
                new Rating(25.939870821784513, 8.247641552260456)
            },
            new[]
            {
                new Rating(27.21366020491262, 8.274321317985242)
            },
            new[]
            {
                new Rating(21.84646897330287, 8.213058173195341),
                new Rating(21.84646897330287, 8.213058173195341)
            }
        });
    }
}