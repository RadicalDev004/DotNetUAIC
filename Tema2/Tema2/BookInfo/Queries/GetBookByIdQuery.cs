namespace Tema2.BookInfo.Queries;
using Tema2.DB;
public sealed record GetBookByIdQuery(int Id) : IRequest<BookDto?>;