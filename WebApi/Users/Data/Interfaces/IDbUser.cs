namespace Api.Users.Data.Interfaces;

using Api.Data.DTOs.Return;
using Api.Data.DTOs.Intra;

public interface IDbUser
{
        public Task<bool> CreateUserAsyncDb(CreateUserDTO dto);
        public Task<bool> VerifyUserExistenceAsyncDb(string phone);
        public Task<ReturnUserInfoDTO> GetInfoUserAsyncDb(string phone);
}

