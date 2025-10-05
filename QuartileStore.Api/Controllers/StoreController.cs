using Microsoft.AspNetCore.Mvc;
using QuartileStore.Commons.Dtos.Stores;
using QuartileStore.Commons.Services.Contracts;

namespace QuartileStore.Api.Controllers;

[ApiController]
[Route("api/companies/{companyCode:int}/stores")]
public class StoreController(IStoreService storeService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(int companyCode, [FromBody] CreateStoreDto createStoreDto)
    {
        var newStore = await storeService.CreateAsync(companyCode, createStoreDto);

        return CreatedAtAction(
            nameof(GetStore),
            new { companyCode = newStore.CompanyCode, code = newStore.Code },
            newStore);
    }

    [HttpGet("{code:int}")]
    public async Task<IActionResult> GetStore(int code, int companyCode)
    {
        return Ok(await storeService.GetAsync(code, companyCode));
    }

    [HttpGet]
    public async Task<IActionResult> GetStores(int companyCode)
    {
        return Ok(await storeService.GetAllAsync(companyCode));
    }

    [HttpPut("{code:int}")]
    public async Task<IActionResult> Update(int code, int companyCode, [FromBody] UpdateStoreDto updateStoreDto)
    {
        return Ok(await storeService.UpdateAsync(code, companyCode, updateStoreDto));
    }

    [HttpDelete("{code:int}")]
    public async Task<IActionResult> Delete(int code, int companyCode)
    {
        await storeService.DeleteAsync(code, companyCode);
        return NoContent();
    }
}