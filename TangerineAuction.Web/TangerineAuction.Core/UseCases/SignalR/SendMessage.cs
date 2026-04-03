using Ardalis.Result;
using FluentValidation;
using MediatR;
using TangerineAuction.Core.Enums;

namespace TangerineAuction.Core.UseCases.SignalR;

public class SendMessage
{
    
    public record Command(HubMessageType Type, Guid EntityId) : IRequest<Result>;
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.EntityId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id сущности не должно быть пустым");
        }
    }
    
}