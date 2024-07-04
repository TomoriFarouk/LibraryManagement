using LibraryManagement.Application.Command;
using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interface;
using LibraryManagement.Application.Response;
using LibraryManagement.Core.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LibraryManagement.Application.Handler.CommandHandler
{
    /// <summary>
    /// Handler for authentication-related commands.
    /// </summary>
    public class AuthCommandHandler
    {
        /// <summary>
        /// Handler for user login commands.
        /// </summary>
        public class LoginAuthCommandHandler : IRequestHandler<LoginAuthCommand, AuthResponse>
        {
            private readonly ITokenGenerator _tokenGenerator;
            private readonly ILogger<LoginAuthCommandHandler> _logger;
            private readonly IIdentityService _identityService;

            /// <summary>
            /// Initializes a new instance of the <see cref="LoginAuthCommandHandler"/> class.
            /// </summary>
            /// <param name="identityService">The identity service.</param>
            /// <param name="tokenGenerator">The token generator service.</param>
            /// <param name="logger">The logger service.</param>
            public LoginAuthCommandHandler(IIdentityService identityService, ITokenGenerator tokenGenerator, ILogger<LoginAuthCommandHandler> logger)
            {
                _identityService = identityService;
                _tokenGenerator = tokenGenerator;
                _logger = logger;
            }

            /// <inheritdoc />
            public async Task<AuthResponse> Handle(LoginAuthCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(LoginAuthCommand)} with data: {JsonConvert.SerializeObject(request)}");

                var result = await _identityService.SigninUserAsync(request.UserName, request.Password);
                if (!result)
                {
                    _logger.LogError("Invalid username or password.");
                    throw new BadRequestException("Invalid username or password.");
                }

                var (userId, fullName, userName, email, roles) = await _identityService.GetUserDetailsAsync(await _identityService.GetUserIdAsync(request.UserName));
                string token = _tokenGenerator.GenerateJWTToken((userId, userName, roles));

                _logger.LogInformation($"Login successful for user: {userName}");
                return new AuthResponse
                {
                    UserId = userId,
                    Name = userName,
                    Token = token
                };
            }
        }

        /// <summary>
        /// Handler for user registration commands.
        /// </summary>
        public class RegisterAuthCommandHandler : IRequestHandler<RegisterAuthCommand, AuthResponse>
        {
            private readonly IMediator _mediator;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ILogger<RegisterAuthCommandHandler> _logger;

            /// <summary>
            /// Initializes a new instance of the <see cref="RegisterAuthCommandHandler"/> class.
            /// </summary>
            /// <param name="userManager">The user manager service.</param>
            /// <param name="mediator">The mediator service.</param>
            /// <param name="logger">The logger service.</param>
            public RegisterAuthCommandHandler(UserManager<ApplicationUser> userManager, IMediator mediator, ILogger<RegisterAuthCommandHandler> logger)
            {
                _mediator = mediator;
                _userManager = userManager;
                _logger = logger;
            }

            /// <inheritdoc />
            public async Task<AuthResponse> Handle(RegisterAuthCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(RegisterAuthCommand)} with data: {JsonConvert.SerializeObject(request)}");

                var userByEmail = await _userManager.FindByEmailAsync(request.Email);
                var userByUsername = await _userManager.FindByNameAsync(request.UserName);
                if (userByEmail != null || userByUsername != null)
                {
                    _logger.LogError($"User with email {request.Email} or username {request.UserName} already exists.");
                    throw new ArgumentException($"User with email {request.Email} or username {request.UserName} already exists.");
                }

                var user = new ApplicationUser
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Unable to register user {request.UserName}.");
                    throw new ArgumentException($"Unable to register user {request.UserName}.");
                }

                var loginCommand = new LoginAuthCommand
                {
                    UserName = user.UserName,
                    Password = request.Password
                };

                _logger.LogInformation($"User {request.UserName} registered successfully. Initiating login.");
                return await _mediator.Send(loginCommand, cancellationToken);
            }
        }
    }
}
