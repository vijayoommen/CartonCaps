namespace CartonCaps.Contracts;

public class ReferralStatus
{
    public string ReferralCode { get; set; } = string.Empty;

    public List<Referrals> Referrals { get; set; } = new List<Referrals>();
}

public class Referrals
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Name { get; set; }
    public bool Enrolled { get; set; } = false;
}
