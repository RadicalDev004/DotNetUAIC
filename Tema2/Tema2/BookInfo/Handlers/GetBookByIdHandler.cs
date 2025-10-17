namespace Tema2.BookInfo.Handlers;
using Tema2.BookInfo.Commands;
using Tema2.BookInfo.Queries;
using Tema2.DB;
public sealed class GetBookByIdHandler(AppDbContext db) : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    public async Task<BookDto?> Handle(GetBookByIdQuery req, CancellationToken ct) =>
        await db.Books.AsNoTracking()
            .Where(b => b.Id == req.Id)
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Year))
            .SingleOrDefaultAsync(ct);
}
