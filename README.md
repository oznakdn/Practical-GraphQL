
<img src="https://api.nuget.org/v3-flatcontainer/hotchocolate/13.8.1/icon" height=200 width=200>
<a href="https://chillicream.com/docs/hotchocolate/v13">Hot Chocolate documentation</a>

# Nuget Package

```csharp
<PackageReference Include="HotChocolate.AspNetCore" Version="13.8.0" />
<PackageReference Include="HotChocolate.Data.EntityFramework" Version="13.8.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="7.0.0" />
```

# Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("ApiDb"));

        builder.Services.AddGraphQLServer()
                        .AddProjections()
                        .AddFiltering()
                        .AddSorting()
                        .AddQueryType<Query>()
                        .AddMutationType<Mutation>()
                        .AddSubscriptionType<Subscription>();


        var app = builder.Build();

        app.SeedData();

        app.UseWebSockets();

        app.MapGraphQL();

        app.Run();
```

# Models
```csharp
public class Project
{
    public string Id { get; set; }
    public string Name { get; set; }

    public ICollection<Member> Members { get; set; } = new HashSet<Member>();
}
```

```csharp
public class Member
{
    
    public string Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Title { get; set; }

    public string ProjectId { get; set; }
    public Project? Project { get; set; }
}
```

# Seeding Data by Bogus

```csharp
public static class DataSeeding
{
    public static void SeedData(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();


        string projectId = Guid.NewGuid().ToString();
        var fakerProject = new Faker<Project>()
        .RuleFor(x => x.Name, f => f.Company.Bs())
        .RuleFor(x => x.Id, f => projectId);

        Project project = fakerProject.Generate();
        db.Projects.Add(project);



        var fakerMember = new Faker<Member>()
            .RuleFor(x => x.ProjectId, f => projectId)
            .RuleFor(x => x.Name, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Title, f => f.Name.JobTitle())
            .RuleFor(x => x.Id, f => Guid.NewGuid().ToString());

        List<Member> members = fakerMember.Generate(20);
        db.Members.AddRange(members);

        db.SaveChanges();

    }
}
```

# Schemas

## Query.cs

```csharp
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

```

## Mutation.cs
```csharp
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

    // This method is for the Subscription's OnProjectCreated event
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
```

## Subscription.cs

```csharp
public class Subscription
{
    [Subscribe]
    [Topic]
    public Project OnProjectCreated([EventMessage] Project project)
    {
        return project;
    }
}
```

# Requests and Responses
Attention! All the requests must to call by the Http POST method for usage the GraphQL

## Project requests and responses
![Screenshot_1](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/704f5b33-22f1-4515-bf6e-1f5e5bca2328)
![Screenshot_2](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/e4c611d2-8be4-4d02-a77f-49445e124f67)
![Screenshot_3](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/c3fc26bc-ff52-4138-9d86-f0fbecc2ce0c)
![Screenshot_4](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/499cf1de-b83e-4fa0-ad5a-80083985bc16)
![Screenshot_5](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/187cf396-2908-4218-82e4-8c525ba945a4)
![Screenshot_6](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/614c2be2-e68c-4ce2-b701-f5e3443b3d01)

## Member requests and responses
![Screenshot_7](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/efd17bab-f3bb-4946-ae1d-c872d8c7272c)
![Screenshot_8](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/d06300d0-7ac7-4fe4-a207-ddf3e7ea4f72)
![Screenshot_9](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/b85f5db6-3f9e-4391-94c4-5abc61f28edd)
![Screenshot_10](https://github.com/oznakdn/Practical-GraphQL/assets/79724084/d3525a31-456a-48e2-8e04-41499f74e232)


