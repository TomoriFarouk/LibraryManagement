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
using static LibraryManagement.Application.Command.PatronCommand;

namespace LibraryManagement.Application.Handler.CommandHandler
{
	public class PatronCommandHandler
	{
        /// <summary>
        /// Handler for Patron-related commands.
        /// </summary>
        public class CreatePatronHandler : IRequestHandler<CreatePatronCommand, PatronResponse>
        {
            private readonly IPatronCommand _patronCommand;
            private readonly ILogger _logger;
            private readonly IPatronQuery _patronQuery;
            private readonly IIdentityService _identityService;
            private readonly CacheManager _cache;


            /// <summary>
            /// Initializes a new instance of the <see cref="CreateBookHandler"/> class.
            /// </summary>
            /// <param name="patronCommand">The book command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="patronQuery">The book query service.</param>
            /// <param name="identityService">The identity service.</param>


            public CreatePatronHandler(IPatronCommand patronCommand, ILogger logger, IPatronQuery patronQuery, IIdentityService identity,CacheManager cache)
            {
                _patronCommand = patronCommand;
                _logger = logger;
                _patronQuery = patronQuery;
                _identityService = identity;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<PatronResponse> Handle(CreatePatronCommand request, CancellationToken cancellationToken)
            {

                _logger.LogInformation($"Handling {nameof(CreatePatronCommand)} with data: {JsonConvert.SerializeObject(request)}");

                // Check if user exists
                var userExists = await _identityService.GetUserByIdAsync(request.UserId);
                if (userExists == null)
                {
                    _logger.LogError($"User with ID {request.UserId} does not exist.");
                    throw new ApplicationException($"User with ID {request.UserId} does not exist.");
                }

                // Map request to entity
                var taskEntity = LibraryManagementMapper.Mapper.Map<Patron>(request);

                if (taskEntity is null)
                {
                    _logger.LogError("Mapping failed for CreateBookCommand.");
                    throw new ApplicationException("There is a problem in mapper.");
                }
                // Add new patron
                taskEntity.applicationUser = userExists;
                var newPatron = await _patronCommand.AddAsync(taskEntity);
                var patronResponse = LibraryManagementMapper.Mapper.Map<PatronResponse>(newPatron);
                _logger.LogInformation($"Book created successfully with ISBN: {patronResponse.Id}");

                // Invalidate relevant caches
                _cache.InvalidateByTag("Patron");
                
                return patronResponse;
            }
        }

        /// <summary>
        /// Handler for editing an existing book.
        /// </summary>
        /// 
        public class EditPatronHandler : IRequestHandler<EditPatronCommand, PatronResponse>
        {
            private readonly IPatronCommand _patronCommand;
            private readonly ILogger _logger;
            private readonly IPatronQuery _patronQuery;
            private readonly CacheManager _cache;

            /// <summary>
            /// Initializes a new instance of the <see cref="CreateBookHandler"/> class.
            /// </summary>
            /// <param name="patronCommand">The book command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="patronQuery">The book query service.</param>

            public EditPatronHandler(IPatronCommand patronCommand, ILogger logger, IPatronQuery patronQuery,CacheManager cache)

            {
                _patronCommand = patronCommand;
                _logger = logger;
                _patronQuery = patronQuery;
                _cache = cache;
            }

            /// <inheritdoc />

            public async Task<PatronResponse> Handle(EditPatronCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(EditBookCommand)} with data: {JsonConvert.SerializeObject(request)}");

                // Map request to entity
                var patronEntity = LibraryManagementMapper.Mapper.Map<Patron>(request);
                
                if (patronEntity is null)
                {
                    _logger.LogError("Mapping failed for EditPatronCommand.");
                    throw new ApplicationException("There is a problem in mapper.");
                }

               
                patronEntity.LastModified = DateTime.Now;
                // Update book
                try
                {
                    await _patronCommand.UpdateAsync(patronEntity);
                }
                catch (Exception exp)
                {
                    _logger.LogError($"Error updating patron: {exp.Message}");
                    throw new ApplicationException(exp.Message);
                }

                //Get Updated patron
                var patron = await _patronQuery.GetAllAsync();
                var modifiedPatron = patron.FirstOrDefault(x => x.Id == request.Id);
                var patronResponse = LibraryManagementMapper.Mapper.Map<PatronResponse>(modifiedPatron);
                _logger.LogInformation($"Book updated successfully with ISBN: {patronResponse?.Id}");

                // Invalidate the cache for this specific book
                var cacheTag = $"Patron{request.Id}";
                _cache.InvalidateByTag(cacheTag);
                return patronResponse;
            }
        }

        /// <summary>
        /// Handler for deleting a book.
        /// </summary>
        public class DeletePatronHandler : IRequestHandler<DeletePatronCommand, String>
        {
            private readonly IPatronCommand _patronCommand;
            private readonly ILogger _logger;
            private readonly IPatronQuery _patronQuery;
            private readonly CacheManager _cache;


            /// <summary>
            /// Initializes a new instance of the <see cref="DeleteBookHandler"/> class.
            /// </summary>
            /// <param name="patronCommand">The book command service.</param>
            /// <param name="logger">The logger service.</param>
            /// <param name="patronQuery">The book query service.</param>
            /// 
            public DeletePatronHandler(IPatronCommand patronCommand, ILogger logger, IPatronQuery patronQuery,CacheManager cache)
            {
                _patronCommand = patronCommand;
                _logger = logger;
                _patronQuery = patronQuery;
                _cache = cache;
            }

            /// <inheritdoc />
            public async Task<string> Handle(DeletePatronCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Handling {nameof(DeletePatronCommand)} with data: {JsonConvert.SerializeObject(request)}");

                // Delete book
                try
                {
                    var patronEntity = await _patronQuery.GetByIdAsync(request.Id);
                    if (patronEntity == null)
                    {
                        _logger.LogError($"Patron with ID {request.Id} not found.");
                        throw new ApplicationException($"Patron with ID {request.Id} not found.");
                    }

                    await _patronCommand.DeleteAsync(patronEntity);
                }
                catch (Exception exp)
                {
                    _logger.LogError($"Error deleting patron: {exp.Message}");
                    throw new ApplicationException(exp.Message);
                }
                _logger.LogInformation($"Patron with ID {request.Id} deleted successfully.");

                // Invalidate the cache for this specific book
                var cacheTag = $"Patron{request.Id}";
                _cache.InvalidateByTag(cacheTag);

                return $"Patron with ID {request.Id} has been deleted!";
            }
        }
    }
}

