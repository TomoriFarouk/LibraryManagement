using System;
using Dapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Entities.Identity;
using LibraryManagement.Core.Interface.Query;
using LibraryManagement.Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;

namespace LibraryManagement.Infrastructure.Repository.Query
{
    public class PatronQueryRepository : QueryRepository<Patron>, IPatronQuery

    {
        public PatronQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //public async Task<IReadOnlyList<Patron>> GetAllAsync()
        //{
        //    try
        //    {
        //        var query = "SELECT * FROM PATRON";
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryAsync<Patron>(query)).ToList();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}

        public async Task<IReadOnlyList<Patron>> GetAllAsync()
        {
            try
            {
                var query = @"
                SELECT b.*, br.*, a.*
                FROM Patron b
                LEFT JOIN AspNetUsers a ON a.Id = b.UserId
                LEFT JOIN BorrowingRecord br ON b.Id = br.PatronId;              
            ";

                using (var connection = CreateConnection())
                {
                    var patronDictionary = new Dictionary<long, Patron>();
                    var result = await connection.QueryAsync<Patron, BorrowingRecord, ApplicationUser,Patron>(
                        query,
                        (patron, borrowingRecord,applicationUser) =>
                        {
                            Patron patronEntry;

                            if (!patronDictionary.TryGetValue(patron.Id, out patronEntry))
                            {
                                patronEntry = patron;
                                patronEntry.BorrowingRecords = new List<BorrowingRecord>();
                                patronDictionary.Add(patronEntry.Id, patronEntry);
                            }

                            if (borrowingRecord != null)
                                patronEntry.BorrowingRecords.Add(borrowingRecord);

                            if (applicationUser != null && patronEntry.applicationUser == null)
                                patronEntry.applicationUser = applicationUser;

                            return patronEntry;
                        },
                        splitOn: "Id, Id"
                    );

                    return result.Distinct().ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        //public async Task<Patron> GetByIdAsync(long id)
        //{
        //    try
        //    {
        //        var query = "SELECT * FROM PATRON WHERE Id =@Id";
        //        var parameters = new DynamicParameters();
        //        parameters.Add("Id", id, System.Data.DbType.Int64);
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryFirstOrDefaultAsync<Patron>(query, parameters));
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}

        public async Task<Patron> GetByIdAsync(long Id)
        {
            try
            {
                var query = @"
                SELECT b.*, br.*, a.*
                FROM PATRON b
                LEFT JOIN AspNetUsers a ON a.Id = b.UserId
                LEFT JOIN BorrowingRecord br ON b.Id = br.PatronId
                WHERE b.Id = @Id;
            ";

                var parameters = new {  Id = Id };
                using (var connection = CreateConnection())
                {
                    var patronDictionary = new Dictionary<long, Patron>();
                    var result = await connection.QueryAsync<Patron, BorrowingRecord, ApplicationUser,Patron>(
                        query,
                        (patron, borrowingRecord,applicationUser) =>
                        {
                            Patron patronEntry;

                            if (!patronDictionary.TryGetValue(patron.Id, out patronEntry))
                            {
                                patronEntry = patron;
                                patronEntry.BorrowingRecords = new List<BorrowingRecord>();
                                patronDictionary.Add(patronEntry.Id, patronEntry);
                            }

                            if (borrowingRecord != null)
                                patronEntry.BorrowingRecords.Add(borrowingRecord);

                            if (applicationUser != null && patronEntry.applicationUser == null)
                               patronEntry.applicationUser = applicationUser;

                            return patronEntry;
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

    }
}

