using QuartileStore.Commons.Dtos.Stores;

namespace QuartileStore.Commons.Services.Contracts;

public interface IStoreService
{
    Task<StoreDto> CreateAsync(int companyCode, StoreCreateDto dto);
    Task<StoreDto> UpdateAsync(int code, int companyCode, StoreUpdateDto dto);
    Task DeleteAsync(int code, int companyCode);
    Task<StoreDto> GetAsync(int code, int companyCode);
    Task<IEnumerable<StoreDto>> GetAllAsync(int companyCode);
}