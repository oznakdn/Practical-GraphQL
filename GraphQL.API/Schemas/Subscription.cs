using GraphQL.API.Models;

namespace GraphQL.API.Schemas;

public class Subscription
{
    [Subscribe]
    [Topic]
    public Project OnPlatformAdded([EventMessage] Project project)
    {
        return project;
    }
}
