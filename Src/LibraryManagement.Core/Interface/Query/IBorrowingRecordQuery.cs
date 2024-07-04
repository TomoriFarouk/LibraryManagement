using System;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.Query.Base;

namespace LibraryManagement.Core.Interface.Query
{
    public interface IBorrowingRecordQuery : IQuery<BorrowingRecord>
    {
        Task<IReadOnlyList<BorrowingRecord>> GetAllAsync();
        Task<IReadOnlyList<BorrowingRecord>> GetByBookIdAsync(Int64 BookId);
        Task<BorrowingRecord> GetByIdAsync(Int64 id);

    }
}

