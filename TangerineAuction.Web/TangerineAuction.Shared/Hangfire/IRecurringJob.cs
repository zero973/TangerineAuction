namespace TangerineAuction.Shared.Hangfire;

/// <summary>
/// Периодическая джоба
/// </summary>
public interface IRecurringJob
{
    
    /// <summary>
    /// Название джобы
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Расписание запуска
    /// </summary>
    string Cron { get; }
    
    /// <summary>
    /// Тело джобы
    /// </summary>
    Task ExecuteAsync();
    
}