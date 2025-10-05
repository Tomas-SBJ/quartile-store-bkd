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

    public async Task<Store> UpdateAsync(int code, int companyCode, UpdateStoreDto storeDto)
    {
        var store = await storeRepository.SelectOneByAsync(x => x.Code == code && x.Company.Code == companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code: {code} and company code {companyCode} was not found");

        store.Update(storeDto.Name, storeDto.Address);

        await unitOfWork.Commit();
        
        return store;
    }

    public async Task DeleteAsync(int code, int companyCode)
    {
        var store = await storeRepository.SelectOneByAsync(x => x.Code == code && x.Company.Code == companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code: {code} and company code: {companyCode} was not found");

        storeRepository.DeleteAsync(store);
    }

    public async Task<Store> GetAsync(int code, int companyCode)
    {
        var store = await storeRepository.SelectOneByAsync(x => x.Code == code && x.Company.Code == companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code: {code} and company code: {companyCode} was not found");

        return store;
    }

    public async Task<List<Store>> GetAllAsync(int companyCode)
    {
        var company = await companyRepository.SelectOneByAsync(x => x.Code == companyCode);

        if (company is null)
            throw new EntityNotFoundException($"Company with code: {companyCode} was not found");

        return await storeRepository.SelectAllByCompanyAsync(company.Id);
    }
}