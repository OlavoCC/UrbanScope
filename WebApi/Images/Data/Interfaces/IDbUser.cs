namespace Api.Images.Data.Interfaces;

using Api.Images.DTOs.Return;
using Api.Images.DTOs.Intra;
public interface IDbUser
{
        public Task<bool> CreateUserAsync(CreateUserDTO dto);
        public Task<bool> VerifyUserExistenceAsync(string phone);
        public Task<ReturnUserInfoDTO> GetInfoUserAsync(string phone);
}

