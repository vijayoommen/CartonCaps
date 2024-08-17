using CartonCaps.Api.Data.Entities;
using System.Formats.Asn1;

namespace CartonCaps.Api.Data.Repositories;

public interface IReferralsRepo
{
    Task<string> GetReferralCodeAsync(int userId);
    Task<UserReferralEntity?> GetReferralUserAsync(int userId);
    Task<UserReferralEntity?> GetUserByToken(string token);
    Task UpdateUser(UserReferralEntity user);
}

public class ReferralsRepo : IReferralsRepo
{
    private IDataSource _dataSource;

    public ReferralsRepo(IDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<string> GetReferralCodeAsync(int userId)
    {
        var user = await GetUserAsync(userId);

        if (user != null && string.IsNullOrEmpty(user.ReferralCode))
        {
            var referalCode = DateTime.Now.ToString().GetHashCode().ToString("x").ToUpper();
            user.ReferralCode = referalCode;
            await SaveUserAsync(user);
        }

        return user?.ReferralCode ?? string.Empty;
    }

    public async Task<UserReferralEntity?> GetReferralUserAsync(int userId)
    {
        return await GetUserAsync(userId);
    }

    public async Task<UserReferralEntity?> GetUserByToken(string token)
    {
        var users = await _dataSource.GetReferralDataAsync();
        return users.Where(_ => _.Referred.Any(r => r.Token == token)).SingleOrDefault();
    }

    public async Task UpdateUser(UserReferralEntity user)
    {
        await SaveUserAsync(user);
    }

    private async Task<UserReferralEntity> GetUserAsync(int userId)
    {
        var data = await _dataSource.GetReferralDataAsync();
        return data.SingleOrDefault(_ => _.UserId == userId);
    }

    private async Task SaveUserAsync(UserReferralEntity user)
    {
        var data = await _dataSource.GetReferralDataAsync();
        var index = data.FindIndex(_ => _.UserId == user.UserId);
        if (index != -1)
        {
            data[index] = user;
        }
        else
        {
            data.Add(user);
        }

        await _dataSource.SaveReferralDataAsync(data);
    }
}
