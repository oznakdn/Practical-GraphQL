using GraphQL.API.Models;

namespace GraphQL.API.Schemas;

public class Subscription
{
    [Subscribe]
    [Topic]
    public Project OnProjectCreated([EventMessage] Project project)
    {
        return project;
    }
}
