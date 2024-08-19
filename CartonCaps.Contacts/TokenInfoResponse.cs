namespace CartonCaps.Contracts;

public class TokenInfoResponse
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Name { get; set; }
    public string? Token { get; set; }
    public string ReferralCode { get; set; } = "";
}
