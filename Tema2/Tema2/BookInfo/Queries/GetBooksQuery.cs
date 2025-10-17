namespace Tema2.BookInfo.Queries;
using Tema2.DB;
public sealed record GetBooksQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<BookDto>>;