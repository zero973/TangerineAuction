using MassTransit;
using MediatR;
using TangerineGenerator.Core.Commands;
using TangerineGenerator.Shared;

namespace TangerineGenerator.Infrastructure.Consumers;

internal class GenerateTangerineRequestConsumer(ISender sender) : IConsumer<GenerateTangerineRequest>
{
    public Task Consume(ConsumeContext<GenerateTangerineRequest> context) 
        => sender.Send(new CreateTangerine.Command(), context.CancellationToken);
}