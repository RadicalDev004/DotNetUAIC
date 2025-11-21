using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ChecklistExercise.Domain.Entities.Orders;

namespace ChecklistExercise.Application.Features.Orders.Validators.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class OrderCategoryAttribute : ValidationAttribute
{
    private readonly OrderCategory[] _allowed;

    public OrderCategoryAttribute(params OrderCategory[] allowed)
    {
        _allowed = allowed ?? Array.Empty<OrderCategory>();
    }

    public override string FormatErrorMessage(string name)
    {
        var list = _allowed.Any()
            ? string.Join(", ", _allowed.Select(c => c.ToString()))
            : "none";
        return $"Category '{name}' must be one of: {list}.";
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is null) return new ValidationResult(FormatErrorMessage(validationContext.MemberName ?? "Category"));

        if (value is OrderCategory cat && _allowed.Contains(cat))
            return ValidationResult.Success;

        return new ValidationResult(FormatErrorMessage(validationContext.MemberName ?? "Category"));
    }
}

