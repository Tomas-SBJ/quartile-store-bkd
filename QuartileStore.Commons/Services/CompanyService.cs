using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Dtos.Companies;
using QuartileStore.Commons.Exceptions;
using QuartileStore.Commons.Infrastructure.Transactions;
using QuartileStore.Commons.Services.Contracts;

namespace QuartileStore.Commons.Services;

internal class CompanyService(
    ICompanyRepository companyRepository,
    IUnitOfWork unitOfWork
    ) : ICompanyService
{
    public async Task<CompanyDto> CreateAsync(CreateCompanyDto createCompanyDto)
    {
        var companyAlreadyExists = await companyRepository.Exists(x => x.Code == createCompanyDto.Code);

        if (companyAlreadyExists)
            throw new EntityAlreadyExistsException($"Company with code: {createCompanyDto.Code} already exists");

        var company = new Company
        {
            Code = createCompanyDto.Code,
            Name = createCompanyDto.Name,
            CountryCode = createCompanyDto.CountryCode
        };
        
        await companyRepository.CreateAsync(company);
        await unitOfWork.Commit();

        return new CompanyDto(company.Code, company.Name, company.CountryCode);
    }

    public async Task<CompanyDto> GetAsync(int code)
    {
        var company = await companyRepository.SelectOneByAsync(x => x.Code == code);
        
        if (company is null)
            throw new EntityNotFoundException($"Company with code: {code} was not found");
        
        return new CompanyDto(company.Code, company.Name, company.CountryCode);
    }
}