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
    public async Task<CompanyDto> CreateAsync(CompanyCreateDto companyCreateDto)
    {
        var companyAlreadyExists = await companyRepository.Exists(x => x.Code == companyCreateDto.Code);

        if (companyAlreadyExists)
            throw new EntityAlreadyExistsException($"Company with code: {companyCreateDto.Code} already exists");

        var company = new Company
        {
            Code = companyCreateDto.Code,
            Name = companyCreateDto.Name,
            CountryCode = companyCreateDto.CountryCode
        };

        await companyRepository.CreateAsync(company);
        await unitOfWork.Commit();

        return new CompanyDto
        {
            Code = company.Code,
            Name = company.Name,
            CountryCode = company.CountryCode
        };
    }

    public async Task<CompanyDto> GetAsync(int code)
    {
        var company = await companyRepository.SelectOneByAsync(x => x.Code == code);

        if (company is null)
            throw new EntityNotFoundException($"Company with code: {code} was not found");

        return new CompanyDto
        {
            Code = company.Code,
            Name = company.Name,
            CountryCode = company.CountryCode
        };
    }

    public async Task<IEnumerable<CompanyDto>> GetAllAsync()
    {
        var companies = await companyRepository.SelectAllAsync();
        return companies.Select(x => new CompanyDto
        {
            Code = x.Code,
            Name = x.Name,
            CountryCode = x.CountryCode
        }).OrderBy(x => x.Code);
    }

    public async Task<CompanyDto> UpdateAsync(int code, CompanyUpdateDto companyUpdateDto)
    {
        var company = await companyRepository.SelectOneByAsync(x => x.Code == code);
        
        if (company is null)
            throw new EntityNotFoundException($"Company with code: {code} was not found");
        
        company.Update(companyUpdateDto.Name, companyUpdateDto.CountryCode);
        await unitOfWork.Commit();
        
        return new CompanyDto
        {
            Code = company.Code,
            Name = company.Name,
            CountryCode = company.CountryCode
        };
    }

    public async Task DeleteAsync(int code)
    {
        var company = await companyRepository.SelectOneByAsync(x => x.Code == code);
        
        if (company is null)
            throw new EntityNotFoundException($"Company with code: {code} was not found");
        
        var hasStores = await companyRepository.HasStoresAsync(company.Id);

        if (hasStores)
            throw new DeleteConflictException("It is not possible to delete a company that has associated stores");
        
        companyRepository.Delete(company);
        await unitOfWork.Commit();
    }
}