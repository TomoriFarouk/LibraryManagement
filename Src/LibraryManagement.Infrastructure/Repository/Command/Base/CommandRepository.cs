using System;
using LibraryManagement.Core.Interface.COMMAND.Base;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Repository.Command.Base
{
    public class CommandRepository<T> : ICommand<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public CommandRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return entity;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(T entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //_context.Entry(entity).Property("RowVersion").OriginalValue = entity.RowVersion;
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is T)
                    {
                        var databaseValues = await entry.GetDatabaseValuesAsync();
                        if (databaseValues == null)
                        {
                            throw new Exception("The entity being updated was already deleted by another user.");
                        }

                        entry.OriginalValues.SetValues(databaseValues);
                        entry.CurrentValues.SetValues(databaseValues);
                    }
                    else
                    {
                        throw new NotSupportedException($"Don't know how to handle concurrency conflicts for {entry.Metadata.Name}");
                    }
                }
                throw new Exception($"A concurrency conflict occurred. Please try again. {ex.Message}");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task DeleteAsync(T entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is T)
                    {
                        var databaseValues = await entry.GetDatabaseValuesAsync();
                        if (databaseValues == null)
                        {
                            throw new Exception("The entity being deleted was already deleted by another user.");
                        }
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    else
                    {
                        throw new NotSupportedException($"Don't know how to handle concurrency conflicts for {entry.Metadata.Name}");
                    }
                }
                throw new Exception("A concurrency conflict occurred. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
