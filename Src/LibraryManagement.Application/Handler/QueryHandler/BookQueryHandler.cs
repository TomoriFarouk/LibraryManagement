using LibraryManagement.Core.Interface.Query;
using LibraryManagement.Core.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static LibraryManagement.Application.Queries.BookQueries;

namespace LibraryManagement.Application.Handler.QueryHandler
{
    /// <summary>
    /// Handler for book-related queries.
    /// </summary>
    public class BookQueryHandler
    {
        /// <summary>
        /// Handler for retrieving all books.
        /// </summary>
        public class GetAllBookHandler : IRequestHandler<GetAllBookQuery, List<Books>>
        {
            private readonly ILogger<GetAllBookHandler> _logger;
            private readonly IBookQuery _bookQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="GetAllBookHandler"/> class.
            /// </summary>
            /// <param name="logger">The logger service.</param>
            /// <param name="bookQuery">The book query service.</param>
            public GetAllBookHandler(ILogger<GetAllBookHandler> logger, IBookQuery bookQuery,CacheManager cache)
            {
                _logger = logger;
                _bookQuery = bookQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<List<Books>> Handle(GetAllBookQuery request, CancellationToken cancellationToken)
            {
                var cacheKey = "Book";
                var cacheTag = "Book";
                var cachedBooks = await _cache.GetOrCreateAsync(cacheKey, async() =>
                {                  
                    _logger.LogInformation($"Handling {nameof(GetAllBookQuery)} with data: {JsonConvert.SerializeObject(request)}");
                    var books = await _bookQuery.GetAllAsync();
                    _logger.LogInformation($"Book found: {JsonConvert.SerializeObject(books)}");
                    return books.ToList();
                },TimeSpan.FromMinutes(10),cacheTag);
                return cachedBooks;
            }
        }

        /// <summary>
        /// Handler for retrieving a book by its ID.
        /// </summary>
        public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, Books>
        {
            private readonly IMediator _mediator;
            private readonly ILogger<GetBookByIdHandler> _logger;
            private readonly IBookQuery _bookQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="GetBookByIdHandler"/> class.
            /// </summary>
            /// <param name="mediator">The mediator service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="bookQuery">The book query service.</param>
            public GetBookByIdHandler(IMediator mediator, ILogger<GetBookByIdHandler> logger, IBookQuery bookQuery,CacheManager cache)
            {
                _mediator = mediator;
                _logger = logger;
                _bookQuery = bookQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<Books> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
            {
                
                var cacheKey = $"Book{request.Id}";
                var cacheTag = $"Book{request.Id}";
                var cachedBooks = await _cache.GetOrCreateAsync(cacheKey, async () =>
                {
                    _logger.LogInformation($"Handling {nameof(GetBookByIdQuery)} with data: {JsonConvert.SerializeObject(request)}");
                   
                    var selectedBook = await _bookQuery.GetByIdAsync(request.Id);

                    if (selectedBook == null)
                    {
                        _logger.LogWarning($"No book found with ID: {request.Id}");
                        throw new KeyNotFoundException($"No book found with ID: {request.Id}");
                    }

                    _logger.LogInformation($"Book found: {JsonConvert.SerializeObject(selectedBook)}");
                    return selectedBook;
                },TimeSpan.FromMinutes(10),cacheTag);

                return cachedBooks;
            }
        }

        /// <summary>
        /// Handler for retrieving a book by its ISBN.
        /// </summary>
        public class GetBookByISBNHandler : IRequestHandler<GetBookByISBNQuery, Books>
        {
            private readonly IMediator _mediator;
            private readonly ILogger<GetBookByISBNHandler> _logger;
            private readonly IBookQuery _bookQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="GetBookByISBNHandler"/> class.
            /// </summary>
            /// <param name="mediator">The mediator service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="bookQuery">The book query service.</param>
            public GetBookByISBNHandler(IMediator mediator, ILogger<GetBookByISBNHandler> logger, IBookQuery bookQuery,CacheManager cache)
            {
                _mediator = mediator;
                _logger = logger;
                _bookQuery = bookQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<Books> Handle(GetBookByISBNQuery request, CancellationToken cancellationToken)
            {
                var cacheKey = $"Book{request.isbn}";
                var cashTag = $"Book{request.isbn}";
                var cachedBooks = await _cache.GetOrCreateAsync(cacheKey, async () =>
                {
                    _logger.LogInformation($"Handling {nameof(GetBookByISBNQuery)} with data: {JsonConvert.SerializeObject(request)}");
                    
                    var selectedBook = await _bookQuery.GetByISBNAsync(request.isbn);

                    if (selectedBook == null)
                    {
                        _logger.LogWarning($"No book found with ISBN: {request.isbn}");
                        throw new KeyNotFoundException($"No book found with ISBN: {request.isbn}");
                    }

                    _logger.LogInformation($"Book found: {JsonConvert.SerializeObject(selectedBook)}");
                    return selectedBook;
                },TimeSpan.FromMinutes(10),cashTag);

                return cachedBooks;
            }
        }
    }
}
