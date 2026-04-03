using MediatR;
using TangerineAuction.Core.UseCases.Tangerines;
using TangerineAuction.Shared.Hangfire;

namespace TangerineAuction.Core.Jobs;

internal class CreateTangerineJob(ISender sender) : IRecurringJob
{
    
    public string Name => "Задача генерации мандарина";
    
    public string Cron => "*/5 * * * *";
    
    public Task ExecuteAsync() => sender.Send(new GenerateTangerine.Command());
    
}