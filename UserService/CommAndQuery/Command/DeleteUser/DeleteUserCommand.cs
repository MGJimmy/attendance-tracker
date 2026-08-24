using MediatR;

namespace Identity.Service
{
    public class DeleteUserResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class DeleteUserCommand : IRequest<DeleteUserResponse>
    {
        public string UserId { get; set; }
    }


}