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
    public IActionResult GetStore(int code, int companyCode)
    {
        return Ok(storeService.GetAsync(code, companyCode));
    }

    [HttpGet]
    public IActionResult GetAll(int companyCode)
    {
        return Ok(storeService.GetAllAsync(companyCode));
    }

    [HttpPut("{code:int}")]
    public IActionResult Update(int code, int companyCode, [FromBody] UpdateStoreDto updateStoreDto)
    {
        return Ok(storeService.UpdateAsync(code, companyCode, updateStoreDto));
    }

    [HttpDelete("{code:int}")]
    public async Task<IActionResult> Delete(int code, int companyCode)
    {
        await storeService.DeleteAsync(code, companyCode);
        return NoContent();
    }
}