namespace OpenSkill.NET.Types;

public class Rating
{
    public Rating(double mu, double sigma)
    {
        Mu = mu;
        Sigma = sigma;
    }

    public double Mu { get; set; }
    public double Sigma { get; set; }

    public static Rating Default => new Rating(Constants.MU, Constants.SIGMA);
}