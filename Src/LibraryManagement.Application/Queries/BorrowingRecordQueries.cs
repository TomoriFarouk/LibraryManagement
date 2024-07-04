using System;
using LibraryManagement.Core.Entities;
using MediatR;

namespace LibraryManagement.Application.Queries
{
	public class BorrowingRecordQueries
	{
        public class GetAllBorrowingRecordQuery : IRequest<List<BorrowingRecord>>
        {

        }



        public class GetBorrowingRecordByIdQuery : IRequest<BorrowingRecord>
        {
            public Int64 Id { get; set; }
            public GetBorrowingRecordByIdQuery(Int64 id)
            {
                this.Id = id;
            }
        }
    }
}

