using MediatR;
using TangerineAuction.Shared;

namespace TangerineGenerator.Core.Notifications;

public record  OnTangerineGeneratedNotification(TangerineInfo Tangerine) : INotification;

