namespace Api.Endpoints;

using Api.Data.DbContext;
using Api.Users.Data.Interfaces;
using Api.Users.Interfaces;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/users")
            .WithTags("Users");

        users.MapGet("/{phone}/info", async (string phone, IUser _user) =>
        {
            var result = await _user.GetUserInfoAsync(phone);
            if (result != null)
            {
                return Results.Ok(result);
            }
            return Results.NotFound();
        });
    }
}
