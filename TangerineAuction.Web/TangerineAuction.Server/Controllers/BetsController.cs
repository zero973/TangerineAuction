using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TangerineAuction.Core.UseCases.Bets;
using TangerineAuction.Core.UseCases.Bets.Dtos;

namespace TangerineAuction.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BetsController(ISender sender) : ControllerBase
{
    
    /// <summary>
    /// Может ли пользователь сделать ставку
    /// </summary>
    /// <param name="auctionId">Id аукциона</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> CanCreate([FromQuery] Guid auctionId, CancellationToken ct)
        => this.ToActionResult(await sender.Send(new CanCreateBet.Query(auctionId), ct));
    
    /// <summary>
    /// Сделать ставку
    /// </summary>
    /// <param name="request">Ставка</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BetResponse>> Add([FromBody] BetRequest request, CancellationToken ct)
        => this.ToActionResult(await sender.Send(new AddBet.Command(request), ct));
    
}