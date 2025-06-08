using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using VoidMail.Helpers;

namespace VoidMail.Controllers;

/// <summary>
/// EventController
/// </summary>
[ApiController]
[Route("/[controller]")]
public class EventController(IEventService eventService, IWebHostEnvironment webHostEnvironment)
    : ControllerBase
{
    /// <summary>
    /// Get Event by Guid
    /// /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    [HttpGet("{guid:Guid}", Name = "GetEventByGuid")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetEventByGuid([FromRoute] Guid guid)
    {
        try
        {
            // Validate the Guid
            if (guid == Guid.Empty)
            {
                return ApiResponseHelper.BadRequest("Invalid Guid provided.");
            }

            // Get the status from the database
            var status = await eventService.GetEventByGuid(guid);
            // Return the status
            return ApiResponseHelper.Success(status);
        }
        catch (Exception ex)
        {
            // Return a problem response, in development mode, it will include the stack trace
            return ApiResponseHelper.Problem(ex, webHostEnvironment.IsDevelopment());
        }
    }

    /// <summary>
    /// Get All Event
    /// /// </summary>
    /// <returns></returns>
    [HttpGet, Route("GetAllEvents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetAllEventsAsync()
    {
        try
        {
            // Get the Events from the database
            var eventsAsync = await eventService.GetAllEventsAsync();
            // Return the events
            return eventsAsync.Value != null && eventsAsync.Value.Any()
                ? ApiResponseHelper.Success(eventsAsync)
                : ApiResponseHelper.NotFound("No Events found");
        }
        catch (Exception ex)
        {
            // Return a problem response, in development mode, it will include the stack trace
            return ApiResponseHelper.Problem(ex, webHostEnvironment.IsDevelopment());
        }
    }
}
