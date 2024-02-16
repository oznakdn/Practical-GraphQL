using Bogus;
using GraphQL.API.Models;

namespace GraphQL.API.Data;

public static class DataSeeding
{
    public static void SeedData(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();


        // Seeding project
        string projectId = Guid.NewGuid().ToString();
        var fakerProject = new Faker<Project>()
        .RuleFor(x => x.Name, f => f.Company.Bs())
        .RuleFor(x => x.Id, f => projectId);

        Project project = fakerProject.Generate();
        db.Projects.Add(project);


        // Seeding member
        var fakerMember = new Faker<Member>()
            .RuleFor(x => x.ProjectId, f => projectId)
            .RuleFor(x => x.Name, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Title, f => f.Name.JobTitle())
            .RuleFor(x => x.Id, f => Guid.NewGuid().ToString());


        List<Member> members = fakerMember.Generate(20);
        db.Members.AddRange(members);


        // Seeding role
        string adminId = Guid.NewGuid().ToString();
        var fakeRoleAdmin = new Faker<Role>()
            .RuleFor(x => x.Name, f => "ADMIN")
            .RuleFor(x => x.Id, f => adminId);

        string managerId = Guid.NewGuid().ToString();
        var fakeRoleManager = new Faker<Role>()
            .RuleFor(x => x.Name, f => "MANAGER")
            .RuleFor(x => x.Id, f => managerId);


        var admin = fakeRoleAdmin.Generate();
        var manager = fakeRoleManager.Generate();
        db.Roles.AddRange(admin, manager);

        // Seeding User
        var fakeManager = new Faker<User>()
         .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
          .RuleFor(x => x.Email, f => f.Person.Email)
          .RuleFor(x => x.Password, f => "123456")
          .RuleFor(x => x.Username, f => f.Person.UserName)
          .RuleFor(x => x.RoleId, f => managerId);

        var user = fakeManager.Generate(1);
        db.Users.AddRange(user);

        var fakeUser = new Faker<User>()
            .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Password, f => "123456")
            .RuleFor(x => x.Username, f => f.Person.UserName)
            .RuleFor(x => x.RoleId, f => adminId);

        List<User> users = fakeUser.Generate(5);

        db.Users.AddRange(users);



        db.SaveChanges();

    }
}
