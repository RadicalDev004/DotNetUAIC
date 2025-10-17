namespace Tema2.BookInfo.Commands;

public sealed record DeleteBookCommand(int Id) : IRequest;