namespace OpenSkill.Types;

public class Gamma
{
    public double C { get; set; }
    public double K { get; set; }
    public double Mu { get; set; }
    public double SigmaSq { get; set; }
    public Team Team { get; set; } = null!;
    public double QRank { get; set; }
}