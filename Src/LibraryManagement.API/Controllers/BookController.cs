using System;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Entities;
using LibraryManagement.Application.Response;
using static LibraryManagement.Application.Command.BookCommand;
using static LibraryManagement.Application.Queries.BookQueries;

namespace LibraryManagement.API.Controllers
{
	
        [Authorize]
        [Route("api/[controller]")]
        [ApiController]
        public class BookController : ControllerBase
        {
            private readonly IMediator _mediator;

            public BookController(IMediator mediator)
            {
                _mediator = mediator;
            }

            [HttpGet]
            public async Task<List<Books>> Get()
            {
                return await _mediator.Send(new GetAllBookQuery());
            }
            [HttpGet("{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            public async Task<Books  > Get(Int64 id)
            {
                return await _mediator.Send(new GetBookByIdQuery(id));
            }
           
            [HttpPost]
            [ProducesResponseType(StatusCodes.Status200OK)]
            public async Task<ActionResult<BookResponse>> CreateBook([FromBody] CreateBookCommand command)
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }

            [HttpPut("EditBook/{id}")]
            public async Task<ActionResult> EditBook(int id, [FromBody] EditBookCommand command)
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

            [HttpDelete("DeleteBook/{id}")]
            public async Task<ActionResult> DeleteBook(int id)
            {
                try
                {
                    string result = string.Empty;
                    result = await _mediator.Send(new DeleteBookCommand(id));
                    return Ok(result);
                }
                catch (Exception exp)
                {
                    return BadRequest(exp.Message);
                }
            }

           

        }
    }


