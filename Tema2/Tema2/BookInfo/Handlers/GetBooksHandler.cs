namespace Tema2.BookInfo.Handlers;
using Tema2.BookInfo.Queries;
using Tema2.DB;
public sealed class GetBooksHandler(AppDbContext db) : IRequestHandler<GetBooksQuery, PagedResult<BookDto>>
{
    public async Task<PagedResult<BookDto>> Handle(GetBooksQuery req, CancellationToken ct)
    {
        var query = db.Books.AsNoTracking();

        var total = await query.CountAsync(ct);
        
        var items = await query
            .OrderBy(b => b.Id)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Year))
            .ToListAsync(ct);

        return new PagedResult<BookDto>(items, req.Page, req.PageSize, total);
    }
}