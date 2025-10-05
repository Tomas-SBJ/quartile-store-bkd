using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Dtos.Stores;
using QuartileStore.Commons.Exceptions;
using QuartileStore.Commons.Infrastructure.Transactions;
using QuartileStore.Commons.Services.Contracts;

namespace QuartileStore.Commons.Services;

internal class StoreService(
    IStoreRepository storeRepository,
    ICompanyRepository companyRepository,
    IUnitOfWork unitOfWork
) : IStoreService
{
    public async Task<StoreDto> CreateAsync(int companyCode, CreateStoreDto storeDto)
    {
        var company = await companyRepository.SelectOneByAsync(x => x.Code == companyCode);

        if (company is null)
            throw new EntityNotFoundException($"Company with code: {companyCode} was not found");

        var storeAlreadyExists = await storeRepository.Exists(x =>
            x.Code == storeDto.Code &&
            x.CompanyId == company.Id);

        if (storeAlreadyExists)
            throw new EntityAlreadyExistsException($"Store with code {storeDto.Code} already exists");

        var store = new Store
        {
            Code = storeDto.Code,
            Name = storeDto.Name,
            Address = storeDto.Address,
            CompanyId = company.Id,
            Company = company
        };

        await storeRepository.CreateAsync(store);
        await unitOfWork.Commit();

        return new StoreDto(store.Code, store.Company.Code, store.Name, store.Address);
    }

    public async Task<StoreDto> UpdateAsync(int code, int companyCode, UpdateStoreDto storeDto)
    {
        var store = await storeRepository.SelectOneWithCompanyAsync(x => x.Code == code && x.Company.Code == companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code: {code} was not found");

        store.Update(storeDto.Name, storeDto.Address);
        await unitOfWork.Commit();

        return new StoreDto(store.Code, store.Company.Code, store.Name, store.Address);
    }

    public async Task DeleteAsync(int code, int companyCode)
    {
        var store = await storeRepository.SelectOneByAsync(x => x.Code == code && x.Company.Code == companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code: {code} was not found");

        storeRepository.Delete(store);
        await unitOfWork.Commit();
    }

    public async Task<StoreDto> GetAsync(int code, int companyCode)
    {
        var store = await storeRepository.SelectOneWithCompanyAsync(x => x.Code == code && x.Company.Code == companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code: {code} was not found");

        return new StoreDto(store.Code, store.Company.Code, store.Name, store.Address);
    }

    public async Task<List<StoreDto>> GetAllAsync(int companyCode)
    {
        var stores = await storeRepository.SelectAllByCompanyCodeAsync(companyCode);
        return stores.Select(x => new StoreDto(x.Code, x.Company.Code, x.Name, x.Address)).OrderBy(x => x.Code).ToList();
    }
}