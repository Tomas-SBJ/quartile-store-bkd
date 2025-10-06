using Microsoft.AspNetCore.Mvc;
using QuartileStore.Commons.Dtos.Companies;
using QuartileStore.Commons.Dtos.Errors;
using QuartileStore.Commons.Services.Contracts;

namespace QuartileStore.Api.Controllers;

/// <summary>
/// Contains the endpoints for managing Companies
/// </summary>
[ApiController]
[Route("api/companies")]
public class CompanyController(
    ICompanyService companyService
) : ControllerBase
{
    /// <summary>
    /// Create a new Company
    /// </summary>
    /// <param name="companyCreateDto">The data for the new company</param>
    /// <returns>The newly created company</returns>
    /// <response code="201">Returns the newly created company and its location in the Location header</response>
    /// <response code="400">If the request payload is invalid</response>
    /// <response code="409">If a company with the same code already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CompanyCreateDto companyCreateDto)
    {
        var newCompany = await companyService.CreateAsync(companyCreateDto);

        return CreatedAtAction(
            nameof(GetCompany),
            new { code = newCompany.Code },
            newCompany);
    }

    /// <summary>
    /// Gets a especific company by its business code
    /// </summary>
    /// <param name="code">The unique business code for the company</param>
    /// <returns>The details of the found company</returns>
    /// <response code="200">Returns the requested company</response>
    /// <response code="404">If the company with specified code is not found</response>
    [HttpGet("{code:int}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompany(int code)
    {
        return Ok(await companyService.GetAsync(code));
    }

    /// <summary>
    /// Gets a list of all companies
    /// </summary>
    /// <returns>A list of companies</returns>
    /// <response code="200">Sucessfully returns the list of companies</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompanies()
    {
        return Ok(await companyService.GetAllAsync());
    }

    /// <summary>
    /// Updates an existing company.
    /// </summary>
    /// <param name="code">The business code of the company to update.</param>
    /// <param name="companyUpdateDto">The updated data for the company.</param>
    /// <returns>The updated company.</returns>
    /// <response code="200">Returns the updated company.</response>
    /// <response code="400">If the request payload is invalid.</response>
    /// <response code="404">If the company to be updated is not found.</response>
    [HttpPut("{code:int}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int code, [FromBody] CompanyUpdateDto companyUpdateDto)
    {
        return Ok(await companyService.UpdateAsync(code, companyUpdateDto));
    }

    /// <summary>
    /// Deletes a company.
    /// </summary>
    /// <remarks>
    /// Note: A company cannot be deleted if it has associated stores.
    /// </remarks>
    /// <param name="code">The business code of the company to delete.</param>
    /// <response code="204">If the company was deleted successfully.</response>
    /// <response code="404">If the company to be deleted is not found.</response>
    /// <response code="409">If the company cannot be deleted because it has associated stores.</response>
    [HttpDelete("{code:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int code)
    {
        await companyService.DeleteAsync(code);
        return NoContent();
    }
}