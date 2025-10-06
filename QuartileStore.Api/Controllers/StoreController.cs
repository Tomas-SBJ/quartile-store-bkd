using Microsoft.AspNetCore.Mvc;
using QuartileStore.Commons.Dtos.Errors;
using QuartileStore.Commons.Dtos.Stores;
using QuartileStore.Commons.Services.Contracts;

namespace QuartileStore.Api.Controllers;

/// <summary>
/// Contains endpoints for managing stores within a specific company.
/// </summary>
[ApiController]
[Route("api/companies/{companyCode:int}/stores")]
public class StoreController(IStoreService storeService) : ControllerBase
{
    /// <summary>
    /// Creates a new store for a specific company.
    /// </summary>
    /// <param name="companyCode">The business code of the parent company.</param>
    /// <param name="storeCreateDto">The data for the new store.</param>
    /// <returns>The newly created store.</returns>
    /// <response code="201">Returns the newly created store.</response>
    /// <response code="400">If the request payload is invalid.</response>
    /// <response code="404">If the parent company is not found.</response>
    /// <response code="409">If a store with the same code already exists for this company.</response>
    [HttpPost]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(int companyCode, [FromBody] StoreCreateDto storeCreateDto)
    {
        var newStore = await storeService.CreateAsync(companyCode, storeCreateDto);

        return CreatedAtAction(
            nameof(GetStore),
            new { companyCode = newStore.CompanyCode, code = newStore.Code },
            newStore);
    }

    /// <summary>
    /// Gets a specific store by its code, within a specific company.
    /// </summary>
    /// <param name="code">The business code of the store.</param>
    /// <param name="companyCode">The business code of the parent company.</param>
    /// <returns>The details of the found store.</returns>
    /// <response code="200">Returns the requested store.</response>
    /// <response code="404">If the company or the store is not found.</response>
    [HttpGet("{code:int}")]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStore(int code, int companyCode)
    {
        return Ok(await storeService.GetAsync(code, companyCode));
    }

    /// <summary>
    /// Gets a list of all stores for a specific company.
    /// </summary>
    /// <param name="companyCode">The business code of the parent company.</param>
    /// <returns>A list of stores.</returns>
    /// <response code="200">Returns the list of stores (can be empty if the company has no stores or does not exist).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<StoreDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStores(int companyCode)
    {
        return Ok(await storeService.GetAllAsync(companyCode));
    }

    /// <summary>
    /// Updates an existing store.
    /// </summary>
    /// <param name="code">The business code of the store to update.</param>
    /// <param name="companyCode">The business code of the parent company.</param>
    /// <param name="storeUpdateDto">The updated data for the store.</param>
    /// <returns>The updated store.</returns>
    /// <response code="200">Returns the updated store.</response>
    /// <response code="400">If the request payload is invalid.</response>
    /// <response code="404">If the company or store is not found.</response>
    [HttpPut("{code:int}")]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int code, int companyCode, [FromBody] StoreUpdateDto storeUpdateDto)
    {
        return Ok(await storeService.UpdateAsync(code, companyCode, storeUpdateDto));
    }

    /// <summary>
    /// Deletes a store from a company.
    /// </summary>
    /// <param name="code">The business code of the store to delete.</param>
    /// <param name="companyCode">The business code of the parent company.</param>
    /// <response code="204">If the store was deleted successfully.</response>
    /// <response code="404">If the company or store is not found.</response>
    [HttpDelete("{code:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int code, int companyCode)
    {
        await storeService.DeleteAsync(code, companyCode);
        return NoContent();
    }
}