using Insurance.API.Contracts.Requests;
using Insurance.Application.Insuring.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("notifications")]
    public async Task<IActionResult> HandleNotification([FromBody] BookingNotificationRequest request)
    {
        var command = default(IRequest);

        if (request.Action == "confirmation")
        {
            command = new ConfirmBookingCommand(
                request.Reference,
                request.CheckIn,
                request.CheckOut,
                request.People);
        }
        else if(request.Action == "modification") { 
            command = new ModifyBookingCommand(
                request.Reference,
                request.CheckIn,
                request.CheckOut,
                request.People);
        }
        else if (request.Action == "cancellation")
        {
            command = new CancelBookingCommand(
                request.Reference);
        }
        else
            return BadRequest("Not supported action.");

        await _mediator.Send(command);

        return Ok();
    }
}