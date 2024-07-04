using System;
using LibraryManagement.Core.Entities;
using MediatR;

namespace LibraryManagement.Application.Queries
{
    public class BookQueries
    {
        public class GetAllBookQuery : IRequest<List<Books>>
        {

        }

       

        public class GetBookByIdQuery : IRequest<Books>
        {
            public Int64 Id { get; set; }
            public GetBookByIdQuery(Int64 id)
            {
                this.Id = id;
            }
        }

        public class GetBookByISBNQuery : IRequest<Books>
        {
            public string isbn{ get; set; }
            public GetBookByISBNQuery(string isbn)
            {
                this.isbn = isbn;
            }
        }
    }
}

