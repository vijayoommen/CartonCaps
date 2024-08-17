namespace CartonCaps.Contracts;

public class ReferralStatus
{
    public string ReferralCode { get; set; }

    public List<Referrals> Referrals { get; set; }
}

public class Referrals
{
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Name { get; set; }
    public bool Enrolled { get;set; }
}
