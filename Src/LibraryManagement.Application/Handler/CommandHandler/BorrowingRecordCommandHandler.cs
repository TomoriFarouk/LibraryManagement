using LibraryManagement.Application.Mapper;
using LibraryManagement.Application.Response;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.COMMAND;
using LibraryManagement.Core.Interface.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static LibraryManagement.Application.Command.BorrowingRecordCommand;

namespace LibraryManagement.Application.Handler.CommandHandler
{
    /// <summary>
    /// Handler for borrowing record-related commands.
    /// </summary>
    public class BorrowingRecordCommandHandler
    {
        /// <summary>
        /// Handler for creating a new borrowing record.
        /// </summary>
        public class CreateBorrowingRecordHandler : IRequestHandler<CreateBorrowingRecordCommand, BorrowingRecordResponse>
        {
            private readonly IBorrowingRecordCommand _borrowingRecordCommand;
            private readonly ILogger<CreateBorrowingRecordHandler> _logger;
            private readonly IBorrowingRecordQuery _borrowingRecordQuery;
            private readonly IBookQuery _bookQuery;
            private readonly IPatronQuery _patronQuery;
            private readonly IBookCommand _bookCommand;
            private readonly IMediator _mediator;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="CreateBorrowingRecordHandler"/> class.
            /// </summary>
            /// <param name="mediator">The mediator service.</param>
            /// <param name="bookCommand">The book command service.</param>
            /// <param name="patronQuery">The patron query service.</param>
            /// <param name="bookQuery">The book query service.</param>
            /// <param name="borrowingRecordCommand">The borrowing record command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="borrowingRecordQuery">The borrowing record query service.</param>
            public CreateBorrowingRecordHandler(IMediator mediator, IBookCommand bookCommand,CacheManager cache, IPatronQuery patronQuery, IBookQuery bookQuery, IBorrowingRecordCommand borrowingRecordCommand, ILogger<CreateBorrowingRecordHandler> logger, IBorrowingRecordQuery borrowingRecordQuery)
            {
                _borrowingRecordCommand = borrowingRecordCommand;
                _logger = logger;
                _borrowingRecordQuery = borrowingRecordQuery;
                _bookQuery = bookQuery;
                _patronQuery = patronQuery;
                _bookCommand = bookCommand;
                _mediator = mediator;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<BorrowingRecordResponse> Handle(CreateBorrowingRecordCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(CreateBorrowingRecordCommand)} with data: {JsonConvert.SerializeObject(request)}");

                // Map request to entity
                var borrowingRecordEntity = LibraryManagementMapper.Mapper.Map<BorrowingRecord>(request);
                if (borrowingRecordEntity == null)
                {
                    _logger.LogError("Mapping failed for CreateBorrowingRecordCommand.");
                    throw new ApplicationException("There is a problem in mapper.");
                }

                // Validate book and patron
                var book = await _bookQuery.GetByIdAsync(request.BookId);
                var patron = await _patronQuery.GetByIdAsync(request.PatronId);

                if (book == null || patron == null)
                {
                    _logger.LogError("Book or Patron not found.");
                    throw new ApplicationException("Book or Patron not found.");
                }

                if (book.count == 0)
                {
                    _logger.LogError("Book is not available.");
                    throw new ApplicationException("Book is not available.");
                }

                book.count -= 1;
                await _bookCommand.UpdateAsync(book);

                var newBorrowingRecord = await _borrowingRecordCommand.AddAsync(borrowingRecordEntity);
                var borrowingRecordResponse = LibraryManagementMapper.Mapper.Map<BorrowingRecordResponse>(newBorrowingRecord);
                _logger.LogInformation($"Borrowing record created successfully for book ID: {borrowingRecordResponse.BookId}");

                // Invalidate relevant caches
                _cache.InvalidateByTag("BorrowingRecord");
                return borrowingRecordResponse;
            }
        }

