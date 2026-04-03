using FluentValidation;
using TangerineAuction.Core.UseCases.Tangerines.Dtos;

namespace TangerineAuction.Core.UseCases.Tangerines;

internal class TangerineRequestValidator : AbstractValidator<TangerineRequest>
{
    public TangerineRequestValidator()
    {
        RuleFor(x => x.Name)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Название мандарина не должно быть пустым")
            .Must(x => x.Length is > 3 and < 100)
            .WithMessage("Название мандарина должно быть больше 3 символов и меньше 100 символов");

        RuleFor(x => x.StartPrice)
            .InclusiveBetween(1, 1000000)
            .WithMessage("Начальная цена должна быть больше 0 и меньше 1 млн");

        RuleFor(x => x.FileName)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Название файла не должно быть пустым");
    }
}