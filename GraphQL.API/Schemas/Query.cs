using GraphQL.API.Data;
using GraphQL.API.Dtos;
using GraphQL.API.Helper;
using GraphQL.API.Models;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.API.Schemas;

public class Query
{

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Project>> GetProjectAsync([Service] AppDbContext context)
    {
        return await Task.FromResult(context.Projects);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Member>> GetMemberAsync([Service] AppDbContext context)
    {
        return await Task.FromResult(context.Members);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<User>> GetUserAsync([Service] AppDbContext context)
    {
        return await Task.FromResult(context.Users);
    }


    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize("HasRole", Roles = ["MANAGER"])]
    public async Task<IQueryable<Role>> GetRoleAsync([Service] AppDbContext context)
    {
        return await Task.FromResult(context.Roles);
    }


    public async Task<LoginResponse> LoginAsync(LoginInput input, [Service] AppDbContext context, [Service] TokenHelper tokenHelper)
    {
        var user = await context.Users.Include(x=>x.Role).SingleOrDefaultAsync(x => x.Username == input.Username && x.Password == input.Password);

        if (user is null)
        {
            throw new GraphQLException("Username or password is wrong!");
        }

        var token = tokenHelper.GenerateToken(user);

        return new LoginResponse(token.Token, token.TokenExpire, user);
    }
}
