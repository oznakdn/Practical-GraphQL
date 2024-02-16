using GraphQL.API.Data;
using GraphQL.API.Models;

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
}
