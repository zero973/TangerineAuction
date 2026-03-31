using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TangerineAuction.Core.UseCases.Tangerines;
using TangerineAuction.Server.Authorization;

namespace TangerineAuction.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TangerinesController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Сгенерировать мандарин
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    [Authorize(Policy = Policy.AddTangerinePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Generate(CancellationToken ct)
        => this.ToActionResult(await sender.Send(new GenerateTangerine.Command(), ct));
}