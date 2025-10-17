namespace Tema2.BookInfo.Commands;

public sealed record UpdateBookCommand(int Id, string Title, string Author, int Year) : IRequest;