        /// <summary>
        /// Handler for returning a borrowed book.
        /// </summary>
        public class ReturnBookHandler : IRequestHandler<ReturnBookRecordCommand, BorrowingRecordResponse>
        {
            private readonly IBorrowingRecordCommand _borrowingRecordCommand;
            private readonly ILogger<ReturnBookHandler> _logger;
            private readonly IBorrowingRecordQuery _borrowingRecordQuery;
            private readonly IBookQuery _bookQuery;
            private readonly IPatronQuery _patronQuery;
            private readonly IBookCommand _bookCommand;
            private readonly IMediator _mediator;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReturnBookHandler"/> class.
            /// </summary>
            /// <param name="mediator">The mediator service.</param>
            /// <param name="bookCommand">The book command service.</param>
            /// <param name="patronQuery">The patron query service.</param>
            /// <param name="bookQuery">The book query service.</param>
            /// <param name="borrowingRecordCommand">The borrowing record command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="borrowingRecordQuery">The borrowing record query service.</param>
            public ReturnBookHandler(IMediator mediator, CacheManager cache,IBookCommand bookCommand, IPatronQuery patronQuery, IBookQuery bookQuery, IBorrowingRecordCommand borrowingRecordCommand, ILogger<ReturnBookHandler> logger, IBorrowingRecordQuery borrowingRecordQuery)
            {
                _borrowingRecordCommand = borrowingRecordCommand;
                _logger = logger;
                _borrowingRecordQuery = borrowingRecordQuery;
                _bookQuery = bookQuery;
                _patronQuery = patronQuery;
                _bookCommand = bookCommand;
                _mediator = mediator;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<BorrowingRecordResponse> Handle(ReturnBookRecordCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(ReturnBookRecordCommand)} with data: {JsonConvert.SerializeObject(request)}");

                var borrowingRecordEntity = LibraryManagementMapper.Mapper.Map<BorrowingRecord>(request);
                if (borrowingRecordEntity == null)
                {
                    _logger.LogError("Mapping failed for ReturnBookRecordCommand.");
                    throw new ApplicationException("There is a problem in mapper.");
                }

                var borrowingRecords = await _borrowingRecordQuery.GetByBookIdAsync(borrowingRecordEntity.BookId);

                if (borrowingRecords == null || borrowingRecords.Count == 0)
                {
                    _logger.LogError("No borrowing records found for the book.");
                    throw new ApplicationException("No borrowing records found for the book.");
                }

                var record = borrowingRecords.FirstOrDefault(b => b.PatronId == borrowingRecordEntity.PatronId);

                if (record == null || record.ReturnDate != null)
                {
                    _logger.LogError("Book has already been returned or this patron has not borrowed this book.");
                    throw new ApplicationException("Book has already been returned or this patron has not borrowed this book.");
                }

                var book = await _bookQuery.GetByIdAsync(borrowingRecordEntity.BookId);
                book.count += 1;
                record.ReturnDate = DateTime.Now;
                record.IsReturned = true;
                await _borrowingRecordCommand.UpdateAsync(record);
                await _bookCommand.UpdateAsync(book);

                var borrowingRecordResponse = LibraryManagementMapper.Mapper.Map<BorrowingRecordResponse>(record);
                _logger.LogInformation($"Book returned successfully for book ID: {borrowingRecordResponse.BookId}");

                // Invalidate the cache for this specific book
                var cacheTag = $"BorrowingRecord{record.Id}";
                _cache.InvalidateByTag(cacheTag);
                
                return borrowingRecordResponse;
            }
        }

