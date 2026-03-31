using FluentValidation;
using MediatR;

namespace TangerineAuction.Core.UseCases.Microservices;

public class DeleteTangerineImage
{
    public record Command(string FilePath) : IRequest;
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FilePath)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Путь к файлу не должен быть пустым");
        }
    }
}