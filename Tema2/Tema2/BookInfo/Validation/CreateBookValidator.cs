namespace Tema2.BookInfo.Validation;
using Tema2.BookInfo.Commands;

public sealed class CreateBookValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Year).InclusiveBetween(1450, DateTime.UtcNow.Year + 1);
    }
}