        /// <summary>
        /// Handler for editing an existing borrowing record.
        /// </summary>
        public class EditBorrowingRecordHandler : IRequestHandler<EditBorrowingRecordCommand, BorrowingRecordResponse>
        {
            private readonly IBorrowingRecordCommand _borrowingRecordCommand;
            private readonly ILogger<EditBorrowingRecordHandler> _logger;
            private readonly IBorrowingRecordQuery _borrowingRecordQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="EditBorrowingRecordHandler"/> class.
            /// </summary>
            /// <param name="borrowingRecordCommand">The borrowing record command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="borrowingRecordQuery">The borrowing record query service.</param>
            public EditBorrowingRecordHandler(IBorrowingRecordCommand borrowingRecordCommand, CacheManager cache,ILogger<EditBorrowingRecordHandler> logger, IBorrowingRecordQuery borrowingRecordQuery)
            {
                _borrowingRecordCommand = borrowingRecordCommand;
                _logger = logger;
                _borrowingRecordQuery = borrowingRecordQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<BorrowingRecordResponse> Handle(EditBorrowingRecordCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(EditBorrowingRecordCommand)} with data: {JsonConvert.SerializeObject(request)}");

                var borrowingRecordEntity = LibraryManagementMapper.Mapper.Map<BorrowingRecord>(request);
               
                if (borrowingRecordEntity == null)
                {
                    _logger.LogError("Mapping failed for EditBorrowingRecordCommand.");
                    throw new ApplicationException("There is a problem in mapper.");
                }

                borrowingRecordEntity.LastModified = DateTime.Now;
                try
                {
                    await _borrowingRecordCommand.UpdateAsync(borrowingRecordEntity);
                }
                catch (Exception exp)
                {
                    _logger.LogError($"Error updating borrowing record: {exp.Message}");
                    throw new ApplicationException(exp.Message);
                }

                var borrowingRecords = await _borrowingRecordQuery.GetAllAsync();
                var modifiedBorrowingRecord = borrowingRecords.FirstOrDefault(x => x.Id == request.Id);
                var borrowingRecordResponse = LibraryManagementMapper.Mapper.Map<BorrowingRecordResponse>(modifiedBorrowingRecord);
                _logger.LogInformation($"Borrowing record updated successfully for record ID: {borrowingRecordResponse.Id}");

                // Invalidate the cache for this specific book
                var cacheTag = $"BorrowingRecord{request.Id}";
                _cache.InvalidateByTag(cacheTag);
                return borrowingRecordResponse;
            }
        }

        /// <summary>
        /// Handler for deleting an existing borrowing record.
        /// </summary>
        public class DeleteBorrowingRecordHandler : IRequestHandler<DeleteBorrowingRecordCommand, string>
        {
            private readonly IBorrowingRecordCommand _borrowingRecordCommand;
            private readonly ILogger<DeleteBorrowingRecordHandler> _logger;
            private readonly IBorrowingRecordQuery _borrowingRecordQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="DeleteBorrowingRecordHandler"/> class.
            /// </summary>
            /// <param name="borrowingRecordCommand">The borrowing record command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="borrowingRecordQuery">The borrowing record query service.</param>
            public DeleteBorrowingRecordHandler(IBorrowingRecordCommand borrowingRecordCommand, CacheManager cache,ILogger<DeleteBorrowingRecordHandler> logger, IBorrowingRecordQuery borrowingRecordQuery)
            {
                _borrowingRecordCommand = borrowingRecordCommand;
                _logger = logger;
                _borrowingRecordQuery = borrowingRecordQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<string> Handle(DeleteBorrowingRecordCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(DeleteBorrowingRecordCommand)} with data: {JsonConvert.SerializeObject(request)}");

                try
                {
                    var borrowingRecord = await _borrowingRecordQuery.GetByIdAsync(request.Id);
                    if (borrowingRecord == null)
                    {
                        _logger.LogError("Borrowing record not found.");
                        throw new ApplicationException("Borrowing record not found.");
                    }

                    await _borrowingRecordCommand.DeleteAsync(borrowingRecord);
                }
                catch (Exception exp)
                {
                    _logger.LogError($"Error deleting borrowing record: {exp.Message}");
                    throw new ApplicationException(exp.Message);
                }

                _logger.LogInformation("Borrowing record deleted successfully.");

                // Invalidate the cache for this specific book
                var cacheTag = $"BorrowingRecord{request.Id}";
                _cache.InvalidateByTag(cacheTag);
                return "Borrowing record deleted successfully.";
            }
        }
    }
}
