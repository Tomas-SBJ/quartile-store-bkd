using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Dtos.Stores;

namespace QuartileStore.Commons.Services.Contracts;

public interface IStoreService
{
    Task<StoreDto> CreateAsync(int companyCode, CreateStoreDto storeDto);
    Task<StoreDto> UpdateAsync(int code, int companyCode, UpdateStoreDto storeDto);
    Task DeleteAsync(int code, int companyCode);
    Task<StoreDto> GetAsync(int code, int companyCode);
    Task<List<StoreDto>> GetAllAsync(int companyCode);
}