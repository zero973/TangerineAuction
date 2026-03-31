using FluentValidation;
using TangerineAuction.Core.UseCases.Auctions.Dtos;

namespace TangerineAuction.Core.UseCases.Auctions.Validators;

internal class AuctionRequestValidator : AbstractValidator<AuctionRequest>
{
    public AuctionRequestValidator()
    {
        RuleFor(x => x.Name)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Название аукциона не должно быть пустым")
            .Must(x => x.Length is > 3 and < 100)
            .WithMessage("Название аукциона должно быть больше 3 символов и меньше 100 символов");
    }
}