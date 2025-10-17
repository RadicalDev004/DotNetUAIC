namespace Tema2.BookInfo.Validation;
using Tema2.BookInfo.Commands;

public sealed class UpdateBookValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Author)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Year)
            .InclusiveBetween(1450, DateTime.UtcNow.Year + 1);
    }
}