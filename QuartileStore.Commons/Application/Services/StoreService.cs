using QuartileStore.Commons.Application.Dtos.Stores;
using QuartileStore.Commons.Application.Exceptions;
using QuartileStore.Commons.Application.Services.Contracts;
using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Infrastructure.Transactions;

namespace QuartileStore.Commons.Application.Services;

internal class StoreService(
    IStoreRepository storeRepository,
    ICompanyRepository companyRepository,
    IUnitOfWork unitOfWork
) : IStoreService
{
    public async Task<StoreDto> CreateAsync(int companyCode, StoreCreateDto dto)
    {
        var company = await companyRepository.SelectOneByAsync(x => x.Code == companyCode);

        if (company is null)
            throw new EntityNotFoundException($"Company with code: {companyCode} was not found");

        var storeAlreadyExists = await storeRepository.Exists(x =>
            x.Code == dto.Code &&
            x.CompanyId == company.Id);

        if (storeAlreadyExists)
            throw new EntityAlreadyExistsException($"Store with code {dto.Code} already exists");

        var store = new Store
        {
            Code = dto.Code,
            Name = dto.Name,
            Address = dto.Address,
            CompanyId = company.Id,
            Company = company
        };

        await storeRepository.CreateAsync(store);
        await unitOfWork.Commit();

        return new StoreDto
        {
            Code = store.Code,
            CompanyCode = store.Company.Code,
            Name = store.Name,
            Address = store.Address
        };
    }

    public async Task<StoreDto> UpdateAsync(int code, int companyCode, StoreUpdateDto dto)
    {
        var store = await storeRepository.SelectOneWithCompanyAsync(code, companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code: {code} was not found");

        store.Update(dto.Name, dto.Address);
        await unitOfWork.Commit();

        return new StoreDto
        {
            Code = store.Code,
            CompanyCode = store.Company.Code,
            Name = store.Name,
            Address = store.Address
        };
    }

    public async Task DeleteAsync(int code, int companyCode)
    {
        var store = await storeRepository.SelectOneByAsync(x => x.Code == code && x.Company.Code == companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code: {code} was not found");

        var hasProducts = await storeRepository.HasProductsAsync(store.Id);

        if (hasProducts)
            throw new DeleteConflictException("It is not possible to delete a store that has associated products");
        
        storeRepository.Delete(store);
        await unitOfWork.Commit();
    }

    public async Task<StoreDto> GetAsync(int code, int companyCode)
    {
        var store = await storeRepository.SelectOneWithCompanyAsync(code, companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code: {code} was not found");

        return new StoreDto
        {
            Code = store.Code,
            CompanyCode = store.Company.Code,
            Name = store.Name,
            Address = store.Address
        };
    }

    public async Task<IEnumerable<StoreDto>> GetAllAsync(int companyCode)
    {
        var stores = await storeRepository.SelectAllByCompanyCodeAsync(companyCode);
        return stores.Select(x => new StoreDto
        {
            Code = x.Code,
            CompanyCode = x.Company.Code,
            Name = x.Name,
            Address = x.Address
        }).OrderBy(x => x.Code);
    }
}