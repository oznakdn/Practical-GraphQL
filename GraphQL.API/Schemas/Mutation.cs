using GraphQL.API.Data;
using GraphQL.API.Dtos;
using GraphQL.API.Models;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.API.Schemas;

public class Mutation
{
    public async Task<CreateProjectOutput> CreateProjectAsync(CreateProjectInput input, CancellationToken cancellationToken, [Service] AppDbContext context)
    {
        var project = new Project
        {
            Id = Guid.NewGuid().ToString(),
            Name = input.Name
        };

        context.Add(project);
        await context.SaveChangesAsync(cancellationToken);
        return new CreateProjectOutput(project);
    }

    public async Task<bool> UpdateProjectAsync(UpdateProjectInput input, CancellationToken cancellationToken, [Service] AppDbContext context)
    {
        var project = await context.Projects.SingleOrDefaultAsync(p => p.Id == input.Id);

        if (project is null)
            throw new GraphQLException("Project not found!");

        project.Name = input.Name ?? default!;

        context.Update(project);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> RemoveProjectAsync(string id, CancellationToken cancellationToken, [Service] AppDbContext context)
    {
        var project = await context.Projects.SingleOrDefaultAsync(p => p.Id == id);

        if (project is null)
            throw new GraphQLException("Project not found!");

        context.Remove(project);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<CreateProjectOutput> AddProjectWithEventAsync(CreateProjectInput input, [Service] AppDbContext context, [Service] ITopicEventSender eventSender, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Id = Guid.NewGuid().ToString(),
            Name = input.Name
        };

        context.Add(project);
        await context.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(nameof(Subscription.OnProjectCreated), project, cancellationToken);

        return new CreateProjectOutput(project);

    }
}
