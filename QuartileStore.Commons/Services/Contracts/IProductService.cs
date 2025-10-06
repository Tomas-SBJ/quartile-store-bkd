using QuartileStore.Commons.Dtos.Products;

namespace QuartileStore.Commons.Services.Contracts;

public interface IProductService
{
    Task<ProductDto> CreateAsync(int storeCode, int companyCode, ProductCreateDto productCreateDto);
    Task<ProductDto> UpdateAsync(int code, int storeCode, int companyCode, ProductUpdateDto productUpdateDto);
    Task<ProductDto> GetAsync(int code, int storeCode, int companyCode);
    Task<IEnumerable<ProductDto>> GetAllByStoreAsync(int storeCode, int companyCode);
    Task DeleteAsync(int code, int storeCode, int companyCode);
}