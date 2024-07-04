using System;
using LibraryManagement.Core.Interface.Query.Base;
using LibraryManagement.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace LibraryManagement.Infrastructure.Repository.Query.Base
{
    public class QueryRepository<T> : DbConnector, IQuery<T> where T : class
    {
        public QueryRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}

