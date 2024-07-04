using System;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.Query.Base;

namespace LibraryManagement.Core.Interface.Query
{
	
        public interface IBookQuery : IQuery<Books>
        {
            Task<IReadOnlyList<Books>> GetAllAsync();
            Task<Books> GetByIdAsync(Int64 id);
            Task<Books> GetByISBNAsync(string isbn);
    }
    
}

