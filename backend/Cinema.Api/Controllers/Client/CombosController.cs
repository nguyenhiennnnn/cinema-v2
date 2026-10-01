using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Client;

[ApiController]
[Route("api/v1/combos")]
public class CombosController(IComboService comboService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCombos(CancellationToken cancellationToken) =>
        Ok(await comboService.GetActiveCombosAsync(cancellationToken));
}
