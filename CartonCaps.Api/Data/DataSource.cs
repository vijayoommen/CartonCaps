using CartonCaps.Api.Data.Entities;
using System.Text.Json;

namespace CartonCaps.Api.Data;

public interface IDataSource
{
    Task<List<UserReferralEntity>> GetReferralDataAsync();
    Task SaveReferralDataAsync(List<UserReferralEntity> data);
    Task<List<UserEntity>> GetUsersDataAsync();
}

public class DataSource : IDataSource
{
    private const string UserDataFile = @"data\users.json";
    private const string ReferralDataFile = @"data\referral.json";

    public async Task<List<UserReferralEntity>> GetReferralDataAsync()
    {
        var data = await File.ReadAllTextAsync(ReferralDataFile);
        return JsonSerializer.Deserialize<List<UserReferralEntity>>(data, GetSerializationOptions) ?? new List<UserReferralEntity>();
    }

    public async Task SaveReferralDataAsync(List<UserReferralEntity> data)
    {
        var json = JsonSerializer.Serialize(data, GetSerializationOptions);
        await File.WriteAllTextAsync(ReferralDataFile, json);
    }

    public async Task<List<UserEntity>> GetUsersDataAsync()
    {
        var data = await File.ReadAllTextAsync(UserDataFile);
        return JsonSerializer.Deserialize<List<UserEntity>>(data, GetSerializationOptions) ?? new List<UserEntity>();
    }

    private JsonSerializerOptions GetSerializationOptions => new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
