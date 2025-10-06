using Moq;
using QuartileStore.Commons.Application.Dtos.Products;
using QuartileStore.Commons.Application.Exceptions;
using QuartileStore.Commons.Application.Services;
using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Domain.Entities.Products;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Infrastructure.Transactions;

namespace QuartileStore.UnitTests.Application.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStoreRepository> _storeRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _storeRepositoryMock = new Mock<IStoreRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();

        _productService = new ProductService(
            _productRepositoryMock.Object,
            _storeRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenProductDoesNotExist_ShouldCreateStore()
    {
        var company = new Company
        {
            Code = 1,
            Name = "Name",
            CountryCode = "USA"
        };

        var store = new Store
        {
            Code = 1,
            Name = "Name",
            Address = "Brazil",
            Company = company
        };

        var productCreateDto = new ProductCreateDto
        {
            Code = 1,
            Description = "Description",
            Name = "Name",
            Price = (decimal)3.15
        };

        _storeRepositoryMock.Setup(r => r.SelectOneWithCompanyAsync(store.Code, company.Code))
            .ReturnsAsync(store);

        _productRepositoryMock
            .Setup(r => r.Exists(x => x.Code == productCreateDto.Code && x.StoreId == store.Id))
            .ReturnsAsync(false);

        var result = await _productService.CreateAsync(store.Code, company.Code, productCreateDto);

        Assert.NotNull(result);
        Assert.Equal(productCreateDto.Code, result.Code);
        Assert.Equal(productCreateDto.Description, result.Description);
        Assert.Equal(productCreateDto.Name, result.Name);

        _unitOfWorkMock.Verify(r => r.Commit(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenStoreDoesNotExists_ShouldThrowEntityAlreadyExistsException()
    {
        const int companyCode = 1;
        const int storeCode = 1;

        var productCreateDto = new ProductCreateDto
        {
            Code = 1,
            Name = "Name",
            Description = "Description",
            Price = (decimal)3.15
        };

        _storeRepositoryMock
            .Setup(r => r.SelectOneWithCompanyAsync(storeCode, companyCode))
            .ReturnsAsync(null as Store);

        var act = () => _productService.CreateAsync(storeCode, companyCode, productCreateDto);

        await Assert.ThrowsAsync<EntityNotFoundException>(act);

        _productRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenProductAlreadyExists_ShouldThrowEntityAlreadyExistsException()
    {
        const int companyCode = 1;

        var store = new Store
        {
            Code = 1,
            Name = "Test Store",
            Address = "Brazil",
            Company = new Company
            {
                Code = companyCode,
                Name = "Test Company",
                CountryCode = "USA"
            }
        };

        var productCreateDto = new ProductCreateDto
        {
            Code = 1,
            Description = "Description",
            Name = "Test Name",
            Price = (decimal)3.15
        };

        _storeRepositoryMock.Setup(r => r.SelectOneWithCompanyAsync(store.Code, companyCode))
            .ReturnsAsync(store);

        _productRepositoryMock
            .Setup(r => r.Exists(x => x.Code == productCreateDto.Code && x.StoreId == store.Id))
            .ReturnsAsync(true);

        var act = () => _productService.CreateAsync(store.Code, companyCode, productCreateDto);

        await Assert.ThrowsAsync<EntityAlreadyExistsException>(act);

        _productRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }

    [Fact]
    public async Task GetAsync_WhenProductExists_ReturnsProductDto()
    {
        const int productCode = 1;

        var product = new Product
        {
            Code = productCode,
            Description = "Description",
            Name = "Name",
            Price = (decimal)3.15,
            Store = new Store
            {
                Code = 1,
                Name = "Test",
                Address = "Brazil",
                Company = new Company
                {
                    Code = 1,
                    Name = "Test",
                    CountryCode = "BRA"
                }
            }
        };

        _productRepositoryMock
            .Setup(r => r.SelectOneWithHierarchyAsync(productCode, product.Store.Code, product.Store.Company.Code))
            .ReturnsAsync(product);

        var result = await _productService.GetAsync(productCode, product.Store.Code, product.Store.Company.Code);

        Assert.NotNull(result);
        Assert.Equal(product.Code, result.Code);
        Assert.Equal(product.Name, result.Name);
    }

    [Fact]
    public async Task GetAsync_WhenProductDoesNotExist_ShouldThrowEntityNotFoundException()
    {
        const int companyCode = 1;
        const int storeCode = 1;
        const int productCode = 1;

        _productRepositoryMock
            .Setup(r => r.SelectOneWithHierarchyAsync(productCode, storeCode, companyCode))
            .ReturnsAsync(null as Product);

        var act = () => _productService.GetAsync(productCode, storeCode, companyCode);

        await Assert.ThrowsAsync<EntityNotFoundException>(act);
    }

    [Fact]
    public async Task GetAllAsync_WhenProductsExist_ShouldReturnProductDtoList()
    {
        const int storeCode = 1;
        const int companyCode = 1;

        var store = new Store
        {
            Code = storeCode,
            Name = "Store One",
            Address = "Brazil",
            Company = new Company
            {
                Code = companyCode,
                Name = "Test",
                CountryCode = "USA"
            }
        };

        var products = new List<Product>
        {
            new()
            {
                Code = 1,
                Description = "Description",
                Name = "Name",
                Price = (decimal)3.15,
                Store = store
            },
            new()
            {
                Code = 2,
                Description = "Description2",
                Name = "Name2",
                Price = (decimal)3.16,
                Store = store
            }
        };

        _productRepositoryMock
            .Setup(r => r.SelectAllWithHierarchyAsync(storeCode, companyCode))
            .ReturnsAsync(products);

        var result = await _productService.GetAllByStoreAsync(storeCode, companyCode);

        Assert.NotNull(result);
        var productDtos = result.ToList();
        Assert.Equal(2, productDtos.Count);

        var firstProduct = productDtos.First();
        Assert.Equal(productDtos.First().Code, firstProduct.Code);
        Assert.Equal(productDtos.First().Name, firstProduct.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductExists_ShouldUpdatePropertiesAndComplete()
    {
        const int companyCode = 1;
        const int storeCode = 1;
        const int productCode = 1;

        var product = new Product
        {
            Code = productCode,
            Description = "Description",
            Name = "Name",
            Price = (decimal)3.15,
            Store = new Store
            {
                Code = storeCode,
                Name = "Test",
                Address = "Brazil",
                Company = new Company
                {
                    Code = companyCode,
                    Name = "Test",
                    CountryCode = "BRA"
                }
            }
        };

        var productUpdateDto = new ProductUpdateDto
        {
            Name = "Updated Name",
            Description = "Updated",
            Price = (decimal)3.20,
        };

        _productRepositoryMock
            .Setup(r => r.SelectOneWithHierarchyAsync(productCode, storeCode, companyCode))
            .ReturnsAsync(product);

        await _productService.UpdateAsync(productCode, storeCode, companyCode, productUpdateDto);

        Assert.Equal(product.Name, productUpdateDto.Name);
        Assert.Equal(product.Price, productUpdateDto.Price);

        _unitOfWorkMock.Verify(r => r.Commit(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExists_ShouldThrowEntityNotFoundException()
    {
        const int companyCode = 1;
        const int storeCode = 1;
        const int productCode = 1;

        _productRepositoryMock
            .Setup(r => r.SelectOneWithHierarchyAsync(productCode, storeCode, companyCode))
            .ReturnsAsync(null as Product);

        var act = () => _productService.UpdateAsync(productCode, storeCode, companyCode, new ProductUpdateDto());

        await Assert.ThrowsAsync<EntityNotFoundException>(act);

        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ShouldDeleteProduct()
    {
        const int companyCode = 1;
        const int storeCode = 1;
        const int productCode = 1;

        var product = new Product
        {
            Code = productCode,
            Description = "Description",
            Name = "Name",
            Price = (decimal)3.15,
            Store = new Store
            {
                Code = storeCode,
                Name = "Test",
                Address = "Brazil",
                Company = new Company
                {
                    Code = companyCode,
                    Name = "Test",
                    CountryCode = "BRA"
                }
            }
        };

        _productRepositoryMock
            .Setup(r => r.SelectOneWithHierarchyAsync(productCode, storeCode, companyCode))
            .ReturnsAsync(product);

        await _productService.DeleteAsync(productCode, storeCode, companyCode);

        _productRepositoryMock.Verify(r => r.Delete(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExists_ShouldThrowEntityNotFoundException()
    {
        const int companyCode = 1;
        const int storeCode = 1;
        const int productCode = 1;

        _productRepositoryMock
            .Setup(r => r.SelectOneWithHierarchyAsync(productCode, storeCode, companyCode))
            .ReturnsAsync(null as Product);

        var act = () => _productService.DeleteAsync(productCode, storeCode, companyCode);

        await Assert.ThrowsAsync<EntityNotFoundException>(act);

        _productRepositoryMock.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.Commit(), Times.Never);
    }
}