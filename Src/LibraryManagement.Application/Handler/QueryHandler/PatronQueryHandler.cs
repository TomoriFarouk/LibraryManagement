using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interface.Query;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static LibraryManagement.Application.Queries.PatronQueries;

namespace LibraryManagement.Application.Handler.QueryHandler
{
    /// <summary>
    /// Handler for patron-related queries.
    /// </summary>
    public class PatronQueryHandler
    {
        /// <summary>
        /// Handler for retrieving all patrons.
        /// </summary>
        public class GetAllPatronHandler : IRequestHandler<GetAllPatronQuery, List<Patron>>
        {
            private readonly ILogger<GetAllPatronHandler> _logger;
            private readonly IPatronQuery _patronQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="GetAllPatronHandler"/> class.
            /// </summary>
            /// <param name="logger">The logger service.</param>
            /// <param name="patronQuery">The patron query service.</param>
            public GetAllPatronHandler(ILogger<GetAllPatronHandler> logger, IPatronQuery patronQuery, CacheManager cache)
            {
                _logger = logger;
                _patronQuery = patronQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<List<Patron>> Handle(GetAllPatronQuery request, CancellationToken cancellationToken)
            {
                var cacheKey = "Patron";
                var cacheTag = "Patron";
                var cachedPatron = await _cache.GetOrCreateAsync(cacheKey, async () =>
                {
                    _logger.LogInformation($"Handling {nameof(GetAllPatronQuery)} with data: {JsonConvert.SerializeObject(request)}");
                    var patrons = await _patronQuery.GetAllAsync();
                    _logger.LogInformation($"Patron found: {JsonConvert.SerializeObject(patrons)}");
                    return patrons.ToList();
                }, TimeSpan.FromMinutes(10), cacheTag);
                return cachedPatron;
            }

            /// <summary>
            /// Handler for retrieving a patron by their ID.
            /// </summary>
            public class GetPatronByIdHandler : IRequestHandler<GetPatronByIdQuery, Patron>
            {
                private readonly IMediator _mediator;
                private readonly ILogger<GetPatronByIdHandler> _logger;
                private readonly IPatronQuery _patronQuery;
                private readonly CacheManager _cache;

                /// <summary>
                /// Initializes a new instance of the <see cref="GetPatronByIdHandler"/> class.
                /// </summary>
                /// <param name="mediator">The mediator service.</param>
                /// <param name="logger">The logger service.</param>
                /// <param name="patronQuery">The patron query service.</param>
                public GetPatronByIdHandler(IMediator mediator, ILogger<GetPatronByIdHandler> logger, IPatronQuery patronQuery, CacheManager cache)
                {
                    _mediator = mediator;
                    _logger = logger;
                    _patronQuery = patronQuery;
                    _cache = cache;
                }

                /// <inheritdoc />
                public async Task<Patron> Handle(GetPatronByIdQuery request, CancellationToken cancellationToken)
                {
                    var cacheKey = $"Patron{request.Id}";
                    var cacheTag = $"Patron{request.Id}";
                    var cachedPatron = await _cache.GetOrCreateAsync(cacheKey, async () =>
                    {
                        _logger.LogInformation($"Handling {nameof(GetPatronByIdQuery)} with data: {JsonConvert.SerializeObject(request)}");

                        var selectedPatron = await _patronQuery.GetByIdAsync(request.Id);

                        if (selectedPatron == null)
                        {
                            _logger.LogWarning($"No patron found with ID: {request.Id}");
                            throw new KeyNotFoundException($"No patron found with ID: {request.Id}");
                        }

                        _logger.LogInformation($"Patron found: {JsonConvert.SerializeObject(selectedPatron)}");
                        return selectedPatron;
                    }, TimeSpan.FromMinutes(10), cacheTag);
                    return cachedPatron;
                }
            }
        }
    }
}