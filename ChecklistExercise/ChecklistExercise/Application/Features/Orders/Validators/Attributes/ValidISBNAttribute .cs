using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ChecklistExercise.Application.Features.Orders.Validators.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ValidISBNAttribute : ValidationAttribute, IClientModelValidator
{
    public ValidISBNAttribute()
    {
        ErrorMessage = "ISBN must be 10 or 13 digits (hyphens/spaces allowed).";
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;

        var s = value.ToString() ?? string.Empty;
        var normalized = new string(s.Where(ch => ch != '-' && ch != ' ').ToArray());
        if (normalized.All(char.IsDigit) && (normalized.Length == 10 || normalized.Length == 13))
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }

    // Client-side adapter
    public void AddValidation(ClientModelValidationContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        MergeAttribute(context, "data-val", "true");
        MergeAttribute(context, "data-val-validisbn", ErrorMessage ?? "Invalid ISBN.");
    }

    private static void MergeAttribute(ClientModelValidationContext context, string key, string value)
    {
        if (!context.Attributes.ContainsKey(key))
            context.Attributes.Add(key, value);
    }
}

