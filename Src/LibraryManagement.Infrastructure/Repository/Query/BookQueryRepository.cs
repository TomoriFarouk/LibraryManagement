using System;
using Dapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Entities.Identity;
using LibraryManagement.Core.Interface.Query;
using LibraryManagement.Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;

namespace LibraryManagement.Infrastructure.Repository.Query
{
    public class BookQueryRepository : QueryRepository<Books>, IBookQuery
    {
        public BookQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }



        //public async Task<IReadOnlyList<Books>> GetAllAsync()
        //{
        //    try
        //    {
        //        var query = "SELECT * FROM BOOKS";
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryAsync<Books>(query)).ToList();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}

        //====== Using EF framework ==== ==

        //public async Task<List<Books>> GetAllAsync()
        //{
        //    using (var context = new LibraryManagementDbContext())
        //    {
        //        var books = await context.Books
        //            .Include(b => b.BorrowingRecords)
        //            .ThenInclude(br => br.Patron)
        //            .ToListAsync();

        //        return books;
        //    }
        //}

        //==========================

        public async Task<IReadOnlyList<Books>> GetAllAsync()
        {

            try
            {
                var query = @"
            SELECT b.*, br.*, a.*
            FROM Books b
            LEFT JOIN BorrowingRecord br ON b.Id = br.BookId
            LEFT JOIN AspNetUsers a ON a.Id = b.UserId";

                using (var connection = CreateConnection())
                {
                    var bookDictionary = new Dictionary<long, Books>();
                    var result = await connection.QueryAsync<Books, BorrowingRecord, ApplicationUser, Books>(
                        query,
                        (book, borrowingRecord, applicationUser) =>
                        {
                            Books bookEntry;

                            if (!bookDictionary.TryGetValue(book.Id, out bookEntry))
                            {
                                bookEntry = book;
                                bookEntry.BorrowingRecords = new List<BorrowingRecord>();
                                bookDictionary.Add(bookEntry.Id, bookEntry);
                            }

                            if (borrowingRecord != null)
                                bookEntry.BorrowingRecords.Add(borrowingRecord);

                            if (applicationUser != null && bookEntry.applicationUser == null)
                                bookEntry.applicationUser = applicationUser;

                            return bookEntry;
                        },
                        splitOn: "Id, Id"
                    );

                    return result.Distinct().ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Error retrieving books with ApplicationUser information", exp);
            }
        }


        //    public async Task<Books> GetByIdAsync(long id)
        //   {
        //    try
        //    {
        //        var query = "SELECT * FROM BOOKS WHERE Id =@Id";
        //        var parameters = new DynamicParameters();
        //        parameters.Add("Id", id, System.Data.DbType.Int64);
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryFirstOrDefaultAsync<Books>(query, parameters));
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}

        //========= Using EF framework==============

        //public async Task<Books> GetByIdAsync(int bookId)
        //{
        //    using (var context = new LibraryManagementDbContext())
        //    {
        //        var book = await context.Books
        //            .Include(b => b.BorrowingRecords)
        //            .ThenInclude(br => br.Patron)
        //            .FirstOrDefaultAsync(b => b.Id == bookId);

        //        if (book == null)
        //        {
        //            throw new KeyNotFoundException($"No book found with ID: {bookId}");
        //        }

        //        return book;
        //    }
        //}


        public async Task<Books> GetByIdAsync(long bookId)
        {
            try
            {
                var query = @"
                SELECT b.*, br.*,a.*
                FROM Books b
                LEFT JOIN BorrowingRecord br ON b.Id = br.BookId
                LEFT JOIN AspNetUsers a ON a.Id = b.UserId
                WHERE b.Id = @Id;
            ";

                var parameters = new { Id = bookId };
                using (var connection = CreateConnection())
                {
                    var bookDictionary = new Dictionary<long, Books>();
                    var result = await connection.QueryAsync<Books, BorrowingRecord, ApplicationUser, Books>(
                        query,
                        (book, borrowingRecord, applicationUser) =>
                        {
                            Books bookEntry;

                            if (!bookDictionary.TryGetValue(book.Id, out bookEntry))
                            {
                                bookEntry = book;
                                bookEntry.BorrowingRecords = new List<BorrowingRecord>();
                                bookDictionary.Add(bookEntry.Id, bookEntry);
                            }

                            if (borrowingRecord != null)
                                bookEntry.BorrowingRecords.Add(borrowingRecord);

                            if (applicationUser != null && bookEntry.applicationUser == null)
                                bookEntry.applicationUser = applicationUser;

                            return bookEntry;
                        },
                        parameters,
                        splitOn: "Id, Id"
                    );

                    return result.FirstOrDefault();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        //public async Task<Books> GetByISBNAsync(string isbn)
        //{
        //    try
        //    {
        //        var query = "SELECT * FROM BOOKS WHERE ISBN =@isbn";
        //        var parameters = new DynamicParameters();
        //        parameters.Add("isbn", isbn, System.Data.DbType.String);
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryFirstOrDefaultAsync<Books>(query, parameters));
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}

        public async Task<Books> GetByISBNAsync(string isbn)
        {
            try
            {
                var query = @"
            SELECT b.*, br.*, a.*
            FROM Books b
            LEFT JOIN BorrowingRecord br ON b.Id = br.BookId
            LEFT JOIN AspNetUsers a ON a.Id = b.UserId
            WHERE b.ISBN = @isbn";

                var parameters = new { isbn };
                using (var connection = CreateConnection())
                {
                    var bookDictionary = new Dictionary<long, Books>();
                    var result = await connection.QueryAsync<Books, BorrowingRecord, ApplicationUser, Books>(
                        query,
                        (book, borrowingRecord, applicationUser) =>
                        {
                            Books bookEntry;

                            if (!bookDictionary.TryGetValue(book.Id, out bookEntry))
                            {
                                bookEntry = book;
                                bookEntry.BorrowingRecords = new List<BorrowingRecord>();
                                bookDictionary.Add(bookEntry.Id, bookEntry);
                            }

                            if (borrowingRecord != null)
                                bookEntry.BorrowingRecords.Add(borrowingRecord);

                            if (applicationUser != null && bookEntry.applicationUser == null)
                                bookEntry.applicationUser = applicationUser;

                            return bookEntry;
                        },
                        parameters,
                        splitOn: "Id, Id"
                    );

                    return result.FirstOrDefault();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Error retrieving book by ISBN with ApplicationUser information", exp);
            }
        }

    }
}

