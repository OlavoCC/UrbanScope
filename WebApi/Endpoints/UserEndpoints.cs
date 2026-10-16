namespace Api.Images.Endpoints;
using Api.Images.Context;
using Api.Images.Data.Interfaces;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/users")
            .WithTags("Users");

        users.MapGet("/{phone}/info", async (string phone, IDbUser user) =>
        {
            var result = await user.GetInfoUserAsync(phone);

            if (result != null)
            {
                return Results.Ok(result);
            }

            return Results.NotFound();
        });
    }
}
