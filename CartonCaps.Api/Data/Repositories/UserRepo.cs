using CartonCaps.Api.Data.Entities;

namespace CartonCaps.Api.Data.Repositories;

public interface IUserRepo
{
    Task<UserEntity> GetUserAsync(int userId);
}

public class UserRepo : IUserRepo
{
    private IDataSource _dataSource;

    public UserRepo(IDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<UserEntity> GetUserAsync(int userId)
    {
        var users = await _dataSource.GetUsersDataAsync();
        return users.Where(_ => _.UserId == userId).SingleOrDefault();

    }
}
