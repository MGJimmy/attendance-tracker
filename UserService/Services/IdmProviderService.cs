using Microsoft.AspNetCore.Identity;

namespace Identity.Service
{
    public sealed class IdmProviderService<TUser> : IIdmProvider<TUser> where TUser : class, IApplicationUser
    {
        private readonly UserManager<TUser> _userManager;
        public IdmProviderService(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<TUser> FindByNameAsync(string username)
        {         
            return await _userManager.FindByNameAsync(username);
        }
        public async Task<TUser> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        public async Task<LoginStatus> LoginUserAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
            {
                return LoginStatus.WrongEmail;
            }

            bool isValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!isValidPassword)
            {
                return LoginStatus.WrongCredentials;
            }

            return LoginStatus.Succeeded;
        }
        public async Task<List<string>> GetUserRoles(TUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
        public async Task<(bool Success, string ErrorMessage)> CreateUserAsync(TUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return (true, null);
            }
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task<(bool Success, string ErrorMessage)> UpdateUserAsync(TUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return (true, null);
            }
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task<(bool Success, string ErrorMessage)> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return (true, null);
            }
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task<(bool Success, string ErrorMessage)> AssignRolesToUser(TUser user, IEnumerable<string> roles)
        {
            var result = await _userManager.AddToRolesAsync(user, roles);
            if (result.Succeeded)
            {
                return (true, null);
            }
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task<(bool Success, string ErrorMessage)> ReplaceRolesAsync(TUser user, IEnumerable<string> newRoles)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return (false, string.Join(", ", removeResult.Errors.Select(e => e.Description)));

            var newRolesList = newRoles.ToList();
            if (newRolesList.Count == 0)
                return (true, null);

            var addResult = await _userManager.AddToRolesAsync(user, newRolesList);
            if (addResult.Succeeded)
                return (true, null);

            return (false, string.Join(", ", addResult.Errors.Select(e => e.Description)));
        }
    }
}