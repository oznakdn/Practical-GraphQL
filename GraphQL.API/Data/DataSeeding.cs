using Bogus;
using GraphQL.API.Models;

namespace GraphQL.API.Data;

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
