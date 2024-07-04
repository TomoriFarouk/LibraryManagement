using System;
using LibraryManagement.Application.Response;
using LibraryManagement.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LibraryManagement.Application.Command.BorrowingRecordCommand;
using static LibraryManagement.Application.Queries.BorrowingRecordQueries;

namespace LibraryManagement.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingRecordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BorrowingRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<List<BorrowingRecord>> Get()
        {
            return await _mediator.Send(new GetAllBorrowingRecordQuery());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<BorrowingRecord> Get(Int64 id)
        {
            return await _mediator.Send(new GetBorrowingRecordByIdQuery(id));
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BorrowingRecordResponse>> CreateBorrowingRecord([FromBody] CreateBorrowingRecordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("Return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BorrowingRecordResponse>> ReturnBookRecord([FromBody] ReturnBookRecordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("EditBorrowingRecord/{id}")]
        public async Task<ActionResult> EditBorrowingRecord(int id, [FromBody] EditBorrowingRecordCommand command)
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

        [HttpDelete("DeleteBorrowingRecord/{id}")]
        public async Task<ActionResult> DeleteBorrowingRecord(int id)
        {
            try
            {
                string result = string.Empty;
                result = await _mediator.Send(new DeleteBorrowingRecordCommand(id));
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }



    }
}

