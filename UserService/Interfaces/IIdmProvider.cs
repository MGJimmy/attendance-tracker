namespace Identity.Service;

public interface IIdmProvider<TUser> where TUser : IApplicationUser
{
    Task<LoginStatus> LoginUserAsync(string email, string password);
    Task<TUser> FindByNameAsync(string email);
    Task<List<string>> GetUserRoles(TUser user);
    Task<(bool Success, string ErrorMessage)> UpdateUserAsync(TUser user);
    Task<(bool Success, string ErrorMessage)> DeleteUserAsync(string userId);
    Task<(bool Success, string ErrorMessage)> CreateUserAsync(TUser user, string password);
    Task<(bool Success, string ErrorMessage)> AssignRolesToUser(TUser user, IEnumerable<string> roles);
    Task<TUser> FindByIdAsync(string userId);
    Task<(bool Success, string ErrorMessage)> ReplaceRolesAsync(TUser user, IEnumerable<string> newRoles);
}


