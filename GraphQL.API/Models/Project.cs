namespace GraphQL.API.Models;

public class Project
{
  
    public string Id { get; set; }
    public string Name { get; set; }

    public ICollection<Member> Members { get; set; } = new HashSet<Member>();
}
