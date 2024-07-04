using System;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.COMMAND;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repository.Command.Base;

namespace LibraryManagement.Infrastructure.Repository.Command
{
    public class BorrowingRecordCommandRepository : CommandRepository<BorrowingRecord>, IBorrowingRecordCommand
    {
        public BorrowingRecordCommandRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

