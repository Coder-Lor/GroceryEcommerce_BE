using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Auth;

public interface IUserRepository
{
    Task<Result<User?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<User?>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<User?>> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Result<User?>> GetUserByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Result<bool>> AddAsync(User user, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(User user, CancellationToken cancellationToken = default);
    Task<Result<(bool emailExists, bool usernameExists)>> CheckUserExistenceAsync(string email, string username, CancellationToken cancellationToken = default);


}