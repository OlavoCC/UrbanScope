namespace Api.Users.Services;

using Api.Users.Interfaces;
using Api.Data.DTOs.Return;
using Api.Users.Data.Interfaces;

public class UserService : IUser
{
    private readonly IDbUser _IUser;

    public UserService(IDbUser Iuser)
    {
        Iuser = _IUser;
    }
    
    public async Task<ReturnUserInfoDTO> GetUserInfoAsync(string phone){
        var result = await _IUser.GetInfoUserAsyncDb(phone);

        if (result != null)
        {
            return result;
        }

        return null;
    }
}