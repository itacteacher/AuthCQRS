namespace AuthCQRS.Domain.Entities;
public class Ticket
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool isActive { get; set; }
}
