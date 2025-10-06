using QuartileStore.Commons.Application.Dtos.Stores;

namespace QuartileStore.Commons.Application.Services.Contracts;

public interface IStoreService
{
    Task<StoreDto> CreateAsync(int companyCode, StoreCreateDto dto);
    Task<StoreDto> UpdateAsync(int code, int companyCode, StoreUpdateDto dto);
    Task DeleteAsync(int code, int companyCode);
    Task<StoreDto> GetAsync(int code, int companyCode);
    Task<IEnumerable<StoreDto>> GetAllAsync(int companyCode);
}