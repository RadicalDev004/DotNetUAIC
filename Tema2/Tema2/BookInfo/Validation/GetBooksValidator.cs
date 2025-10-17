namespace Tema2.BookInfo.Validation;
using Tema2.BookInfo.Queries;

public sealed class GetBooksValidator : AbstractValidator<GetBooksQuery>
{
    public GetBooksValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}