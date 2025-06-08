using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Data;
using Core.Interfaces.Factories;
using Domain;

namespace Core.Services;

public class EventService(
    IEventRepository eventRepository,
    IRepositoryResultFactory resultFactory,
    IEventDtoFactory eventDtoFactory
) : IEventService
{
    /// <summary>
    /// Retrieves all events from the repository and converts them to display DTOs.
    /// Because we are filtering out null events, we can use the `e => e != null` predicate.
    /// and the rest of error handling is done in the repository.
    /// </summary>
    /// <returns></returns>
    public async Task<RepositoryResult<IEnumerable<EventDisplay>>> GetAllEventsAsync()
    {
        try
        {
            // Get all events from the repository
            var getEvents = await eventRepository.GetAllAsync(e => e != null);
            // Check if the repository operation itself failed
            var eventDisplayDTos = getEvents
                // Filter out any null events, if necessary
                .Where(e => e != null)
                .Select(e => eventDtoFactory.ToDisplay(e!))
                .ToList();
            // Return the result using the result factory
            return resultFactory.OperationSuccess<IEnumerable<EventDisplay>>(eventDisplayDTos, 200);
        }
        // Case 1: Repository operation itself failed (e.g., DB connection issue, invalid query).
        catch (Exception ex)
        {
            // Log the exception ex (recommended)
            return resultFactory.OperationFailed<IEnumerable<EventDisplay>>(
                new Error(
                    "EventService.GetAll.Exception",
                    $"An unexpected error occurred while retrieving all events: {ex.Message}"
                ),
                500 // Internal Server Error
            );
        }
    }

    /// <summary>
    /// Retrieves a specific event by its unique identifier (GUID).
    /// Handles various scenarios such as entity not found, repository errors, and unexpected states.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <returns>
    /// A <see cref="RepositoryResult{T}"/> containing the <see cref="EventDisplay"/> object
    /// or an error if the operation fails.
    /// </returns>
    public async Task<RepositoryResult<EventDisplay>> GetEventByGuid(Guid id)
    {
        try
        {
            // Get the event from the repository
            var eventResult = await eventRepository.GetAsync(
                e => e != null && e.Id == id,
                false,
                p => p.Packages
            );

            // Case 1: Repository operation itself failed (e.g., DB connection issue, invalid query).
            // eventResult.Error will be a specific error type, not Error.NonError.
            if (eventResult.Error != Error.NonError)
            {
                return resultFactory.OperationFailed<EventDisplay>(
                    eventResult.Error,
                    eventResult.StatusCode
                );
            }

            // Case 2: Entity was found.
            // BaseRepository.GetAsync returns Value != null and StatusCode = 200.
            // This means the event was successfully retrieved.
            if (eventResult.Value != null)
            {
                var displayEventDto = eventDtoFactory.ToDisplay(eventResult.Value);
                return resultFactory.OperationSuccess(displayEventDto, eventResult.StatusCode);
            }

            // Case 3: Entity was not found.
            if (eventResult.StatusCode == 404)
            {
                return resultFactory.OperationFailed<EventDisplay>(
                    Error.NotFound("Event is not found"),
                    404
                );
            }

            // Case 4: Unexpected state after retrieving event details.
            return resultFactory.OperationFailed<EventDisplay>(
                new Error(
                    "EventService.UnexpectedState",
                    "An unexpected state was encountered after retrieving event details."
                ),
                500 // Or eventResult.StatusCode if it's a known non-error, non-404 code
            );
        }
        catch (Exception)
        {
            // Log the exception ex (recommended)
            return resultFactory.OperationFailed<EventDisplay>(
                new Error(
                    "Event.RetrievalError",
                    "An unexpected error occurred while retrieving user details."
                ),
                500 // Internal Server Error
            );
        }
    }
}
