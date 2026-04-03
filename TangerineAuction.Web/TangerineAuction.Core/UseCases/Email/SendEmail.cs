using Ardalis.Result;
using FluentValidation;
using MediatR;

namespace TangerineAuction.Core.UseCases.Email;

public class SendEmail
{
    
    public record Command(string Receiver, string Topic, string Body, string? ImageUrl = null) : IRequest<Result>;
    
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

            RuleFor(x => x.ImageUrl)
                .Must(value => value == null || Uri.TryCreate(value, UriKind.Absolute, out var uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                .WithMessage("Ссылка на картинку должна быть валидной");
        }
    }
    
}