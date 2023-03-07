using OpenSkill.Types;

namespace OpenSkill.Tests;

public class RateTests
{
    [Fact]
    public void AcceptsTauTerm()
    {
        var a = new Rating(Constants.MU, 3);
        var b = new Rating(Constants.MU, 3);
        var options = new Options
        {
            Tau = 0.3
        };

        var openSkill = new OpenSkill(options);
        var result = openSkill.Rate(new List<Team>
        {
            new(new List<Rating> { a }),
            new(new List<Rating> { b })
        });

        result.Equivalent(new[]
        {
            new[]
            {
                new Rating(25.624880438870754, 2.9879993738476953)
            },
            new[]
            {
                new Rating(24.375119561129246, 2.9879993738476953)
            }
        });
    }

    [Fact]
    public void PreventsSigmaFromRising()
    {
        var a = new Rating(40, 3);
        var b = new Rating(-20, 3);
        var options = new Options
        {
            Tau = 0.3d,
            PreventSigmaIncrease = true
        };

        var openSkill = new OpenSkill(options);
        var result = openSkill.Rate(new List<Team>
        {
            new(new List<Rating> { a }),
            new(new List<Rating> { b })
        });

        result.Equivalent(new[]
        {
            new[]
            {
                new Rating(40.00032667136128, 3)
            },
            new[]
            {
                new Rating(-20.000326671361275, 3)
            }
        });
    }

    [Fact]
    public void EnsuresSigmaIncreases()
    {
        var a = Rating.Default;
        var b = Rating.Default;
        var opts = new Options
        {
            Tau = 0.3,
            PreventSigmaIncrease = true
        };
        var openskill = new OpenSkill(opts);

        var result = openskill.Rate(new List<Team>
        {
            new(new List<Rating> { a }),
            new(new List<Rating> { b })
        });
        result.Equivalent(new[]
        {
            new[] { new Rating(27.6372798316677, 8.070625245679999) },
            new[] { new Rating(22.3627201683323, 8.070625245679999) }
        });
    }
}