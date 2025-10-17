namespace Tema2.BookInfo;

public sealed class Book
{
    public int Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public int Year { get; private set; }
    
    public static Book Create(string title, string author, int year)
        => new Book { Title = title, Author = author, Year = year };

    public void Update(string title, string author, int year)
    {
        Title = title; Author = author; Year = year;
    }
}