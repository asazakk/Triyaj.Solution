namespace Triyaj.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NationalId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
