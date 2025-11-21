namespace ChecklistExercise.Application.Common.Logging;

public static class LoggingExtensions
{
    public static void LogOrderCreationMetrics(
        this ILogger logger,
        OrderCreationMetrics metrics)
    {
        logger.LogInformation(
            new EventId(LogEvents.OrderCreationCompleted, nameof(LogEvents.OrderCreationCompleted)),
            "Order creation metrics {@Metrics}",
            metrics
        );
    }

    public static void LogOrderEvent(
        this ILogger logger,
        int eventId,
        LogLevel level,
        string message,
        object? state = null,
        Exception? ex = null)
    {
        var evt = new EventId(eventId);
        if (ex is not null)
            logger.Log(level, evt, ex, message + " {@State}", state);
        else
            logger.Log(level, evt, message + " {@State}", state);
    }
}