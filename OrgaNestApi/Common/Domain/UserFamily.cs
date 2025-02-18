namespace OrgaNestApi.Common.Domain;

public class UserFamily
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid FamilyId { get; set; }
    public Family Family { get; set; } = null!;
}