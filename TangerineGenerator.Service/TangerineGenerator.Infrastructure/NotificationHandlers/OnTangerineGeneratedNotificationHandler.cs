using MassTransit;
using MediatR;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Notifications;
using TangerineGenerator.Infrastructure.Data;

namespace TangerineGenerator.Infrastructure.NotificationHandlers;

internal class OnTangerineGeneratedNotificationHandler(IPublishEndpoint publishEndpoint, AppDbContext dbContext) 
    : INotificationHandler<OnTangerineGeneratedNotification>
{
    public async Task Handle(OnTangerineGeneratedNotification notification, CancellationToken ct)
    {
        await publishEndpoint.Publish(new OnTangerineCreatedRequest { TangerineInfo = notification.Tangerine}, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}