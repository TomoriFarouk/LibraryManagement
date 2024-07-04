using System;
using LibraryManagement.Application.Response;
using LibraryManagement.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LibraryManagement.Application.Command.PatronCommand;
using static LibraryManagement.Application.Queries.PatronQueries;

namespace LibraryManagement.API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatronController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatronController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<List<Patron>> Get()
        {
            return await _mediator.Send(new GetAllPatronQuery());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Patron> Get(Int64 id)
        {
            return await _mediator.Send(new GetPatronByIdQuery(id));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PatronResponse>> CreatePatron([FromBody] CreatePatronCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("EditPatron/{id}")]
        public async Task<ActionResult> EditPatron(int id, [FromBody] EditPatronCommand command)
        {
            try
            {
                if (command.Id == id)
                {
                    var result = await _mediator.Send(command);
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }


        }

        [HttpDelete("DeletePatron/{id}")]
        public async Task<ActionResult> DeletePatron(int id)
        {
            try
            {
                string result = string.Empty;
                result = await _mediator.Send(new DeletePatronCommand(id));
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }



    }
}

