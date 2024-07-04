using System;
using LibraryManagement.Application.Response;
using MediatR;

namespace LibraryManagement.Application.Command
{
    public class LoginAuthCommand : IRequest<AuthResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }


    public class RegisterAuthCommand : IRequest<AuthResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}

