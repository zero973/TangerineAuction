using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TangerineAuction.Core.UseCases.Auctions;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Bets;
using TangerineAuction.Core.UseCases.Bets.Dtos;

namespace TangerineAuction.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BetsController(ISender sender) : ControllerBase
{
    
    /// <summary>
    /// Получить последнюю ставку на аукционе
    /// </summary>
    /// <param name="id">Id аукциона</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BetResponse>> GetLastBet([FromQuery] Guid id, CancellationToken ct)
        => this.ToActionResult(await sender.Send(new GetLastAuctionBet.Query(id), ct));
    
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
    
    /// <summary>
    /// Выкупить лот
    /// </summary>
    /// <param name="request">Id аукциона</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> BuyTangerine([FromBody] BuyTangerineRequest request, CancellationToken ct)
        => this.ToActionResult(await sender.Send(new BuyTangerine.Command(request), ct));
    
}