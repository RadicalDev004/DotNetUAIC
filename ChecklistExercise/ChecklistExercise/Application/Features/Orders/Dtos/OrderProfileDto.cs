namespace ChecklistExercise.Application.Features.Orders.Dtos;

public class OrderProfileDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public string CategoryDisplayName { get; set; }
    public decimal Price { get; set; }
    public string FormattedPrice;
    public DateTime PublishedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }
    
    public string PublishedAge => GetPublishedAge();
    public string AuthorInitials => GetAuthorInitials();
    public string AvailabilityStatus => IsAvailable ? "In Stock" : "Out of Stock";

    private string GetPublishedAge()
    {
        var years = DateTime.UtcNow.Year - PublishedDate.Year;
        if (PublishedDate > DateTime.UtcNow.AddYears(-years)) years--;
        return years <= 0 ? "New" : $"{years} year{(years > 1 ? "s" : "")} ago";
    }

    private string GetAuthorInitials()
    {
        if (string.IsNullOrWhiteSpace(Author)) return string.Empty;

        var parts = Author.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join("", Array.ConvertAll(parts, p => p[0].ToString().ToUpper()));
    }
}