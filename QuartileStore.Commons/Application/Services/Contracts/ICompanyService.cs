using QuartileStore.Commons.Application.Dtos.Companies;

namespace QuartileStore.Commons.Application.Services.Contracts;

public interface ICompanyService
{
    Task<CompanyDto> CreateAsync(CompanyCreateDto companyCreateDto);
    Task<CompanyDto> GetAsync(int code);
    Task<IEnumerable<CompanyDto>> GetAllAsync();
    Task<CompanyDto> UpdateAsync(int code, CompanyUpdateDto companyUpdateDto);
    Task DeleteAsync(int code);
}