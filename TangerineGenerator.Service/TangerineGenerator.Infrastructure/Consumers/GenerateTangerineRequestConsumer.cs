using MassTransit;
using MediatR;
using TangerineGenerator.Core.Commands;
using TangerineGenerator.Shared;

namespace TangerineGenerator.Infrastructure.Consumers;

internal class GenerateTangerineRequestConsumer(ISender sender) : IConsumer<GenerateTangerineRequest>
{
    public async Task Consume(ConsumeContext<GenerateTangerineRequest> context)
    {
        var tangerine = await sender.Send(new CreateTangerine.Command(), context.CancellationToken);
        await context.RespondAsync(tangerine);
    }
}