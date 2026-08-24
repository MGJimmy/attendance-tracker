namespace Identity.Service;

public interface IUserContextService
{
    string GetCurrentUserId();
    string GetCurrentUserName();
    IEnumerable<string> GetCurrentUserRoles();
}