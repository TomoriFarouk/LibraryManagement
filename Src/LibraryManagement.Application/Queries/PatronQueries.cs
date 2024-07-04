using System;
using LibraryManagement.Core.Entities;
using MediatR;

namespace LibraryManagement.Application.Queries
{
    public class PatronQueries
    {
        public class GetAllPatronQuery : IRequest<List<Patron>>
        {

        }



        public class GetPatronByIdQuery : IRequest<Patron>
        {
            public Int64 Id { get; set; }
            public GetPatronByIdQuery(Int64 id)
            {
                this.Id = id;
            }
        }

       
    }
}

