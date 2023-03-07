using FluentAssertions;
using OpenSkill.Types;

namespace OpenSkill.Tests;

public class UnwindTests
{
    private readonly Utils _utils = new(new Options());

    [Fact]
    public void ZeroItems()
    {
        var (teams, tenet) =
            _utils.Unwind(new List<double>(), new List<Team>());
        teams.Should().HaveCount(0);
        tenet.Should().HaveCount(0);
    }

    [Fact]
    public void OneItem()
    {
        var (sorted, tenet) = _utils.Unwind(new List<double> { 0 }, new List<string> { "a" });
        sorted.Should().BeEquivalentTo("a");
        tenet.Should().BeEquivalentTo(new[] { 0 });
    }

    [Fact]
    public void TwoItems()
    {
        var (sorted, tenet) = _utils.Unwind(new List<double> { 1, 0 }, new List<string> { "b", "a" });
        sorted.Should().Equal("a", "b");
        tenet.Should().Equal(1d, 0);
    }

    [Fact]
    public void ThreeItems()
    {
        var src = new List<string> { "b", "c", "a" };
        var rank = new List<double> { 1, 2, 0 };

        var (dst, derank) = _utils.Unwind(rank, src);
        dst.Should().Equal("a", "b", "c");
        derank.Should().Equal(2, 0, 1);
    }

    [Fact]
    public void FourItems()
    {
        var src = new List<string> { "b", "d", "c", "a" };
        var rank = new List<double> { 1, 3, 2, 0 };

        var (dst, derank) = _utils.Unwind(rank, src);
        dst.Should().Equal("a", "b", "c", "d");
        derank.Should().Equal(3, 0, 2, 1);
    }

    [Fact]
    public void Shuffle()
    {
        var src = Enumerable.Range(0, 100).Select(_ => Random.Shared.Next()).ToList();
        var rank = Enumerable.Range(0, 100).Select(x => (double)x).ToList();

        var (r, t) = _utils.Unwind(rank, src);
        var (r1, t1) = _utils.Unwind(t, r);

        r1.Should().Equal(src);
        t1.Should().Equal(rank);
    }

    [Fact]
    public void NonIntegerRanks()
    {
        var src = new List<string> { "a", "b", "c", "d", "e", "f" };
        var rank = new List<double> { 0.28591, 0.42682, 0.35912, 0.21237, 0.60619, 0.47078 };

        var (r, t) = _utils.Unwind(rank, src);
        r.Should().Equal("d", "a", "c", "b", "f", "e");

        var (r1, t1) = _utils.Unwind(t, r);
        r1.Should().Equal(src);
    }
}