namespace CartonCaps.Api.Data.Entities;

public class UserReferralEntity
{
    public int UserId { get; set; }
    public string ReferralCode { get; set; }
    public List<Referred> Referred { get; set; }
}

public class Referred
{
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Name { get; set; }
    public bool Enrolled { get; set; } = false;
    public string Token { get; set; }
}