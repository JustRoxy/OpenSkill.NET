namespace OpenSkill.Types;

public class Rating
{
    public Rating(double mu, double sigma, object? reference = null)
    {
        Mu = mu;
        Sigma = sigma;
        Reference = reference;
    }

    public double Mu { get; set; }
    public double Sigma { get; set; }

    public object? Reference { get; set; }

    public static Rating Default => new Rating(Constants.MU, Constants.SIGMA);
}