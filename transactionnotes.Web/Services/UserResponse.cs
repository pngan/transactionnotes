namespace transactionnotes.Web.Services;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty;
}