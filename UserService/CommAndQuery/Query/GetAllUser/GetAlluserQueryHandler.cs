using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Service;

public class GetAlluserQueryHandler : IRequestHandler<GetAlluserQuery, List<UserDto>>
{
    private readonly IdentityDbContext _context;
    private readonly UserManager<User> _userManager;

    public GetAlluserQueryHandler(IdentityDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<UserDto>> Handle(GetAlluserQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.Users.ToListAsync(cancellationToken);
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList()
            });
        }
        return result;
    }
}
