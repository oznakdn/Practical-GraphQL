using GraphQL.API.Data;
using GraphQL.API.Helper;
using GraphQL.API.Schemas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<TokenHelper>();

        builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("ApiDb"));


        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidIssuer = "http://localhost:5054",
                                ValidAudience = "http://localhost:5054",
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c082a53a-938b-4504-8cff-def4667e8854"))
                            };
                        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("HasRole", policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == ClaimTypes.Role)));
        });

        builder.Services.AddGraphQLServer()
                        .AddAuthorization()
                        .AddProjections()
                        .AddFiltering()
                        .AddSorting()
                        .AddQueryType<Query>()
                        .AddMutationType<Mutation>()
                        .AddSubscriptionType<Subscription>();


        var app = builder.Build();

        app.SeedData();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseWebSockets();

        app.MapGraphQL();

        app.Run();
    }
}