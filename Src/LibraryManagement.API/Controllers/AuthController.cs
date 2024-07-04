using System;
using LibraryManagement.Application.Command;
using LibraryManagement.Application.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {

            _mediator = mediator;
        }

        [HttpPost("Login")]
        [ProducesDefaultResponseType(typeof(AuthResponse))]
        public async Task<IActionResult> Login([FromBody] LoginAuthCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("Register")]
        [ProducesDefaultResponseType(typeof(AuthResponse))]
        public async Task<IActionResult> Register([FromBody] RegisterAuthCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}

