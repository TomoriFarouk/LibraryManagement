using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.Query;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static LibraryManagement.Application.Queries.BookQueries;
using static LibraryManagement.Application.Queries.BorrowingRecordQueries;

namespace LibraryManagement.Application.Handler.QueryHandler
{
    public class BorrowingRecordQueriesHandler
    {
        /// <summary>
        /// Handler for book-related queries.
        /// </summary>
        /// 
        public class GetAllBorrowingRecordHandler : IRequestHandler<GetAllBorrowingRecordQuery, List<BorrowingRecord>>
        {

            private readonly ILogger _logger;
            private readonly IBorrowingRecordQuery _borrowingRecordQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="GetAllBookHandler"/> class.
            /// </summary>
            /// <param name="logger">The logger service.</param>
            /// <param name="borrowingRecordQuery">The book query service.</param>
            public GetAllBorrowingRecordHandler(ILogger logger, IBorrowingRecordQuery borrowingRecordQuery, CacheManager cache)
            {
                _logger = logger;
                _borrowingRecordQuery = borrowingRecordQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<List<BorrowingRecord>> Handle(GetAllBorrowingRecordQuery request, CancellationToken cancellationToken)
            {
                var cacheKey = "BorrowingRecord";
                var cacheTag = "BorrowingRecord";
                var cachedRecord = await _cache.GetOrCreateAsync(cacheKey, async () =>
                {
                    _logger.LogInformation($"Handling {nameof(GetAllBookQuery)} with data: {JsonConvert.SerializeObject(request)}");
                    var borrowingRecords = await _borrowingRecordQuery.GetAllAsync();
                    _logger.LogInformation($"Borrowing Record found: {JsonConvert.SerializeObject(borrowingRecords)}");
                    return borrowingRecords.ToList();
                }, TimeSpan.FromMinutes(10), cacheTag);
                return cachedRecord;
            }

            /// <summary>
            /// Handler for retrieving a book by its ID.
            /// </summary>
            public class GetBorrowingRecordByIdHandler : IRequestHandler<GetBorrowingRecordByIdQuery, BorrowingRecord>
            {
                private readonly IMediator _mediator;
                private readonly ILogger _logger;
                private readonly IBorrowingRecordQuery _borrowingRecordQuery;
                private readonly CacheManager _cache;

                /// <summary>
                /// Initializes a new instance of the <see cref="GetBookByIdHandler"/> class.
                /// </summary>
                /// <param name="mediator">The mediator service.</param>
                /// <param name="logger">The logger service.</param>
                /// <param name="bookQuery">The book query service.</param>

                public GetBorrowingRecordByIdHandler(IMediator mediator, ILogger logger, IBorrowingRecordQuery borrowingRecordQuery, CacheManager cache)
                {
                    _mediator = mediator;
                    _logger = logger;
                    _borrowingRecordQuery = borrowingRecordQuery;
                    _cache = cache;
                }

                /// <inheritdoc />
                public async Task<BorrowingRecord> Handle(GetBorrowingRecordByIdQuery request, CancellationToken cancellationToken)
                {
                    var cacheKey = $"BorrowingRecord{request.Id}";
                    var cacheTag = $"BorrowingRecord{request.Id}";

                    var cachedRecord = await _cache.GetOrCreateAsync(cacheKey, async () =>
                    {
                        _logger.LogInformation($"Handling {nameof(GetBorrowingRecordByIdQuery)} with data: {JsonConvert.SerializeObject(request)}");
                        
                        var selectedBorrowingRecord = await _borrowingRecordQuery.GetByIdAsync(request.Id);

                        if (selectedBorrowingRecord == null)
                        {
                            _logger.LogWarning($"No borrowingRecord found with ID: {request.Id}");
                            throw new KeyNotFoundException($"No borrowingRecord found with ID: {request.Id}");
                        }

                        _logger.LogInformation($"Borrowing Record found: {JsonConvert.SerializeObject(selectedBorrowingRecord)}");
                        return selectedBorrowingRecord;

                    }, TimeSpan.FromMinutes(10), cacheTag);
                    return cachedRecord;
                }
            }
        }
    }
}