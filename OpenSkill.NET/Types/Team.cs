namespace OpenSkill.NET.Types;

public class Team
{
    public Team(List<Rating> ratings)
    {
        Ratings = ratings;
    }

    public static Team With(params Rating[] ratings)
    {
        return new Team(ratings.ToList());
    }

    public List<Rating> Ratings { get; }
}