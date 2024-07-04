using System;
namespace LibraryManagement.Core.Interface.COMMAND.Base
{
    public interface ICommand<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}

