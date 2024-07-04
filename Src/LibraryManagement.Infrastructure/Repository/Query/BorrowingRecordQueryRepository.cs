using System;
using Dapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Entities.Identity;
using LibraryManagement.Core.Interface.Query;
using LibraryManagement.Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;

namespace LibraryManagement.Infrastructure.Repository.Query
{
    public class BorrowingRecordQueryRepository : QueryRepository<BorrowingRecord>, IBorrowingRecordQuery

    {
        public BorrowingRecordQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //public async Task<IReadOnlyList<BorrowingRecord>> GetAllAsync()
        //{
        //    try
        //    {
        //        var query = "SELECT * FROM BORROWINGRECORD";
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryAsync<BorrowingRecord>(query)).ToList();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}


        public async Task<IReadOnlyList<BorrowingRecord>> GetAllAsync()
        {
            try
            {
                var query = @"
            SELECT br.*, p.*, b.*, a.*
            FROM BorrowingRecord br
            LEFT JOIN Patron p ON p.Id = br.PatronId
            LEFT JOIN Books b ON b.Id = br.BookId
            LEFT JOIN AspNetUsers a ON a.Id = br.UserId
        ";

                using (var connection = CreateConnection())
                {
                    var borrowingRecordDictionary = new Dictionary<long, BorrowingRecord>();
                    var result = await connection.QueryAsync<BorrowingRecord, Patron, Books, ApplicationUser, BorrowingRecord>(
                        query,
                        (borrowingRecord, patron, book, applicationUser) =>
                        {
                            BorrowingRecord borrowingRecordEntry;

                            if (!borrowingRecordDictionary.TryGetValue(borrowingRecord.Id, out borrowingRecordEntry))
                            {
                                borrowingRecordEntry = borrowingRecord;
                                borrowingRecordEntry.patron = patron;
                                borrowingRecordEntry.Book = book;
                                borrowingRecordEntry.applicationUser = applicationUser;
                                borrowingRecordDictionary.Add(borrowingRecordEntry.Id, borrowingRecordEntry);
                            }

                            return borrowingRecordEntry;
                        },
                        splitOn: "Id, Id, Id" // Ensure correct splitting points based on column names
                    );

                    return result.Distinct().ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Error retrieving Borrowing Records with related entities", exp);
            }
        }

        //public async Task<BorrowingRecord> GetByIdAsync(long id)
        //{
        //    try
        //    {
        //        var query = "SELECT * FROM BORROWINGRECORD WHERE Id =@Id";
        //        var parameters = new DynamicParameters();
        //        parameters.Add("Id", id, System.Data.DbType.Int64);
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryFirstOrDefaultAsync<BorrowingRecord>(query, parameters));
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}

        public async Task<BorrowingRecord> GetByIdAsync(long Id)
        {
            try
            {
                var query = @"
            SELECT br.*, p.*, b.*, a.*
            FROM BorrowingRecord br
            LEFT JOIN Patron p ON p.Id = br.PatronId
            LEFT JOIN Books b ON b.Id = br.BookId
            LEFT JOIN AspNetUsers a ON a.Id = br.UserId
            WHERE br.Id = @Id
        ";

                var parameters = new { Id = Id };

                using (var connection = CreateConnection())
                {
                    var borrowingRecordDictionary = new Dictionary<long, BorrowingRecord>();

                    var result = await connection.QueryAsync<BorrowingRecord, Patron, Books, ApplicationUser, BorrowingRecord>(
                        query,
                        (borrowingRecord, patron, book, applicationUser) =>
                        {
                            BorrowingRecord borrowingRecordEntry;

                            if (!borrowingRecordDictionary.TryGetValue(borrowingRecord.Id, out borrowingRecordEntry))
                            {
                                borrowingRecordEntry = borrowingRecord;
                                borrowingRecordEntry.patron = patron;
                                borrowingRecordEntry.Book = book;
                                borrowingRecordEntry.applicationUser = applicationUser;
                                borrowingRecordDictionary.Add(borrowingRecordEntry.Id, borrowingRecordEntry);
                            }

                            return borrowingRecordEntry;
                        },
                        parameters,
                        splitOn: "Id, Id, Id" // Ensure correct splitting points based on column names
                    );

                    return result.FirstOrDefault();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Error retrieving Borrowing Record by Id", exp);
            }
        }




        //public async Task<IReadOnlyList<BorrowingRecord>> GetByBookIdAsync(Int64 BookId)
        //    {
        //        try
        //        {
        //            var query = "SELECT * FROM BORROWINGRECORD WHERE BookId = @BookId";
        //            var parameters = new DynamicParameters();
        //            parameters.Add("BookId", BookId, System.Data.DbType.Int64);
        //            using (var connection = CreateConnection())
        //            {
        //                return (await connection.QueryAsync<BorrowingRecord>(query,parameters)).ToList();
        //            }
        //        }
        //        catch (Exception exp)
        //        {
        //            throw new Exception(exp.Message, exp);
        //        }
        //    }

        //}


        public async Task<IReadOnlyList<BorrowingRecord>> GetByBookIdAsync(long bookId)
        {
            try
            {
                var query = @"
            SELECT br.*, p.*, a.Id as ApplicationUserId, a.*, b.*
            FROM BorrowingRecord br
            LEFT JOIN Patron p ON p.Id = br.PatronId
            LEFT JOIN AspNetUsers a ON a.Id = br.UserId
            LEFT JOIN Books b ON b.Id = br.BookId
            WHERE br.BookId = @BookId
        ";

                var parameters = new { BookId = bookId };

                using (var connection = CreateConnection())
                {
                    var borrowingRecordDictionary = new Dictionary<long, BorrowingRecord>();

                    var result = await connection.QueryAsync<BorrowingRecord, Patron, ApplicationUser, Books, BorrowingRecord>(
                        query,
                        (borrowingRecord, patron, applicationUser, book) =>
                        {
                            if (!borrowingRecordDictionary.TryGetValue(borrowingRecord.Id, out var borrowingRecordEntry))
                            {
                                borrowingRecordEntry = borrowingRecord;
                                borrowingRecordEntry.patron = patron;
                                borrowingRecordEntry.applicationUser = applicationUser;
                                borrowingRecordEntry.Book = book;
                                borrowingRecordDictionary.Add(borrowingRecordEntry.Id, borrowingRecordEntry);
                            }

                            return borrowingRecordEntry;
                        },
                        parameters,
                        splitOn: "Id,ApplicationUserId,Id" // Specify the correct split columns
                    );

                    return result.Distinct().ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Error retrieving Borrowing Records by BookId", exp);
            }
        }



    }
}


