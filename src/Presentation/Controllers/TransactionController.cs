using Application.Transactions.Commands.CreateTransaction;
using Application.Transactions.Queries.GetTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    // 1. Controller gets injected with Mediator
    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionCommand command)
    {
        // 2. Command is sent to Mediator
        // 3. MediatR asks DI: "Who handles CreateTransactionCommand?"
        // 4. DI says: "CreateTransactionCommandHandler"
        // 5. MediatR asks DI: "Create me a CreateTransactionCommandHandler"
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