using System;
using System.Transactions;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.Infrastructure.Services
{
    public class TransactionScopeManager
    {
        private readonly ApplicationDbContext _context;

        public TransactionScopeManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await action();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}