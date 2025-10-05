using QuartileStore.Commons.Dtos.Companies;

namespace QuartileStore.Commons.Services.Contracts;

public interface ICompanyService
{
    Task<CompanyDto> CreateAsync(CreateCompanyDto createCompanyDto);
    Task<CompanyDto> GetAsync(int code);
}