using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Dtos.Stores;

namespace QuartileStore.Commons.Services.Contracts;

public interface IStoreService
{
    Task<StoreDto> CreateAsync(int companyCode, CreateStoreDto storeDto);
    Task<Store> UpdateAsync(int code, int companyCode, UpdateStoreDto storeDto);
    Task DeleteAsync(int code, int companyCode);
    Task<Store> GetAsync(int code, int companyCode);
    Task<List<Store>> GetAllAsync(int companyCode);
}