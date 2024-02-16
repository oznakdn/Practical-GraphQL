namespace GraphQL.API.Models;

public class Member
{
    
    public string Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Title { get; set; }

    public string ProjectId { get; set; }
    public Project? Project { get; set; }

}
