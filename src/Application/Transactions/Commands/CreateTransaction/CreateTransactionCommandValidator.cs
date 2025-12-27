using Application.Transactions.Commands.CreateTransaction;
using FluentValidation;

namespace Application.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(v => v.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(v => v.CategoryId)
            .NotEmpty().WithMessage("Category is required.");
    }
}
