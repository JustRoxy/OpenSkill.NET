namespace OpenSkill.NET.Types;

public class TeamRating
{
    public double Mu { get; set; }
    public double SigmaSq { get; set; }
    public Team Team { get; set; }
    public double Rank { get; set; }
}