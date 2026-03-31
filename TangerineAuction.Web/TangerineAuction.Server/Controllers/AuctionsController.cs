using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TangerineAuction.Core.UseCases.Auctions;
using TangerineAuction.Core.UseCases.Auctions.Dtos;

namespace TangerineAuction.Server.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuctionsController(ISender sender) : ControllerBase
{
    
    /// <summary>
    /// Получить аукционы с фильтрацией и пагинацией
    /// </summary>
    /// <param name="searchParams">Параметры поиска</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<AuctionResponse>>> GetAll([FromQuery] AuctionSearchParams searchParams, CancellationToken ct)
        => this.ToActionResult(await sender.Send(new GetAuctions.Query(searchParams), ct));
    
    /// <summary>
    /// Получить аукцион
    /// </summary>
    /// <param name="id">Id аукциона</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuctionFullResponse>> Get([FromQuery] Guid id, CancellationToken ct)
        => this.ToActionResult(await sender.Send(new GetAuction.Query(id), ct));
    
}