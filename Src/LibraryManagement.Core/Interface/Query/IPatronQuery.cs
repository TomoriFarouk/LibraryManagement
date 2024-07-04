using System;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.Query.Base;

namespace LibraryManagement.Core.Interface.Query
{
    public interface IPatronQuery : IQuery<Patron>
    {
        Task<IReadOnlyList<Patron>> GetAllAsync();
        Task<Patron> GetByIdAsync(Int64 id);
    
    }
}

