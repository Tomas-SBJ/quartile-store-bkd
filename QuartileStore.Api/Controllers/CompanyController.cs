using Microsoft.AspNetCore.Mvc;
using QuartileStore.Commons.Dtos.Companies;
using QuartileStore.Commons.Services.Contracts;

namespace QuartileStore.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController(
    ICompanyService companyService
    ) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto createCompanyDto)
    {
        var newCompany = await companyService.CreateAsync(createCompanyDto);

        return CreatedAtAction(
            nameof(GetCompany),
            new { code = newCompany.Code },
            newCompany);
    }

    [HttpGet("{code:int}")]
    public async Task<IActionResult> GetCompany(int code)
    {
        return Ok(await companyService.GetAsync(code));
    }
}