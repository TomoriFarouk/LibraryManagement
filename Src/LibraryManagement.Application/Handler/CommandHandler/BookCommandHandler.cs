using LibraryManagement.Application.Common.Interface;
using LibraryManagement.Application.Mapper;
using LibraryManagement.Application.Response;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.COMMAND;
using LibraryManagement.Core.Interface.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static LibraryManagement.Application.Command.BookCommand;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LibraryManagement.Application.Handler.CommandHandler
{
    /// <summary>
    /// Handler for book-related commands.
    /// </summary>
    public class BookCommandHandler
    {
        /// <summary>
        /// Handler for creating a new book.
        /// </summary>
        public class CreateBookHandler : IRequestHandler<CreateBookCommand, BookResponse>
        {
            private readonly IBookCommand _bookCommand;
            private readonly ILogger<CreateBookHandler> _logger;
            private readonly IBookQuery _bookQuery;
            private readonly IIdentityService _identityService;
            private readonly IBorrowingRecordQuery _borrowingRecordQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="CreateBookHandler"/> class.
            /// </summary>
            /// <param name="bookCommand">The book command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="bookQuery">The book query service.</param>
            /// <param name="identityService">The identity service.</param>
            ///  <param name="borrowingRecordQuery">The identity service.</param>

            public CreateBookHandler(IBookCommand bookCommand, ILogger<CreateBookHandler> logger, IBookQuery bookQuery, IIdentityService identityService,IBorrowingRecordQuery borrowingRecordQuery,CacheManager cache)
            {
                _bookCommand = bookCommand;
                _logger = logger;
                _bookQuery = bookQuery;
                _identityService = identityService;
                _borrowingRecordQuery = borrowingRecordQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<BookResponse> Handle(CreateBookCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(CreateBookCommand)} with data: {JsonConvert.SerializeObject(request)}");

                // Check if user exists
                var userExists = await _identityService.GetUserByIdAsync(request.UserId);
                if (userExists == null)
                {
                    _logger.LogError($"User with ID {request.UserId} does not exist.");
                    throw new ApplicationException($"User with ID {request.UserId} does not exist.");
                }

                // Map request to entity
                var bookEntity = LibraryManagementMapper.Mapper.Map<Books>(request);
                if (bookEntity == null)
                {
                    _logger.LogError("Mapping failed for CreateBookCommand.");
                    throw new ApplicationException("There is a problem in mapper.");
                }

                // Check if book already exists
                var existingBook = await _bookQuery.GetByISBNAsync(bookEntity.ISBN);
                if (existingBook != null)
                {
                    existingBook.count += 1;
                    await _bookCommand.UpdateAsync(existingBook);
                    var updatedResponse = LibraryManagementMapper.Mapper.Map<BookResponse>(existingBook);
                    _logger.LogInformation($"Book updated successfully with ISBN: {updatedResponse.ISBN}");
                    return updatedResponse;
                }

                // Add new book
                bookEntity.applicationUser = userExists;
                var newBook = await _bookCommand.AddAsync(bookEntity);
                var books = await _bookQuery.GetAllAsync();
                var modifiedBook = books.FirstOrDefault(x => x.ISBN == newBook.ISBN);

                var bookResponse = LibraryManagementMapper.Mapper.Map<BookResponse>(newBook);
                _logger.LogInformation($"Book created successfully with ISBN: {bookResponse.ISBN}");

                // Invalidate relevant caches
                _cache.InvalidateByTag("Books");
              
                return bookResponse;
            }
        }

        /// <summary>
        /// Handler for editing an existing book.
        /// </summary>
        public class EditBookHandler : IRequestHandler<EditBookCommand, BookResponse>
        {
            private readonly IBookCommand _bookCommand;
            private readonly ILogger<EditBookHandler> _logger;
            private readonly IBookQuery _bookQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="EditBookHandler"/> class.
            /// </summary>
            /// <param name="bookCommand">The book command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="bookQuery">The book query service.</param>
            public EditBookHandler(IBookCommand bookCommand, ILogger<EditBookHandler> logger, IBookQuery bookQuery, CacheManager cache)
            {
                _bookCommand = bookCommand;
                _logger = logger;
                _bookQuery = bookQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<BookResponse> Handle(EditBookCommand request, CancellationToken cancellationToken)
            {
                string cacheTag = $"Book{request.Id}";
                string cachetagIsbn = $"Book{request.ISBN}";
                _logger.LogInformation($"Handling {nameof(EditBookCommand)} with data: {JsonConvert.SerializeObject(request)}");

                // Map request to entity
                var bookEntity = LibraryManagementMapper.Mapper.Map<Books>(request);

                if (bookEntity == null)
                {
                    _logger.LogError("Mapping failed for EditBookCommand.");
                    throw new ApplicationException("There is a problem in mapper.");
                }

                // Set LastModified and RowVersion
                bookEntity.LastModified = DateTime.UtcNow;

                // Update book
                try
                {
                    await _bookCommand.UpdateAsync(bookEntity);
                }
                catch (Exception exp)
                {
                    _logger.LogError($"Error updating book: {exp.Message}");
                    throw new ApplicationException(exp.Message);
                }

                // Invalidate the cache for this specific book
                _cache.InvalidateByTag(cacheTag);
                _cache.InvalidateByTag(cachetagIsbn);

                // Get updated book
                var books = await _bookQuery.GetAllAsync();
                var modifiedBook = books.FirstOrDefault(x => x.Id == request.Id);
                var bookResponse = LibraryManagementMapper.Mapper.Map<BookResponse>(modifiedBook);
                _logger.LogInformation($"Book updated successfully with ISBN: {bookResponse?.ISBN}");

                return bookResponse;
            }
        }

            /// <summary>
            /// Handler for deleting a book.
            /// </summary>
            public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, string>
        {
            private readonly IBookCommand _bookCommand;
            private readonly ILogger<DeleteBookHandler> _logger;
            private readonly IBookQuery _bookQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="DeleteBookHandler"/> class.
            /// </summary>
            /// <param name="bookCommand">The book command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="bookQuery">The book query service.</param>
            public DeleteBookHandler(IBookCommand bookCommand, ILogger<DeleteBookHandler> logger, IBookQuery bookQuery,CacheManager cache)
            {
                _bookCommand = bookCommand;
                _logger = logger;
                _bookQuery = bookQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<string> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(DeleteBookCommand)} with data: {JsonConvert.SerializeObject(request)}");

                // Delete book
                try
                {
                    var bookEntity = await _bookQuery.GetByIdAsync(request.Id);
                    if (bookEntity == null)
                    {
                        _logger.LogError($"Book with ID {request.Id} not found.");
                        throw new ApplicationException($"Book with ID {request.Id} not found.");
                    }

                    await _bookCommand.DeleteAsync(bookEntity);
                }
                catch (Exception exp)
                {
                    _logger.LogError($"Error deleting book: {exp.Message}");
                    throw new ApplicationException(exp.Message);
                }

                _logger.LogInformation($"Book with ID {request.Id} deleted successfully.");

                // Invalidate the cache for this specific book
                var cacheTag = $"Book{request.Id}";
                _cache.InvalidateByTag(cacheTag);

                return $"Book with ID {request.Id} has been deleted!";
            }
        }
    }
}
