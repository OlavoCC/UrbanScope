namespace Api.Users.Interfaces;

using Api.Data.DTOs.Return;

public interface IUser
{
    public Task<ReturnUserInfoDTO> GetUserInfoAsync(string phone);
}