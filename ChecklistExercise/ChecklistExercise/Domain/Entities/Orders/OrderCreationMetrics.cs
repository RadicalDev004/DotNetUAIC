using ChecklistExercise.Domain.Entities.Orders;

public sealed record OrderCreationMetrics(
    string OperationId,
    string OrderTitle,
    string ISBN,
    OrderCategory Category,
    TimeSpan ValidationDuration,
    TimeSpan DatabaseSaveDuration,
    TimeSpan TotalDuration,
    bool Success,
    string? ErrorReason
);