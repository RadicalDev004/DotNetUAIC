using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChecklistExercise.Application.Features.Orders;
using ChecklistExercise.Domain.Entities.Orders;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChecklistExercise.Application.Validators
{
    public sealed class CreateOrderProfileValidator : AbstractValidator<CreateOrderProfileRequest>
    {
        private readonly ApplicationContext _db;
        private readonly ILogger<CreateOrderProfileValidator> _logger;


        private static readonly string[] InappropriateWords =
        {
            "abuse","xxx","nsfw","violent","obscene"
        };

        private static readonly string[] TechnicalKeywords =
        {
            "c#", "dotnet", ".net", "java", "python", "kubernetes", "docker", "cloud", "microservices",
            "algorithms","data structures","sql","nosql","devops","distributed","react","angular"
        };

        private static readonly string[] ChildrenRestrictedWords =
        {
            "violent","gore","explicit","adult","nsfw"
        };

        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public CreateOrderProfileValidator(ApplicationContext db, ILogger<CreateOrderProfileValidator> logger)
        {
            _db = db;
            _logger = logger;

            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(1).WithMessage("Title must be at least 1 character.")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters.")
                .Must(BeValidTitle).WithMessage("Title contains inappropriate content.")
                .MustAsync(BeUniqueTitle).WithMessage("An order with the same Title for this Author already exists.");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MinimumLength(2).WithMessage("Author must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Author must be at most 100 characters.")
                .Must(BeValidAuthorName).WithMessage("Author contains invalid characters. Allowed: letters, spaces, hyphens, apostrophes, dots.");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Must(BeValidISBN).WithMessage("ISBN must be 10 or 13 digits (hyphens/spaces allowed).")
                .MustAsync(BeUniqueISBN).WithMessage("An order with this ISBN already exists.");

            RuleFor(x => x.Category)
                .Must(cat => Enum.IsDefined(typeof(OrderCategory), cat))
                .WithMessage("Category is invalid.");

            RuleFor(x => x.Price)
                .GreaterThan(0m).WithMessage("Price must be greater than 0.")
                .LessThan(10_000m).WithMessage("Price must be less than $10,000.");

            RuleFor(x => x.PublishedDate)
                .Must(d => d <= DateTime.UtcNow).WithMessage("Published date cannot be in the future.")
                .Must(d => d.Year >= 1400).WithMessage("Published date cannot be before year 1400.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.")
                .LessThanOrEqualTo(100_000).WithMessage("Stock quantity cannot exceed 100,000.");

            When(x => !string.IsNullOrWhiteSpace(x.CoverImageUrl), () =>
            {
                RuleFor(x => x.CoverImageUrl!)
                    .Must(BeValidImageUrl).WithMessage("CoverImageUrl must be an HTTP/HTTPS image URL ending with .jpg, .jpeg, .png, .gif, or .webp.");
            });

        }


        private bool BeValidTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return false;
            var t = title.ToLowerInvariant();
            var hit = InappropriateWords.FirstOrDefault(w => t.Contains(w));
            if (hit is not null)
            {
                _logger.LogInformation("Title failed inappropriate content check on word '{Word}'", hit);
                return false;
            }
            return true;
        }

        private async Task<bool> BeUniqueTitle(CreateOrderProfileRequest req, string title, ValidationContext<CreateOrderProfileRequest> ctx, CancellationToken ct)
        {
            var exists = await _db.Orders.AsNoTracking()
                .AnyAsync(o => o.Title.ToLower() == title.ToLower()
                            && o.Author.ToLower() == req.Author.ToLower(), ct);

            _logger.LogDebug("BeUniqueTitle check for '{Title}' by '{Author}': {Exists}", title, req.Author, exists);
            return !exists;
        }

        private bool BeValidAuthorName(string author)
        {
            if (string.IsNullOrWhiteSpace(author)) return false;
            var ok = Regex.IsMatch(author, @"^[\p{L}\s\-\.'’]+$");
            return ok;
        }

        private bool BeValidISBN(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return false;
            var normalized = new string(isbn.Where(ch => ch != '-' && ch != ' ').ToArray());
            if (!normalized.All(char.IsDigit)) return false;
            return normalized.Length is 10 or 13;
        }

        private async Task<bool> BeUniqueISBN(string isbn, CancellationToken ct)
        {
            var exists = await _db.Orders.AsNoTracking().AnyAsync(o => o.ISBN == isbn, ct);
            _logger.LogDebug("BeUniqueISBN check for '{ISBN}': {Exists}", isbn, exists);
            return !exists;
        }

        private bool BeValidImageUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return false;
            var path = uri.AbsolutePath.ToLowerInvariant();
            return AllowedImageExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}
