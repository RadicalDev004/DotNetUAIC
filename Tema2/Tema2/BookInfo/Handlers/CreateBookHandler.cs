namespace Tema2.BookInfo.Handlers;
using Tema2.BookInfo.Commands;
using Tema2.DB;
public sealed class CreateBookHandler(AppDbContext db) : IRequestHandler<CreateBookCommand, int>
{
    public async Task<int> Handle(CreateBookCommand req, CancellationToken ct)
    {
        var entity = Book.Create(req.Title, req.Author, req.Year);
        db.Books.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }
}