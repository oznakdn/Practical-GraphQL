using GraphQL.API.Data;
using GraphQL.API.Schemas;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
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
    }
}