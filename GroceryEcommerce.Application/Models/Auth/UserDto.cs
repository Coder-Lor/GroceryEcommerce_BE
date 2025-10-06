namespace GroceryEcommerce.Application.Models;

public class UserDto
{
    public Guid UserId { get; set; }
    public required string Email { get; set; } 
    public required string Username { get; set; }
    public string? FullName { get; set; }
    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginAttempts { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<UserAddressDto> Addresses { get; set; } = new();
    public List<UserRoleDto> Roles { get; set; } = new();
}

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public short Gender { get; set; }
}

public class UpdateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public short Gender { get; set; }
}
