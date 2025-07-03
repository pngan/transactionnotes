namespace transactionnotes.ApiService.ApiTypes.Response;

public class UserResponse(Guid id, string firstName, string lastName, string email)
{
    public Guid Id { get; set; } = id;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public string Email { get; set; } = email;
}
