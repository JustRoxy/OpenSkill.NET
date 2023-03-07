namespace OpenSkill.NET.Types;

public class Team
{
    public Team(List<Rating> ratings)
    {
        Ratings = ratings;
    }

    public List<Rating> Ratings { get; }
}