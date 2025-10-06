using QuartileStore.Commons.Application.Dtos.Products;
using QuartileStore.Commons.Application.Exceptions;
using QuartileStore.Commons.Application.Services.Contracts;
using QuartileStore.Commons.Domain.Entities.Products;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Infrastructure.Transactions;

namespace QuartileStore.Commons.Application.Services;

internal class ProductService(
    IProductRepository productRepository,
    IStoreRepository storeRepository,
    IUnitOfWork unitOfWork
) : IProductService
{
    public async Task<ProductDto> CreateAsync(int storeCode, int companyCode, ProductCreateDto productCreateDto)
    {
        var store = await storeRepository.SelectOneWithCompanyAsync(storeCode, companyCode);

        if (store is null)
            throw new EntityNotFoundException($"Store with code {storeCode} was not found");

        var productAlreadyExists = await productRepository.Exists(x =>
            x.Code == productCreateDto.Code &&
            x.StoreId == store.Id);

        if (productAlreadyExists)
            throw new EntityAlreadyExistsException($"Product with code {productCreateDto.Code} already exists");

        var product = new Product
        {
            Code = productCreateDto.Code,
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            Price = productCreateDto.Price,
            StoreId = store.Id,
            Store = store
        };

        await productRepository.CreateAsync(product);
        await unitOfWork.Commit();

        return new ProductDto
        {
            Code = product.Code,
            StoreCode = product.Store.Code,
            CompanyCode = product.Store.Company.Code,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
    }

    public async Task<ProductDto> UpdateAsync(int code, int storeCode, int companyCode,
        ProductUpdateDto productUpdateDto)
    {
        var product = await productRepository.SelectOneWithHierarchyAsync(code, storeCode, companyCode);

        if (product is null)
            throw new EntityNotFoundException($"Product with code {code} was not found");

        product.Update(productUpdateDto.Name, productUpdateDto.Description, productUpdateDto.Price);
        await unitOfWork.Commit();

        return new ProductDto
        {
            Code = product.Code,
            StoreCode = product.Store.Code,
            CompanyCode = product.Store.Company.Code,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
    }

    public async Task<ProductDto> GetAsync(int code, int storeCode, int companyCode)
    {
        var product = await productRepository.SelectOneWithHierarchyAsync(code, storeCode, companyCode);

        if (product is null)
            throw new EntityNotFoundException($"Product with code {code} was not found");

        return new ProductDto
        {
            Code = product.Code,
            StoreCode = product.Store.Code,
            CompanyCode = product.Store.Company.Code,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
    }

    public async Task<IEnumerable<ProductDto>> GetAllByStoreAsync(int storeCode, int companyCode)
    {
        var products = await productRepository.SelectAllWithHierarchyAsync(storeCode, companyCode);
        return products.Select(x => new ProductDto
        {
            Code = x.Code,
            StoreCode = x.Store.Code,
            CompanyCode = x.Store.Company.Code,
            Name = x.Name,
            Description = x.Description,
            Price = x.Price
        }).OrderBy(x => x.Code);
    }

    public async Task DeleteAsync(int code, int storeCode, int companyCode)
    {
        var product = await productRepository.SelectOneWithHierarchyAsync(code, storeCode, companyCode);

        if (product is null)
            throw new EntityNotFoundException($"Product with code {code} was not found");

        productRepository.Delete(product);
        await unitOfWork.Commit();
    }
}