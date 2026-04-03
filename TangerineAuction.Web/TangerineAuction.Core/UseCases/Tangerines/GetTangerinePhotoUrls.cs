using Ardalis.Result;
using FluentValidation;
using MediatR;

namespace TangerineAuction.Core.UseCases.Tangerines;

public class GetTangerinePhotoUrls
{
    
    public record Query(List<string> TangerineFileNames) : IRequest<Result<Dictionary<string, string>>>;
    
    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.TangerineFileNames)
                .NotEmpty()
                .WithMessage("Список названий файлов мандаринок не должен быть пустым");
        }
    }
    
}