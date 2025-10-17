namespace Tema2.BookInfo.Commands;



public sealed record CreateBookCommand(string Title, string Author, int Year) : IRequest<int>;