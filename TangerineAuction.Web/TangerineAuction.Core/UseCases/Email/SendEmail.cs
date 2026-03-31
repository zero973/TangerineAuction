using Ardalis.Result;
using FluentValidation;
using MediatR;

namespace TangerineAuction.Core.UseCases.Email;

public class SendEmail
{
    
    public record Command(string Receiver, string Topic, string Body, string? TangerineFilePath = null) : IRequest<Result>;
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Receiver)
                .NotEmpty()
                .WithMessage("Получатель не должен быть пустым")
                .EmailAddress()
                .WithMessage("Указан неправильный email")
                .MaximumLength(256);

            RuleFor(x => x.Topic)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Тема сообщения не должна быть пустой")
                .MaximumLength(200);

            RuleFor(x => x.Body)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Сообщение не должно быть пустым")
                .MaximumLength(5000);

            RuleFor(x => x.TangerineFilePath)
                .Must(x => x == null || File.Exists(x))
                .WithMessage("Не найден файл с мандаринкой");
        }
    }
    
}