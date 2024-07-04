using System;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.COMMAND;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repository.Command.Base;

namespace LibraryManagement.Infrastructure.Repository.Command
{
    public class PatronCommandRepository : CommandRepository<Patron>, IPatronCommand
    {
        public PatronCommandRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

