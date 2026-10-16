namespace Images.Data.Services;

using Api.Images.Data.Interfaces;
using Api.Images.DTOs.Intra;
using Api.Images.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Api.Images.Models;
using BCrypt.Net;
using Api.Images.DTOs.Return;

public class DbUserService : IDbUser
{
    private readonly UrbanScopeContext _context;

    public DbUserService(UrbanScopeContext context)
    {
        _context = context;
    }

    public async Task<Guid?> SearchGuidFromRole(string role)
    {
        // Cast para Guid? porque FirstOrDefaultAsync retorna Guid.Empty
        // (e não null) quando nao encontra nenhuma linha.
        return await _context.Roles
            .Where(u => u.role == role)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CreateUserAsync(CreateUserDTO dto)
    {
        string role;
        if(dto.cpf == null)
        {
            var Complainant = new UserModel
            {
              phone = dto.phone,
            };
            _context.Users.Add(Complainant);
            

            role = "CIDADÃO";
            Guid? roleGuidC = await SearchGuidFromRole(role);
            if (roleGuidC is null)
            {
                return false;
            }

            var userRoleC = new UserRoleModel
            {
                id_user = Complainant.Id,
                id_role = roleGuidC.Value
            };
            _context.UserRoles.Add(userRoleC);

            await _context.SaveChangesAsync();
            
            return true;
        }

        string HashPassword = BCrypt.HashPassword(dto.password);

        var User = new UserModel
        {
            name = dto.name,
            last_name = dto.last_name,
            phone = dto.phone,
            email = dto.email,
            cpf = dto.cpf,
            password = HashPassword,
        };
        _context.Users.Add(User);

        role = "ADMIN";
        Guid? roleGuidA = await SearchGuidFromRole(role);
        if (roleGuidA is null)
        {
            return false;
        }
        var userRoleA = new UserRoleModel
        {
            id_user = User.Id,
            id_role = roleGuidA.Value
        };
        _context.UserRoles.Add(userRoleA);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> VerifyUserExistenceAsync(string phone)
    {
        bool existe = await _context.Users
            .AnyAsync(u => u.phone == phone);
        if(existe){
            return true;
        }
        return false;
    }

    public async Task<ReturnUserInfoDTO> GetInfoUserAsync(string phone)
    {
        bool existe = await VerifyUserExistenceAsync(phone);
        if(!existe){
            return null;
        }
        var user = await _context.Users
            .Where(u => u.phone == phone)
            .Select(u => new ReturnUserInfoDTO
            {
                id = u.Id,
                name = u.name,
                last_name = u.last_name,
                phone = u.phone,
                email = u.email,
                cpf = u.cpf,
                role = (from ur in _context.UserRoles
                        join r in _context.Roles on ur.id_role equals r.Id
                        where ur.id_user == u.Id
                        select r.role)
                        .FirstOrDefault()
            }).FirstOrDefaultAsync();
        return user;
    }
}

