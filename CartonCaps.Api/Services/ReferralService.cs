using CartonCaps.Api.Data.Entities;
using CartonCaps.Api.Data.Repositories;
using CartonCaps.Contracts;

namespace CartonCaps.Api.Services;

public interface IReferralService
{
    Task<string> GetReferralCodeAsync(int userId);
    Task<List<ReferralResponse>> GenerateReferralTokensAsync(int userId, List<ReferralRequest> requests);
    Task<ReferralStatus> GetReferralStatusAsync(int userId);
    Task<TokenInfoResponse?> RetrieveReferralTokenAsync(string token);
}

public class ReferralService : IReferralService
{
    private IReferralsRepo _referralRepo;

    public ReferralService(IReferralsRepo referralsRepo)
    {
        _referralRepo = referralsRepo;
    }

    public async Task<string> GetReferralCodeAsync(int userId)
    {
        return await _referralRepo.GetReferralCodeAsync(userId);
    }

    public async Task<List<ReferralResponse>> GenerateReferralTokensAsync(int userId, List<ReferralRequest> requests)
    {
        var user = await _referralRepo.GetReferralUserAsync(userId);

        if (user == null)
        {
            return new List<ReferralResponse>();
        }

        var response = new List<ReferralResponse>();
        requests.ForEach(_ =>
        {
            var referrence = user.Referred
                .Where(u => !string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Equals(_.Email) 
                            && !string.IsNullOrEmpty(u.Phone) && u.Phone.Equals(_.Phone))
                .FirstOrDefault();

            if (referrence == null)
            {
                referrence = new Referred
                {
                    Email = _.Email,
                    Phone = _.Phone,
                    Token = Guid.NewGuid().ToString(),
                    Name = _.Name,
                    Enrolled = false
                };

                user.Referred.Add(referrence);
            }

            response.Add(new ReferralResponse
            {
                Email = referrence.Email,
                Phone = referrence.Phone,
                Name = referrence.Name,
                ReferralToken = referrence.Token,
            });
        });

        await _referralRepo.UpdateUser(user);

        return response;

    }

    public async Task<ReferralStatus> GetReferralStatusAsync(int userId)
    {
        var user = await _referralRepo.GetReferralUserAsync(userId);

        if (user == null)
        {
            return new ReferralStatus();
        }

        return new ReferralStatus
        {
            ReferralCode = user.ReferralCode,
            Referrals = user.Referred.Select(_ => new Referrals
            {
                Email = _.Email,
                Phone = _.Phone,
                Name = _.Name,
                Enrolled = _.Enrolled,
            }).ToList()
        };

    }

    public async Task<TokenInfoResponse?> RetrieveReferralTokenAsync(string token)
    {
        var referralUser = await _referralRepo.GetUserByToken(token);
        var referral = referralUser?.Referred.FirstOrDefault(_ => _.Token == token) ?? null;

        if (referralUser == null || referral == null)
        {
            return null;
        }

        return new TokenInfoResponse
        {
            Email = referral.Email,
            Phone = referral.Phone,
            Name = referral.Name,
            Token = referral.Token,
            ReferralCode = referralUser.ReferralCode,
        };
    }
}
