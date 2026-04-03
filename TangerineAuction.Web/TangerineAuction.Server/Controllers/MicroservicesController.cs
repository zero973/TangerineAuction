using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TangerineAuction.Core.UseCases.Microservices;
using TangerineAuction.Server.Authorization;
using TangerineAuction.Shared;
using TangerineAuction.Shared.Models;

namespace TangerineAuction.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
internal class MicroservicesController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Получить версию сервиса генератора мандаринок
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    [Authorize(Policy = Policy.AddTangerinePolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VersionInfo>> Add(CancellationToken ct)
        => this.ToActionResult(await sender.Send(new GetTangerineGeneratorServiceVersion.Query(), ct));
}