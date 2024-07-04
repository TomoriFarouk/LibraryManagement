using System;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.COMMAND;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repository.Command.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repository.Command
{
    
    public class BookCommandRepository : CommandRepository<Books>, IBookCommand
    {
        public BookCommandRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

