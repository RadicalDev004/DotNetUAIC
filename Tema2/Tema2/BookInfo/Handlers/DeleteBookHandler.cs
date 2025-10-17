namespace Tema2.BookInfo.Handlers;
using Tema2.BookInfo.Commands;
using Tema2.DB;
public sealed class DeleteBookHandler(AppDbContext db) : IRequestHandler<DeleteBookCommand>
{
    public async Task Handle(DeleteBookCommand req, CancellationToken ct)
    {
        var entity = await db.Books.FindAsync([req.Id], ct);
        if (entity is null) return;
        db.Books.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}