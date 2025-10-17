namespace Tema2.BookInfo.Handlers;
using Tema2.BookInfo.Commands;
using Tema2.DB;
public sealed class UpdateBookHandler(AppDbContext db) : IRequestHandler<UpdateBookCommand>
{
    public async Task Handle(UpdateBookCommand req, CancellationToken ct)
    {
        var entity = await db.Books.FindAsync([req.Id], ct);
        if (entity is null) throw new KeyNotFoundException($"Book {req.Id} not found");
        entity.Update(req.Title, req.Author, req.Year);
        await db.SaveChangesAsync(ct);
    }
}