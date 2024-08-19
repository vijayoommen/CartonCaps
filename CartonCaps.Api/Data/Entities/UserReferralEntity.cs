namespace CartonCaps.Api.Data.Entities;

public class UserReferralEntity
{
    public int UserId { get; set; }
    public string ReferralCode { get; set; } = string.Empty;
    public List<Referred> Referred { get; set; } = new List<Referred>();
}

public class Referred
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Name { get; set; }
    public bool Enrolled { get; set; } = false;
    public string Token { get; set; } = string.Empty;
}