using Ardalis.Specification;
using TangerineAuction.Core.Models;

namespace TangerineAuction.Core.UseCases.Tangerines;

internal class TangerineSpecification : Specification<Tangerine>
{
    public TangerineSpecification WithIds(Guid[] topicIds)
    {
        // ReSharper disable once CSharp14OverloadResolutionWithSpanBreakingChange
        Query.Where(x => topicIds.Contains(x.Id));
        return this;
    }
}