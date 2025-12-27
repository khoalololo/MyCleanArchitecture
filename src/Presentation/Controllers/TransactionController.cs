using Application.Transactions.Commands.CreateTransaction;
using Application.Transactions.Queries.GetTransactions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id, message = "Transaction created successfully!" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _mediator.Send(new GetTransactionsQuery());
        return Ok(transactions);
    }
}