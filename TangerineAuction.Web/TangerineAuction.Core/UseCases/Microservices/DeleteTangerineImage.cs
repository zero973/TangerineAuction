using FluentValidation;
using MediatR;

namespace TangerineAuction.Core.UseCases.Microservices;

public class DeleteTangerineImage
{
    
    public record Command(string FileName) : IRequest;
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FileName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Название файла не должно быть пустым");
        }
    }
    
}