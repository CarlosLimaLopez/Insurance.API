using Insurance.Application.Reporting.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("insured-bookings")]
    public async Task<ActionResult<IReadOnlyList<InsuredBookingDto>>> GetInsuredBookings()
    {
        var result = await _mediator.Send(new GetInsuredBookingsQuery());

        return Ok(result);
    }

    [HttpGet("cancelled-bookings")]
    public async Task<ActionResult<IReadOnlyList<CancelledBookingDto>>> GetCancelledBookings()
    {
        var result = await _mediator.Send(new GetCancelledBookingsQuery());

        return Ok(result);
    }
}