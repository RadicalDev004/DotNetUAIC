﻿namespace ChecklistExercise.Models;

public class Order
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public OrderCategory Category { get; set; }
    public decimal Price { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsAvailable => StockQuantity > 0;
    public int StockQuantity { get; set; } = 0;
}